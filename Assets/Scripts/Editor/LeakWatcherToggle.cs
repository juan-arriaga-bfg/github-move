﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class LeakWatcherToggle : MonoBehaviour
{
    private static readonly Regex getClassNamaRegex = new Regex("(?<=class\\s)([a-z]|[A-Z]|[0-9]|_)*", RegexOptions.Compiled);

    private static List<string> ignores = new List<string>
    {
        "/Editor/",
        "/AutoGenerated/",
        "AsyncInitManager.cs",// defines with '{' or '}',
        "LeakWatcher.cs",
        "LeakWatcherToggle.cs",        
    };

    [MenuItem("Tools/Print Leak Watcher")]
    public static void Print()
    {
        LeakWatcher.Instance.DataAsString(Input.GetKey(KeyCode.LeftShift));
    }
    
    [MenuItem("Tools/Enable Leak Watcher")]
    public static void Process()
    {
        var files = Directory.GetFiles(Application.dataPath + "/Scripts", "*.cs", SearchOption.AllDirectories);

        int limit = int.MaxValue;
        for (var i = 0; i < files.Length; i++)
        {           
            string file = files[i].Replace("\\", "/");

            bool skipThisFile = false;
            foreach (var ignoreDir in ignores)
            {
                if (file.Contains(ignoreDir))
                {
                    skipThisFile = true;
                    break;
                }
            }

            // if (!file.Contains("APurchaseController.cs"))
            // {
            //     skipThisFile = true;
            // }
            
            if (skipThisFile)
            {
                continue;
            }
            
            Debug.Log($"=== Process file: [{i}/{files.Length}] {file} ===");

            if (ProcessFile(file))
            {
                limit--;
            }
            else
            {
                Debug.Log($"No changes are made!"); 
            }

            if (limit <= 0)
            {
                return;
            }
        }
    }

    private static bool ProcessFile(string file)
    {
        string text = File.ReadAllText(file);
        text = RemoveComments(text);

        StringBuilder output = new StringBuilder();
        
        using (StringReader reader = new StringReader(text))
        {
            bool isClassFound = false;
            string className = null;

            bool needWriteDtorToNextLine = false;
            bool needWriteCtorToNextLine = false;
            bool isDtorWritten = false;
            bool isCtorWritten = false;
            bool firstParenthesesInClassFound = false;

            bool isFileModified = false;

            int deltaParentheses = 0;

            int lineNumber = 0;
            
            string line = string.Empty;
            do
            {
                line = reader.ReadLine();
                lineNumber++;
                
                if (line != null)
                {
                    if (needWriteDtorToNextLine)
                    {
                        int pos = line.IndexOf("{");
                        pos = Mathf.Max(0, pos);
                        
                        line = line.Insert(pos + 1, GetLeakDtorCall());
                        isFileModified = true;
                        
                        needWriteDtorToNextLine = false;
                        isDtorWritten = true;
                        goto INC_PARENTHESES;
                    }
                    
                    if (needWriteCtorToNextLine)
                    {
                        line = WriteCtorToLine(line);
                        isFileModified = true;
                        
                        needWriteCtorToNextLine = false;
                        isCtorWritten = true;
                        goto INC_PARENTHESES;
                    }

                    if (isClassFound && !firstParenthesesInClassFound)
                    {
                        if (line.Contains("{"))
                        {
                            firstParenthesesInClassFound = true;
                        }
                    }
                    
                    if (!isClassFound && line.Contains(" class ") && !line.Contains("static") && !line.Contains("abstract")
                        && (!line.Contains(" partial ")) // todo: support partials
                        )
                    {
                        isClassFound = true;
                        if (line.Contains("{"))
                        {
                            firstParenthesesInClassFound = true;
                        }
                        className = getClassNamaRegex.Match(line).Value;

                        if (string.IsNullOrEmpty(className))
                        {
                            Debug.LogError($"Empty class name for file {file} at line [{lineNumber}]: {line}");
                        }
                        
                        Debug.Log($"Class found at {lineNumber}: {className}");
                    }

                    INC_PARENTHESES:
                    if (isClassFound)
                    {
                        int inc = getDeltaOfParentheses(line);
                        deltaParentheses += inc;
                        
                        //Debug.Log($"Parentheses at {lineNumber}: {deltaParentheses}, delta: {deltaParentheses}  //{line}");
                    }

                    // search for ctor/dtor
                    if (isClassFound && firstParenthesesInClassFound)
                    {
                        if (line.Contains("~") && line.Contains(className))
                        {
                            Debug.Log($"DTOR found at {lineNumber}: {line}");
                            
                            needWriteDtorToNextLine = true;
                            goto WRITE;
                        }
                        
                        if ((line.Contains(" " + className + " ") || line.Contains(" " + className + "(") )
                         && (!line.Contains(" new "))
                         && (!line.Contains(" static "))
                         && (line.Contains("public") || line.Contains("protected ") || line.Contains("private ")) 
                         && line.Contains("("))
                        {
                            bool check = true;

                            string fixedLine = line.Replace("  ", " ");
                            int posOfToken = fixedLine.IndexOf(className);
                            int endOfToken = posOfToken + className.Length;
                            if (fixedLine[endOfToken] != '(' && fixedLine.Substring(endOfToken, 2) != " (")
                            {
                                check = false;
                            }

                            if (check)
                            {
                                Debug.Log($"CTOR found at {lineNumber}: {line}");

                                // ctor like this: private JSONNull() { }
                                if (getDeltaOfParentheses(line) == 0 && line.Contains("{") && line.Contains("}"))
                                {
                                    int pos = line.LastIndexOf("}");
                                    line = line.Insert(pos - 1, GetFullLeakCtorCall(line));

                                    pos = line.LastIndexOf("}");
                                    line = line.Insert(pos - 1, GetFullLeakDtorCall(line));

                                    isFileModified = true;
                                    isCtorWritten = true;
                                    isDtorWritten = true;
                                }
                                else
                                {
                                    needWriteCtorToNextLine = true;
                                }

                                goto WRITE;
                            }
                        }
                    }

                    // Finalize class
                    if (isClassFound && firstParenthesesInClassFound && deltaParentheses <= 0)
                    {
                        Debug.Log($"Class end at {lineNumber}: {className}");
                        
                        isClassFound = false;

                        if (!isDtorWritten)
                        {
                            Debug.Log($"No DTOR in {className}, write new");

                            int pos = line.LastIndexOf("}");
                            line = line.Insert(Mathf.Max(0, pos), GetFullLeakDtorCall(className));
                            isFileModified = true;
                            
                        }
                        
                        if (!isCtorWritten)
                        {
                            Debug.Log($"No CTOR in {className}, write new");
                            
                            int pos = line.LastIndexOf("}");
                            line = line.Insert(Mathf.Max(0, pos), GetFullLeakCtorCall(className));
                            isFileModified = true;
                            
                        }

                        isDtorWritten = false;
                        isCtorWritten = false;

                        firstParenthesesInClassFound = false;

                        if (deltaParentheses != 0)
                        {
                            Debug.LogError($"deltaParentheses == {deltaParentheses} at the end of class");
                        }
                        
                        Debug.Log($"Class processed: {className}");
                    }

                    WRITE:
                        output.AppendLine(line);
                }
            } while (line != null);

            if (isFileModified)
            {
                File.WriteAllText(file, output.ToString(), new UTF8Encoding(false));
                return true;
            }

            return false;
        }
    }

    private static string WriteCtorToLine(string line)
    {
        int pos = line.IndexOf("{");
        pos = Mathf.Max(0, pos);

        line = line.Insert(pos + 1, GetLeakCtorCall());
        return line;
    }

    private static string GetLeakCtorCall()
    {
        return "\n        LeakWatcher.Instance.Ctor(this);\n\n";
    }
    
    private static string GetLeakDtorCall()
    {
        return "\n        LeakWatcher.Instance.Dtor(this);\n\n";
    }
    
    private static string GetFullLeakCtorCall(string className)
    {
        return $"\n    public {className}() {{\n        LeakWatcher.Instance.Ctor(this);\n     }}\n";
    }
    
    private static string GetFullLeakDtorCall(string className)
    {
        return $"\n    ~{className}() {{\n        LeakWatcher.Instance.Dtor(this);\n    }}\n";
    }
    
    private static int getDeltaOfParentheses(string line)
    {
        int open = line.Count(f => f == '{');
        int close = line.Count(f => f == '}');

        int total = open - close;

        return total;
    }
    
    private static string RemoveComments(string input)
    {
        var blockComments   = @"/\*(.*?)\*/";
        var lineComments    = @"//(.*?)\r?\n";
        var strings         = @"""((\\[^\n]|[^""\n])*)""";
        var verbatimStrings = @"@(""[^""]*"")+";
            
        string noComments = Regex.Replace(input,
            blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
            me => {
                if (me.Value.StartsWith("/*") || me.Value.StartsWith("//"))
                    return me.Value.StartsWith("//") ? Environment.NewLine : "";
                // Keep the literal strings
                return me.Value;
            },
            RegexOptions.Singleline);

        return noComments;
    }
}

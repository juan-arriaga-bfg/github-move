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

    private static List<string> ignoreDirs = new List<string>
    {
        "/Editor/",
        "/AutoGenerated/"
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

        int limit = 100;
        for (var i = 0; i < files.Length; i++)
        {           
            string file = files[i].Replace("\\", "/");

            bool skipThisFile = false;
            foreach (var ignoreDir in ignoreDirs)
            {
                if (file.Contains(ignoreDir))
                {
                    skipThisFile = true;
                    break;
                }
            }

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
            bool firstLineInClass = false;

            bool isFileModified = false;

            int deltaParentheses = 0;
            
            string line = string.Empty;
            do
            {
                line = reader.ReadLine();
                if (line != null)
                {
                    if (needWriteDtorToNextLine)
                    {
                        int pos = line.IndexOf("{");
                        pos = Mathf.Max(0, pos);
                        
                        line = line.Insert(pos, GetLeakDtorCall());
                        isFileModified = true;
                        
                        needWriteDtorToNextLine = false;
                        isDtorWritten = true;
                        goto WRITE;
                    }
                    
                    if (needWriteCtorToNextLine)
                    {
                        int pos = line.IndexOf("{");
                        pos = Mathf.Max(0, pos);
                        
                        line = line.Insert(pos, GetLeakCtorCall());
                        isFileModified = true;
                        
                        needWriteCtorToNextLine = false;
                        isCtorWritten = true;
                        goto WRITE;
                    }
                    
                    if (!isClassFound && line.Contains(" class ") && !line.Contains("static") && !line.Contains("abstract"))
                    {
                        isClassFound = true;
                        firstLineInClass = true;
                        className = getClassNamaRegex.Match(line).Value;
                        
                        Debug.Log($"Class found: {className}");
                    }

                    if (isClassFound)
                    {
                        deltaParentheses += getCountOfParentheses(line);
                    }

                    // search for ctor/dtor
                    if (isClassFound && !firstLineInClass)
                    {
                        if (line.Contains("~") && line.Contains(className))
                        {
                            Debug.Log($"DTOR found: {line}");
                            
                            needWriteDtorToNextLine = true;
                            goto WRITE;
                        }
                        
                        if ((line.Contains(" " + className + " ") || line.Contains(" " + className + "(") )
                         && (line.Contains("public") || line.Contains("protected") || line.Contains("private")))
                        {
                            Debug.Log($"CTOR found: {line}");
                            
                            needWriteCtorToNextLine = true;
                            goto WRITE;
                        }
                    }

                    // Finalize class
                    if (isClassFound && !firstLineInClass && deltaParentheses <= 0)
                    {
                        isClassFound = false;

                        if (!isDtorWritten)
                        {
                            Debug.Log($"No DTOR in {className}, write new");
                            line = line.Insert(0, GetFullLeakDtorCall(className));
                            isFileModified = true;
                            
                        }
                        
                        if (!isCtorWritten)
                        {
                            Debug.Log($"No CTOR in {className}, write new");
                            line = line.Insert(0, GetFullLeakCtorCall(className));
                            isFileModified = true;
                            
                        }

                        isDtorWritten = false;
                        isCtorWritten = false;
                        
                        Debug.Log($"Class processed: {className}");
                    }

                    WRITE:
                        if (firstLineInClass)
                        {
                            firstLineInClass = false;
                        }
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

    private static string GetLeakCtorCall()
    {
        return "    LeakWatcher.Instance.Ctor(this);\n";
    }
    
    private static string GetLeakDtorCall()
    {
        return "    LeakWatcher.Instance.Dtor(this);\n";
    }
    
    private static string GetFullLeakCtorCall(string className)
    {
        return $"\n    public {className}() {{\n        LeakWatcher.Instance.Ctor(this);\n}}\n";
    }
    
    private static string GetFullLeakDtorCall(string className)
    {
        return $"\n    ~{className}() {{\n        LeakWatcher.Instance.Dtor(this);\n}}\n";
    }
    
    private static int getCountOfParentheses(string line)
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

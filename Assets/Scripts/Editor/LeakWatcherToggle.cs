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
        "/Editor",
        "/AutoGenerated/"
    };

    [MenuItem("Tools/Enable Leak Watcher")]
    public static void Process()
    {
        var files = Directory.GetFiles(Application.dataPath + "/Scripts", "*.cs", SearchOption.AllDirectories);

        int limit = 10;
        for (var i = 0; i < files.Length; i++)
        {
            if (--limit <= 0)
            {
                return;
            }
            
            string file = files[i];
            
            foreach (var ignoreDir in ignoreDirs)
            {
                if (file.Contains(ignoreDir))
                {
                    continue;
                }
            }
            
            Debug.Log($"=== Process file: [{i}/{files.Length}] {file} ===");
            
            ProcessFile(file);
        }
    }

    private static void ProcessFile(string file)
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
                        
                        line = line.Insert(pos, "@@@@DTOR@@@@");
                        needWriteDtorToNextLine = false;
                        isDtorWritten = true;
                        goto WRITE;
                    }
                    
                    if (needWriteCtorToNextLine)
                    {
                        int pos = line.IndexOf("{");
                        pos = Mathf.Max(0, pos);
                        
                        line = line.Insert(pos, "@@@@CTOR@@@@");
                        needWriteCtorToNextLine = false;
                        isCtorWritten = true;
                        goto WRITE;
                    }
                    
                    if (!isClassFound && line.Contains(" class ") && !line.Contains("static") && !line.Contains("abstract"))
                    {
                        isClassFound = true;
                        className = getClassNamaRegex.Match(line).Value;
                        
                        Debug.Log($"Class found: {className}");
                    }

                    if (isClassFound)
                    {
                        deltaParentheses += getCountOfParentheses(line);
                    }

                    // search for ~
                    if (isClassFound)
                    {
                        if (line.Contains("~") && line.Contains(className))
                        {
                            Debug.Log($"DTOR found: {line}");
                            
                            needWriteDtorToNextLine = true;
                            goto WRITE;
                        }
                        
                        if (line.Contains(className) && (line.Contains("public") || line.Contains("protected") || line.Contains("private")))
                        {
                            Debug.Log($"CTOR found: {line}");
                            
                            needWriteCtorToNextLine = true;
                            goto WRITE;
                        }
                    }

                    // Finalize class
                    if (isClassFound && deltaParentheses <= 0)
                    {
                        isClassFound = false;

                        if (!isDtorWritten)
                        {
                            Debug.Log($"No DTOR in {className}, write new");
                            line = line.Insert(0, "@@@@FULL_DTOR@@@@");
                        }
                        
                        if (!isCtorWritten)
                        {
                            Debug.Log($"No CTOR in {className}, write new");
                            line = line.Insert(0, "@@@@FULL_CTOR@@@@");
                        }

                        isDtorWritten = false;
                        isCtorWritten = false;
                        
                        Debug.Log($"Class processed: {className}");
                    }

                    WRITE:
                        output.Append(line);
                }
            } while (line != null);
            
            File.WriteAllText(file, output.ToString(), new UTF8Encoding(false));
        }
    }
    
    private static int getCountOfParentheses(string line)
    {
        int open = line.Count(f => f == '(');
        int close = line.Count(f => f == ')');

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

using System;
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
    
    [MenuItem("Tools/Enable Leak Watcher")]
    public static void Process()
    {
        var files = Directory.GetFiles(Application.dataPath, "*.cs");

        int limit = 10;
        foreach (var file in files)
        {
            if (--limit < 0)
            {
                break;
            }
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
                            needWriteDtorToNextLine = true;
                            goto WRITE;
                        }
                        
                        if (line.Contains(className) && (line.Contains("public") || line.Contains("protected") || line.Contains("private")))
                        {
                            needWriteCtorToNextLine = true;
                            goto WRITE;
                        }
                    }

                    // Finalize class
                    if (deltaParentheses <= 0)
                    {
                        isClassFound = false;

                        if (!isDtorWritten)
                        {
                            line = line.Insert(0, "@@@@DTOR@@@@");
                        }
                        
                        if (!isCtorWritten)
                        {
                            line = line.Insert(0, "@@@@CTOR@@@@");
                        }

                        isDtorWritten = false;
                        isCtorWritten = false;
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

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

class LeakWatcherEntry
{
    public int Ctors;
    public int Dtors;
}

public class LeakWatcher
{
    private int m_globalCtors;
    private int m_globalDtors;

    private readonly Dictionary<System.Type, LeakWatcherEntry> m_data = new Dictionary<System.Type, LeakWatcherEntry>();

    private static LeakWatcher s_instance;

    public static LeakWatcher Instance
    {
        get { return s_instance ?? (s_instance = new LeakWatcher()); }
    }

    private LeakWatcher()
    {
        Ctor(this);
    }

    ~LeakWatcher()
    {
        Dtor(this);
    }

    public void Ctor(object obj)
    {
#if !UNITY_EDITOR
    return;
#endif

        System.Type t = obj.GetType();
        LeakWatcherEntry entry;
        if (m_data.TryGetValue(t, out entry))
        {
            entry.Ctors ++;
        }
        else
        {
            m_data.Add(t, new LeakWatcherEntry {Ctors = 1, Dtors = 0});
        }
        m_globalCtors++;
    }

    public void Dtor(object obj)
    {
#if !UNITY_EDITOR
    return;
#endif
        System.Type t = obj.GetType();
        LeakWatcherEntry entry;
        if (m_data.TryGetValue(t, out entry))
        {
            entry.Dtors++;
        }
        else
        {
            m_data.Add(t, new LeakWatcherEntry { Ctors = 0, Dtors = 1 });
        }
        m_globalDtors++;
    }

    public string DataAsString(bool skipZeroDelta)
    {
        var lines = new List<string>();

        var ret = new StringBuilder();
        ret.Append("----------------------------------------\n");
        ret.Append("      GAME OBJECTS MEMORY STATS         \n");
        ret.Append("----------------------------------------\n");

        foreach (var entry in m_data)
        {
            int delta = entry.Value.Ctors - entry.Value.Dtors;
            var line = String.Format("{0} ({1}/{2})", delta, entry.Value.Ctors, entry.Value.Dtors).PadRight(15);
            line += (entry.Key.ToString());
            if (delta > 0)
            {
                line = line.PadRight(60) + " [ WARNING ]\n";
                lines.Add(line);
            }
            else
            {
                if (!skipZeroDelta)
                {
                    line += "\n";
                    lines.Add(line);
                }
            }
        }

        lines.Sort();

        foreach (var line in lines)
        {
            ret.Append(line);
        }

        ret.Append("----------------------------------------\n");
        ret.Append(String.Format("CTORS: {0}, DTORS: {1}, DELTA: {2}\n", m_globalCtors, m_globalDtors, m_globalCtors - m_globalDtors));
        return ret.ToString();
    }
}

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

class LeakWatcherEntry
{
    public int Ctors;
    public int Dtors;

    public int Delta => Ctors - Dtors;
}

public class LeakWatcher
{
    private int m_globalCtors;
    private int m_globalDtors;

    private readonly Dictionary<System.Type, LeakWatcherEntry> m_data = new Dictionary<System.Type, LeakWatcherEntry>();

    private static LeakWatcher s_instance;

    private Dictionary<Type, int> m_snapshot;
    
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

    public string DataAsString(bool onlyWarnings)
    {
        var lines = new List<string>();

        var ret = new StringBuilder();
        ret.Append("----------------------------------------\n");
        ret.Append("      GAME OBJECTS MEMORY STATS         \n");
        ret.Append("----------------------------------------\n");

        foreach (var entry in m_data)
        {
            int delta = entry.Value.Delta;
            var line = $"{delta} ({entry.Value.Ctors}/{entry.Value.Dtors})".PadRight(15);
            line += (entry.Key.ToString());
            if (delta > 0 && !onlyWarnings)
            {
                line = line.PadRight(60) + " [ WARNING ]\n";
            }
            else
            {
                line += "\n";
            }

            if (!onlyWarnings || delta > 1)
            {
                lines.Add(line);
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

    private Dictionary<Type, int> TakeSnapshot()
    {
        Dictionary<Type, int> ret = new Dictionary<Type, int>();
        foreach (var item in m_data)
        {
            ret.Add(item.Key, item.Value.Delta);
        }

        return ret;
    }

    public void Snapshot()
    {
        m_snapshot = TakeSnapshot();
    }

    public string CompareSnapshots()
    {
        var newSnapshot = TakeSnapshot();

        StringBuilder ret = new StringBuilder();
        ret.Append("----------------------------------------\n");
        ret.Append("                    DIFF                \n");
        ret.Append("----------------------------------------\n");

        bool anyChanges = false;
        
        foreach (KeyValuePair<Type, int> pair in newSnapshot)
        {
            Type type = pair.Key;
            int newDelta = pair.Value;

            m_snapshot.TryGetValue(type, out int oldDelta);

            int diff = newDelta - oldDelta;
            if (diff == 0)
            {
                continue;
            }

            anyChanges = true;
            
            var line = $"{diff} ({oldDelta} => {newDelta})".PadRight(15);
            line = line + type.ToString();
            ret.AppendLine(line);
        }
        
        ret.Append("----------------------------------------\n");
        
        m_snapshot = newSnapshot;

        if (!anyChanges)
        {
            ret.Clear();
            ret.Append("No changes!");
        }
        
        return ret.ToString();
    }
}

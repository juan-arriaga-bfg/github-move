using System;
using UnityEditor;
using UnityEngine;

namespace Dws
{
    /// <summary>
    /// Allow to call an Action from any thread within Unity Editor thread
    /// </summary>
    public class EditorMainThreadSync
    {
        private readonly Action m_action;

        public static void Execute(Action action)
        {
            // ReSharper disable once ObjectCreationAsStatement
            new EditorMainThreadSync(action);
        }

        private EditorMainThreadSync(Action action)
        {
            m_action = action;
            EditorApplication.update += Update;
        }

        private void Update()
        {
            EditorApplication.update -= Update;
            m_action();
        }
    }
}
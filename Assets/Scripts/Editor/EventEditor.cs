using UnityEditor;
using UnityEngine;

namespace Bonkers.Events
{
    [CustomEditor(typeof(LegacyGameEvent), editorForChildClasses: true)]
    public class EventEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            LegacyGameEvent e = target as LegacyGameEvent;
            if (GUILayout.Button("Raise"))
                e.Raise();
        }
    }
}
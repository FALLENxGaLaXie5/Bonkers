using UnityEditor;
using UnityEngine;

namespace Bonkers.Events
{
    [CustomEditor(typeof(LegacyGameEventWithVector3), editorForChildClasses: true)]
    public class EventEditorVector3 : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            LegacyGameEventWithVector3 e = target as LegacyGameEventWithVector3;
            if (GUILayout.Button("Raise"))
                e.Raise(Vector3.zero);
        }
    }
}
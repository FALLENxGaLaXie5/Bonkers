using UnityEditor;
using UnityEngine;

namespace Bonkers.Events
{
    [CustomEditor(typeof(GameEventWithVector3), editorForChildClasses: true)]
    public class EventEditorVector3 : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUI.enabled = Application.isPlaying;

            GameEventWithVector3 e = target as GameEventWithVector3;
            if (GUILayout.Button("Raise"))
                e.Raise(Vector3.zero);
        }
    }
}
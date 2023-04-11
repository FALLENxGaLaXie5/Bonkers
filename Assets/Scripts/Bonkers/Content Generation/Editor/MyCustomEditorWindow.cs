#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;

namespace Bonkers.Content_Generation.Editor
{
    public class MyCustomEditorWindow : OdinEditorWindow
    {
        [MenuItem("Bonkers/My Custom Editor")]
        private static void OpenWindow()
        {
            GetWindow<MyCustomEditorWindow>().Show();
        }

        [EnumToggleButtons, BoxGroup("Settings")]
        public ScaleMode ScaleMode;
        
        [FolderPath(RequireExistingPath = true), BoxGroup("Settings")]
        public string OutputPath;
        
        [HorizontalGroup(0.5f)]
        public List<Texture> InputTextures;
        
        [HorizontalGroup, InlineEditor(InlineEditorModes.LargePreview)]
        public Texture Preview;

        [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0)]
        public void PerformSomeAction()
        {

        }
        
        protected override void Initialize()
        {
            WindowPadding = Vector4.zero;
        }

        protected override object GetTarget()
        {
            return Selection.activeObject;
        }
    }
}
#endif
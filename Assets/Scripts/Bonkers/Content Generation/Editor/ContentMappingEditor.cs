using RoboRyanTron.Unite2017.Variables;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Bonkers.ContentGeneration.Editor
{
    public class ContentMappingEditor : OdinMenuEditorWindow
    {
        [SerializeField] private StringVariable contentMappingsPath;
        [SerializeField] private GeneratedDataSystem generatedDataSystem;
        
        [MenuItem("Content/Content Mapping Editor")]
        private static void OpenWindow() => GetWindow<ContentMappingEditor>().Show();

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();

            tree.AddAllAssetsAtPath("Content Mappings", contentMappingsPath.Value, typeof(ContentMappingArrayEditorData));
            return tree;
        }

        protected override void OnBeginDrawEditors()
        {
            OdinMenuTreeSelection selected = MenuTree.Selection;

            SirenixEditorGUI.BeginHorizontalToolbar();
            {
                GUILayout.FlexibleSpace();

                if (SirenixEditorGUI.ToolbarButton("Delete Current Mapping"))
                {
                    GeneratedContentData asset = (selected.SelectedValue as ContentMappingArrayEditorData).GeneratedContentData;
                    generatedDataSystem.RemoveContentData(asset);
                }
                if (SirenixEditorGUI.ToolbarButton("Delete all Mappings"))
                {
                    generatedDataSystem.ClearAllContentData();
                }
                if (SirenixEditorGUI.ToolbarButton("Generate Level"))
                {
                    GeneratedContentData asset = (selected.SelectedValue as ContentMappingArrayEditorData).GeneratedContentData;
                    generatedDataSystem.GenerateNewLevelScene(asset);
                }
            }
            
            SirenixEditorGUI.EndHorizontalToolbar();
        }
    }
}
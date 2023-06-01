using Bonkers.BlokControl;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bonkers.Helpers.Editor
{
    /// <summary>
    /// This editor is meant to take the prefab of the bloks in a level, saved in the individual blok data
    /// scriptable objects for all broken prefabbed bloks, and reset all the data to their prefabs.
    ///
    /// This is due to the blok fragmenting system breaking those prefabs.
    /// </summary>
    public class BloksResetter : OdinEditorWindow
    {
        [MenuItem("Bonkers Custom Editors/Blok Re-Prefabber")]
        private static void OpenWindow()
        {
            GetWindow<BloksResetter>().Show();
        }

        [HorizontalGroup]
        [Button(ButtonSizes.Large), GUIColor(0, 1, 0)]
        public void SetBloksToPrefabValues()
        {
            Scene scene = SceneManager.GetActiveScene();
            //
            BlokDestroyIntoPoolHelper[] poolHelperObjects = FindObjectsOfType<BlokDestroyIntoPoolHelper>();
            foreach (var poolHelperObject in poolHelperObjects)
            {
                GameObject newBlokObject = PrefabUtility.InstantiatePrefab(poolHelperObject.PoolingData.Prefab) as GameObject;
                var oldBlokPosition = poolHelperObject.transform.position;
                newBlokObject.transform.position = new Vector3(oldBlokPosition.x, oldBlokPosition.y, 0);
                newBlokObject.transform.parent = poolHelperObject.transform.parent;

                
                //Blok fragment generation
                //bloksObject.GetComponent<Configuration>().ConfigureExplodableBloks();
                //bloksObject.GetComponent<BlokFragmentMaterialModificationHelper>().AssistMatchFragmentMaterials();
                
            }
            
        }
    }
}
using UnityEngine;

namespace ScriptableObjectDropdown
{
    [CreateAssetMenu(fileName = "Exit Game System", menuName = "Exit Game/Exit Game System", order = 0)]
    public class ExitGameSystem : ScriptableObject
    {
        public void ExitGame()
        {
            #if UNITY_STANDALONE
            Application.Quit();
            #endif
            
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #endif
        }
    }
}
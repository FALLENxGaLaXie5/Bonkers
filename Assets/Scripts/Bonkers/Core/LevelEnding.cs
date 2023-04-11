using Bonkers.SceneManagement;
using UnityEngine;

namespace Bonkers.Core
{
    /// <summary>
    /// Controls the level ending
    /// </summary>
    public class LevelEnding : MonoBehaviour
    {
        public void EndLevel() => FindObjectOfType<Portal>().StartTransition(0);
    }
}
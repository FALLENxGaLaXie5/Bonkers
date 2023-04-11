using UnityEngine;

namespace Bonkers.Combat
{
    public class WoodenBlokBonks : MonoBehaviour
    {
        #region Inspector/Public Variables

        [SerializeField] [Range(1, 5)] int numBonksToBreak = 2;

        #endregion

        #region Class Variables

        int numTimesBonked = 0;

        #endregion

        #region Class Functions

        public int NumTimesBonked => numTimesBonked;
        public void IncrementNumTimesBonked() => numTimesBonked++;
        public int NumberBonksToBreak => numBonksToBreak;
        public void ResetNumberTimesBonked() => numTimesBonked = 0;

        #endregion
    }
}
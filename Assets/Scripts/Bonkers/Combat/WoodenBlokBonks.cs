using System.Collections;
using System.Collections.Generic;
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
        public int GetNumTimesBonked()
        {
            return this.numTimesBonked;
        }

        public void IncrementNumTimesBonked()
        {
            this.numTimesBonked++;
        }

        public int GetNumBonksToBreak()
        {
            return this.numBonksToBreak;
        }
        #endregion
    }
}
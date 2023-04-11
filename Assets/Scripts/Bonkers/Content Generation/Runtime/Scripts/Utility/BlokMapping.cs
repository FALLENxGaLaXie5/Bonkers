#if UNITY_EDITOR
using System;
using Bonkers.BlokControl;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Bonkers.ContentGeneration
{
    [Serializable]
    public class BlokMapping
    {
        [SerializeField][ReadOnly] private Vector2Int position;
        
        [SerializeField][ReadOnly] private IndividualBlokPoolingData blokType;

        //private bool overrideAsSpawnerBlok = false;
        public event Action<BlokMapping> OnChangeToSpawnerBlok;

        #region Properties

        public Vector2Int Value => position;
        public IndividualBlokPoolingData BlokType => blokType;
        //public bool OverrideAsSpawnerBlok => overrideAsSpawnerBlok;

        #endregion
        
        public BlokMapping(Vector2Int position, IndividualBlokPoolingData blokType)
        {
            this.position = position;
            this.blokType = blokType;
        }

        /// <summary>
        /// Set up a new position
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(Vector2Int position) => this.position = new Vector2Int(position.x, position.y);

        /// <summary>
        /// Sets new blok type
        /// </summary>
        /// <param name="blokType"></param>
        public void SetType(IndividualBlokPoolingData blokType) => this.blokType = blokType;
        
        /// <summary>
        /// Will override the type of blok as a spawning blok
        /// </summary>
        //public void SetAsSpawningBlokOverride() => overrideAsSpawnerBlok = true;
        
        public void InvokeOnChangeToSpawnerBlok() => OnChangeToSpawnerBlok?.Invoke(this);
    }
}
#endif
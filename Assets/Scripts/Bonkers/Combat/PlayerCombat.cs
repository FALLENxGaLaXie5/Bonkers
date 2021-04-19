using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Movement;
using Bonkers.BlokControl;
using System;

namespace Bonkers.Combat
{
    [DisallowMultipleComponent]
    public class PlayerCombat : MonoBehaviour
    {
        #region Inspector Variables
        public LayerMask bonkableLayers;
        [SerializeField] [Range(0.02f, 0.4f)] float checkRadius = 0.1f;
        [SerializeField] [Range(1.0f, 2.0f)] float bonkableDistance = 1.2f;
        #endregion

        #region Class Variables

        PlayerMovement playerMovement;
        #endregion

        #region Unity Event Functions
        // Start is called before the first frame update
        void Start()
        {
            playerMovement = GetComponent<PlayerMovement>();
        }

        public void BonkBlok()
        {
            Collider2D blokCollider = Physics2D.OverlapCircle(transform.position + playerMovement.GetFacingDir(), 0.2f, bonkableLayers);

            //if no hit, return
            if (!blokCollider) return;
            //if not in range of blok that was hit, return
            if (Vector3.Distance(transform.position, blokCollider.transform.position) > bonkableDistance) return;


            switch (blokCollider.transform.tag)
            {
                case "BasicBlok":
                    BonkBasicBlok(blokCollider);
                    break;
                case "GlassBlok":
                    BonkGlassBlok(blokCollider);
                    break;
                case "WoodenBlok":
                    BonkWoodenBlok(blokCollider);
                    break;
                case "IceBlok":
                    BonkIceBlok(blokCollider);
                    break;
                case "BombBlok":
                    BonkBombBlok(blokCollider);
                    break;
                case "SkullBlok":
                    break;
                default:
                    break;
            }
        }

        


        #endregion

        #region Class Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blokCollider"></param>
        void BonkBasicBlok(Collider2D blokCollider)
        {
            Collider2D nextOverBlokCollider = Physics2D.OverlapCircle(transform.position + (playerMovement.GetFacingDir() * 2), checkRadius, bonkableLayers);
            if (nextOverBlokCollider)
            {
                blokCollider.transform.GetComponent<IBlokControl>().PlaySound();
                Transform audioSourceTransform = blokCollider.transform.GetComponentInChildren<AudioSource>().transform;
                audioSourceTransform.parent = null;
                Destroy(audioSourceTransform.gameObject, 1f);
                blokCollider.transform.GetComponent<ExplodeOnOrder>().ExplodeBlok();
            }
            else
            {
                blokCollider.transform.GetComponent<IBlokControl>().SetMoving(true, playerMovement.GetFacingDir());
            }
            UpdateHitBy(blokCollider);
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blokCollider"></param>
        void BonkGlassBlok(Collider2D blokCollider)
        {
            Collider2D nextOverBlokCollider = Physics2D.OverlapCircle(transform.position + (playerMovement.GetFacingDir() * 2), checkRadius, bonkableLayers);
            if (nextOverBlokCollider)
            {
                blokCollider.transform.GetComponent<BlokHealth>().DestroyBlok();
            }
            else
            {
                blokCollider.transform.GetComponent<IBlokControl>().SetMoving(true, playerMovement.GetFacingDir());
            }
            UpdateHitBy(blokCollider);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blokCollider"></param>
        void BonkWoodenBlok(Collider2D blokCollider)
        {
            WoodenBlokBonks bonksInstance = blokCollider.transform.GetComponent<WoodenBlokBonks>();
            bonksInstance.IncrementNumTimesBonked();
            if (bonksInstance.GetNumTimesBonked() >= bonksInstance.GetNumBonksToBreak())
            {
                blokCollider.transform.GetComponent<WoodBlokHealth>().DestroyBlok();
            }
            UpdateHitBy(blokCollider);
        }

        /// <summary>
        /// Bonk Bomb Blok - just set moving, and if it is right adjacent to another blok, will automatically detect that and explode
        /// </summary>
        /// <param name="blokCollider"></param>
        void BonkBombBlok(Collider2D blokCollider)
        {
            blokCollider.transform.GetComponent<IBlokControl>().SetMoving(true, playerMovement.GetFacingDir());
            UpdateHitBy(blokCollider);
        }

        void BonkIceBlok(Collider2D blokCollider)
        {
            Collider2D nextOverBlokCollider = Physics2D.OverlapCircle(transform.position + (playerMovement.GetFacingDir() * 2), checkRadius, bonkableLayers);
            if (nextOverBlokCollider)
            {
                blokCollider.transform.GetComponent<BlokHealth>().DestroyBlok();                
            }
            else
            {
                blokCollider.transform.GetComponent<IBlokControl>().SetMoving(true, playerMovement.GetFacingDir());
            }
            UpdateHitBy(blokCollider);
        }

        private void UpdateHitBy(Collider2D blokCollider)
        {
            blokCollider.transform.GetComponent<HitBy>().entityHitBy = this.transform;
        }

        #endregion
    }
}


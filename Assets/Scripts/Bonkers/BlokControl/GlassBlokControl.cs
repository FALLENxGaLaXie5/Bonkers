using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Combat;
using Bonkers.Score;

namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    public class GlassBlokControl : BlokControl
    {
        #region Unity Events/Functions

        protected override void OnEnable()
        {
            base.OnEnable();
            blokInteraction.onBlokHit += Destroy;
            blokInteraction.onSetMoving += SetMoving;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            blokInteraction.onBlokHit -= Destroy;
            blokInteraction.onSetMoving -= SetMoving;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isMoving) return;
            
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, movePoint.position) <= .05f)
            {
                Collider2D blokCollider = Physics2D.OverlapCircle(transform.position + moveDir, 0.2f, wallBonkMask);
                if (!blokCollider)
                {
                    movePoint.position += moveDir;
                }
                else
                {
                    SetMoving(false, Vector3.zero);
                    PlaySound();
                    //explode!!
                    GetComponent<ExplodeOnOrder>().ExplodeBlok();
                }
            }
        }

        #endregion

        #region Class Functions

        protected override void PlaySound()
        {
            base.PlaySound();
            Transform audioSourceTransform = hitSound.transform;
            audioSourceTransform.parent = null;
            Destroy(audioSourceTransform.gameObject, 1f);
        }

        void Destroy()
        {
            GetComponent<BlokHealth>().DestroyBlok();
        }

        #endregion
    }
}
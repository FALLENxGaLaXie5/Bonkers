﻿using UnityEngine;

namespace Bonkers.BlokControl
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(IBlokInteraction))]
    public class GlassBlokControl : MoveableBlokControl
    {
        #region Unity Events/Functions

        protected override void OnEnable()
        {
            base.OnEnable();
            blokInteraction.OnBlokHit += Destroy;
            blokInteraction.OnSetMoving += SetMoving;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            blokInteraction.OnBlokHit -= Destroy;
            blokInteraction.OnSetMoving -= SetMoving;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isMoving) return;
            transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, movePoint.position) > .05f) return;
            
            Collider2D blokCollider = Physics2D.OverlapCircle(transform.position + moveDir, 0.2f, wallBonkMask);
            if (!blokCollider)
            {
                movePoint.position += moveDir;
            }
            else
            {
                blokInteraction.TriggerBlokImpact(false);
                //explode!!
                Destroy();
            }
            
        }

        #endregion

        #region Class Functions

        void Destroy() => blokHealth.BreakBlok();

        #endregion
    }
}
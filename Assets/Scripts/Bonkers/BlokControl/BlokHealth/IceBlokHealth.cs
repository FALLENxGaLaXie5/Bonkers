using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Drops;

namespace Bonkers.BlokControl
{
    public class IceBlokHealth : BlokHealth
    {
        new void Start()
        {
            base.Start();
        }

        public override void DestroyBlok()
        {
            blokControl.PlaySound();
            Transform audioSourceTransform = transform.GetComponentInChildren<AudioSource>().transform;
            audioSourceTransform.parent = null;
            Destroy(audioSourceTransform.gameObject, 1f);
            explosionOrder.ExplodeBlok();
        }
    }

}

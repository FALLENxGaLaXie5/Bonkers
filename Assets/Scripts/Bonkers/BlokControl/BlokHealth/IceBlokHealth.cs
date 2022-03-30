using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.BlokControl
{
    public class IceBlokHealth : BlokHealth
    {
        public override void DestroyBlok()
        {
            Transform audioSourceTransform = transform.GetComponentInChildren<AudioSource>().transform;
            audioSourceTransform.parent = null;
            Destroy(audioSourceTransform.gameObject, 1f);
            explosionOrder.ExplodeBlok();
        }
    }

}

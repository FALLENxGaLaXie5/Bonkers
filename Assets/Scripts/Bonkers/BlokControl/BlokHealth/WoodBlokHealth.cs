using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bonkers.Drops;

namespace Bonkers.BlokControl
{
    public class WoodBlokHealth : BlokHealth
    {
        BlokDroppable droppable;
        new void Start()
        {
            base.Start();
            droppable = GetComponent<BlokDroppable>();
        }

        new public void DestroyBlok()
        {
            base.DestroyBlok();
            //will tell the droppable script to spawn one of the drops it can spawn
            droppable.SpawnDrop();
        }
    }

}

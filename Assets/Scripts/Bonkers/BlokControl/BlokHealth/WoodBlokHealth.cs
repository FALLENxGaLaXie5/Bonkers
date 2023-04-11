using Bonkers.Drops;

namespace Bonkers.BlokControl
{
    public class WoodBlokHealth : BlokHealth
    {
        BlokDroppable droppable;

        protected override void Awake()
        {
            base.Awake();
            droppable = GetComponent<BlokDroppable>();
        }

        public override void BreakBlok()
        {
            //will tell the droppable script to spawn one of the drops it can spawn
            //Need to do this before destroying the blok, since the base destroy will move the blok outside the boundaries and pool it
            droppable.SpawnDrop();
            
            base.BreakBlok();
        }
    }
}

using UnityEngine;

namespace Bonkers.BlokControl
{
    public class BombBlokHealth : BlokHealth
    {
        [SerializeField] private BombBlokExplosionObjectControl explosionObjectControl;
        protected override void OnEnable()
        {
            base.OnEnable();
            OnRespawnBlok += explosionObjectControl.ResetExplosionObjectController;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            OnRespawnBlok -= explosionObjectControl.ResetExplosionObjectController;
        }
    }
}
using Bonkers._2DDestruction;
using UnityEngine;

namespace Bonkers.Core
{
    public class BloksConfigurationControl : MonoBehaviour
    {
        /// <summary>
        /// Component that is meant to be called to assist in configuring blok fragmentss
        /// </summary>
        public void ConfigureBloks()
        {
            Configuration bloksConfigurer = transform.GetComponentInChildren<Configuration>();
            BlokFragmentMaterialModificationHelper fragmentMaterialModificationHelper =
                transform.GetComponentInChildren<BlokFragmentMaterialModificationHelper>();

            if (!bloksConfigurer) return;
            if (!fragmentMaterialModificationHelper) return;
            
            bloksConfigurer.ConfigureExplodableBloks();
            fragmentMaterialModificationHelper.AssistMatchFragmentMaterials();
        }
    }
}
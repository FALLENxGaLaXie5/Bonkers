using System;
using UnityEngine;

namespace  Bonkers.ItemDrops
{
    public class PlayerEnvironmentEffectorGrabber : MonoBehaviour
    {
        public event Action<ScriptableObject> onEnterEnvironmentEffector;
        public event Action<ScriptableObject> onExitEnvironmentEffector; 

        public void AttemptApplyEffector(IEnvironmentEffector effector)
        {
            ScriptableObject environmentEffectorObject = effector.AttemptGetEffector();
            
            //if an environment effector was successfully returned, notify listeners and give them the effector scriptable object
            if (environmentEffectorObject)
            {
                onEnterEnvironmentEffector?.Invoke(environmentEffectorObject);
            }
        }

        public void AttemptUnapplyEffector(IEnvironmentEffector effector)
        {
            ScriptableObject environmentEffectorObject = effector.AttemptGetEffector();
            
            //if an environment effector was successfully returned, notify listeners and give them the effector scriptable object
            if (environmentEffectorObject) onExitEnvironmentEffector?.Invoke(environmentEffectorObject);
        }
    }
}

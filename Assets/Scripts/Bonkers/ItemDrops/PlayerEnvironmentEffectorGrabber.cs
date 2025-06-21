using System;
using UnityEngine;

namespace  Bonkers.ItemDrops
{
    public class PlayerEnvironmentEffectorGrabber : MonoBehaviour
    {
        public event Action<ScriptableObject> OnEnterEnvironmentEffector;
        public event Action<ScriptableObject> OnExitEnvironmentEffector;

        public void AttemptApplyEffector(IEnvironmentEffector effector)
        {
            ScriptableObject environmentEffectorObject = effector.AttemptGetEffector();
            
            //if an environment effector was successfully returned, notify listeners and give them the effector scriptable object
            if (environmentEffectorObject)
            {
                OnEnterEnvironmentEffector?.Invoke(environmentEffectorObject);
            }
        }

        public void AttemptUnapplyEffector(IEnvironmentEffector effector)
        {
            ScriptableObject environmentEffectorObject = effector.AttemptGetEffector();
            
            //if an environment effector was successfully returned, notify listeners and give them the effector scriptable object
            if (environmentEffectorObject) OnExitEnvironmentEffector?.Invoke(environmentEffectorObject);
        }
    }
}

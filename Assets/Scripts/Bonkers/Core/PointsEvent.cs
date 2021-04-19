using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Core
{
    public class PointsEvent : MonoBehaviour
    {
        //animation event
        public void Destroy()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}

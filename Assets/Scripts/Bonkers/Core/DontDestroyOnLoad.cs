using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.Core
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

}
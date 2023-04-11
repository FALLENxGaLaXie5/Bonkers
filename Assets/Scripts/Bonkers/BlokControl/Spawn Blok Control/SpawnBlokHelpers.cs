using System.Collections.Generic;
using UnityEngine;

namespace Bonkers.BlokControl
{
    public static class SpawnBlokHelpers
    {
        /// <summary>
        /// This will compare a vector3 to a list of vector3's and output the one that's closest
        /// </summary>
        /// <param name="vectorToCompare"></param>
        /// <param name="listToCompare"></param>
        /// <returns></returns>
        public static Vector3 FindClosestVector(Vector3 vectorToCompare, List<Vector3> listToCompare)
        {
            Vector3 closestVector3 = listToCompare[0];
            foreach (Vector3 vector in listToCompare)
            {
                if (Vector3.Distance(vector, vectorToCompare) < Vector3.Distance(vector, closestVector3))
                {
                    closestVector3 = vector;
                }
            }
            return closestVector3;
        }
    }
}
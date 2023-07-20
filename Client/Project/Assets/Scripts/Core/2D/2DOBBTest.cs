using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TwoDimensional
{
    // OBBTest.cs
    public class OBBTest : MonoBehaviour
    {
        public OBB a;
        public OBB b;
        void Update()
        {
            var isIntersects = a.Intersects(b);
            if (isIntersects)
            {
                a.gizmosColor = Color.red;
                b.gizmosColor = Color.red;
            }
            else
            {
                a.gizmosColor = Color.white;
                b.gizmosColor = Color.white;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Collistion
{
    public class CollistionDemo : MonoBehaviour
    {
        public Collision.OBB A;
        public Collision.OBB B;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (A.Intersects(B))
            {
                Debug.Log("碰撞");
            }


        }

        private void OnTriggerEnter(Collider other)
        {
            
        }
    }
}


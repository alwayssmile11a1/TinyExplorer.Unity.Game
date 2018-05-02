using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Checkpoint : MonoBehaviour
    {
        public bool respawnFacingLeft;

        private void Reset()
        {
            GetComponent<BoxCollider2D>().isTrigger = true; 
        }

        protected void OnTriggerEnter2D(Collider2D collision)
        {
            AlessiaController alessia = collision.GetComponent<AlessiaController>();

            if (alessia != null)
            {
                alessia.SetChekpoint(this);
            }

        }
    }
}
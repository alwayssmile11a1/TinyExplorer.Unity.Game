using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit2D
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Checkpoint : MonoBehaviour
    {
        public bool respawnFacingLeft;

        public bool resetGameObjectsOnRespawn;
        
        [Tooltip("The gameobjects have to implement interface IDataResetable")]
        public GameObject[] resetGameObjects; 

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

        public void SetResetGameObjectOnRespawn(bool resetGameObjectsOnRespawn)
        {
            this.resetGameObjectsOnRespawn = resetGameObjectsOnRespawn;
        }


    }
}
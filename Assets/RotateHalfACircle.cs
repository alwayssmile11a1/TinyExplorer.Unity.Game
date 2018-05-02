using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateHalfACircle : MonoBehaviour {
    public float speed;
    [SerializeField]
    private bool canRotate = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        canRotate = true;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //transform.rotation = Quaternion.Euler(0, 0, -90);
        if (canRotate)
        {
            if (transform.rotation.z >= -1 && transform.rotation.z <= Quaternion.Euler(0, 0, -179).z)
            {
                canRotate = false;
            }
            else
            {
                transform.Rotate(Vector3.back * speed * Time.deltaTime);
            }
            //StartCoroutine(RotateObj(0.5f, new Vector3(0, 0, -180)));
        }

        //Debug.Log("rotation: " + transform.rotation.z);
    }
    IEnumerator RotateObj(float timeToRotate, Vector3 direction)
    {
        float t = 0;
        while (t < timeToRotate)
        {
            gameObject.transform.Rotate(direction * (Time.fixedDeltaTime / timeToRotate));
            t += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        canRotate = false;
        yield break;
    }
}

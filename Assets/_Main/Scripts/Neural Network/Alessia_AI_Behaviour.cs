using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Alessia_AI_Behaviour : MonoBehaviour {

    [HideInInspector] public float point;

    

    public float moveForce;
    public ForceMode forceMode;
    public float raycastDistance;
    [Tooltip("add a little distance to front ray")]
    public float additional;
    public bool off;

    private Rigidbody rigidbody;
    private Alessia_AI_DNA alessia_AI_DNA;
    private float oldFitness;
    private float newFitness;
    private bool moveThroughPitch;
    [HideInInspector] public float liveTime;
    [HideInInspector] public bool finish;
    [HideInInspector] public int hitHackingPoint;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        alessia_AI_DNA = GetComponent<Alessia_AI_DNA>();

        //TEST
        //carDNA.InitCar(5, 5, 2);
    }

    public float[] GetOutput()
    {
        float[] input = new float[7];
        RaycastHit hitInfo;
        float startAngle = -45;
        float amount = 15;
        Vector3 raycastPoint = transform.position + Vector3.up * 0.35f;
        for (int i = 0; i < input.Length; i++)
        {
            float angle = i * amount + startAngle;
            if (angle == 0)
            {
                Debug.DrawRay(raycastPoint, (Quaternion.Euler(0, 0, angle) * Vector3.right).normalized * (raycastDistance + additional), Color.green);
                if (Physics.Raycast(raycastPoint, (Quaternion.Euler(0, 0, angle) * Vector3.right).normalized, out hitInfo, (raycastDistance + additional), LayerMask.GetMask("Wall")))
                {
                    input[i] = hitInfo.distance;
                    Debug.DrawRay(raycastPoint, (Quaternion.Euler(0, i * startAngle, 0) * transform.right).normalized * (raycastDistance + additional), Color.red);
                }
                else
                {
                    input[i] = raycastDistance + additional;
                }
            }
            else
            {
                Debug.DrawRay(raycastPoint, (Quaternion.Euler(0, 0, angle) * Vector3.right).normalized * (raycastDistance), Color.green);
                if (Physics.Raycast(raycastPoint, (Quaternion.Euler(0, 0, angle) * Vector3.right).normalized, out hitInfo, (raycastDistance), LayerMask.GetMask("Wall")))
                {
                    input[i] = hitInfo.distance;
                    Debug.DrawRay(raycastPoint, (Quaternion.Euler(0, 0, angle) * Vector3.right).normalized * (raycastDistance), Color.red);
                }
                else
                {
                    input[i] = raycastDistance;
                }
            }
        }
        return alessia_AI_DNA.neuralNetwork.FeedForward(input);
    }

    public float[] GetAxisFromOutput(float[] output)
    {
        float vertical;
        float horizontal;
        if (output[1] <= 0.25f)
        {
            horizontal = -1;
        }
        else if (output[0] >= 0.65f)
        {
            horizontal = 1;
        }
        else
        {
            horizontal = 0;
        }

        if (output[1] <= 0.25)
        {
            vertical = -1;
        }
        else if (output[0] >= 0.65)
        {
            vertical = 1;
        }
        else
        {
            vertical = 0;
        }

        // If the output is just standing still, then move the car forward
        if (vertical == 0 && horizontal == 0)
            vertical = 1;
        //Debug.Log($"{output[0]} {output[1]} {vertical} {horizontal}");
        return new float[] { horizontal, vertical };
    }

    public void RunAlessiaAI(float[] axis)
    {
        rigidbody.angularVelocity = transform.up * axis[0] * 3;
        rigidbody.velocity = (transform.forward * axis[1] * 4);
    }

    public void RestartAlessiaAI(Vector3 spawnPos, Quaternion spawnRotation)
    {
        gameObject.SetActive(true);
        off = false;
        moveThroughPitch = false;
        transform.position = spawnPos;
        transform.rotation = spawnRotation;
        oldFitness = newFitness = 0;
        liveTime = 0;
        hitHackingPoint = 0;
        alessia_AI_DNA.score = 0;
    }

    public void ShutDownAlessiaAI()
    {
        if ((liveTime > 4 && !moveThroughPitch) || liveTime > 60 * 3.5f)
        {
            off = true;
            gameObject.SetActive(false);
        }
    }

    public void UpdateLiveTime()
    {
        liveTime += Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.tag.Equals("Wall"))
        //{
        //    off = true;
        //    gameObject.SetActive(false);
        //}
        if (collision.gameObject.tag.Equals("CollectableGem"))
        {
            alessia_AI_DNA.score++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Pitch"))
        {
            moveThroughPitch = true;
        }
        //else if (other.gameObject.tag.Equals("Goal"))
        //{
        //    File.WriteAllBytes("Assets/Training_Result/result.txt", alessia_AI_DNA.neuralNetwork.ToByteArray());
        //    //finish = true;
        //    off = true;
        //    gameObject.SetActive(false);
        //    hitHackingPoint++;
        //}
        //else if (other.gameObject.tag.Equals("HackingPoint"))
        //{
        //    hitHackingPoint++;
        //}
    }
}

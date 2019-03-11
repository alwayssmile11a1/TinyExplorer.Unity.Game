using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class Alessia_AI_Behaviour : MonoBehaviour {

    [HideInInspector] public float point;

    

    public float moveForce;
    public ForceMode forceMode;
    public float raycastDistance;
    public LayerMask layerMask;
    [Tooltip("add a little distance to front ray")]
    public float additional;
    public bool off;

    private Rigidbody rigidbody;
    private Alessia_AI_DNA alessia_AI_DNA;
    private AlessiaController alessiaController;
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
        alessiaController = GetComponent<AlessiaController>();

        //TEST
        //carDNA.InitCar(5, 5, 2);
    }

    public float[] GetOutput(int inputNodes)
    {
        float[] input = new float[inputNodes];
        RaycastHit hitInfo;
        float startAngle = -45;
        float amount = 15;
        Vector3 raycastPoint = transform.position + Vector3.up * 0.35f;
        for (int i = 0; i < input.Length; i++)
        {
            float angle = i * amount + startAngle;
            if (angle == 0)
            {
                if (Physics2D.Raycast(raycastPoint, (Quaternion.Euler(0, 0, angle) * Vector3.right).normalized, (raycastDistance + additional), layerMask) is var hit)
                {
                    input[i] = hit.distance == 0 ? raycastDistance + additional : hit.distance;
                    Debug.DrawRay(raycastPoint, (Quaternion.Euler(0, 0, angle) * transform.right).normalized * (hit.distance == 0 ? raycastDistance + additional : hit.distance), hit.distance == 0 ? Color.green : Color.red);
                }
            }
            else
            {
                if (Physics2D.Raycast(raycastPoint, (Quaternion.Euler(0, 0, angle) * Vector3.right).normalized, (raycastDistance), layerMask) is var hit)
                {
                    input[i] = hit.distance == 0 ? raycastDistance : hit.distance;
                    Debug.DrawRay(raycastPoint, (Quaternion.Euler(0, 0, angle) * Vector3.right).normalized * (hit.distance == 0 ? raycastDistance : hit.distance), hit.distance == 0 ? Color.green : Color.red);
                }
            }
        }
        return alessia_AI_DNA.neuralNetwork.FeedForward(input);
    }

    public float[] GetActionFromOutput(float[] output)
    {
        float vertical;
        float horizontal;
        if (output[0] >= 0.5f)
        {
            horizontal = 1;
        }
        else
        {
            horizontal = -1;
        }

        if (output[1] >= 0.5)
        {
            vertical = 1;
        }
        else
        {
            vertical = 0;
        }

        // If the output is just standing still, then move the car forward
        if (vertical == 0 && horizontal == 0)
        {
            //vertical = 1;
        }
        //Debug.Log($"{output[0]} {output[1]} {vertical} {horizontal}");
        return new float[] { horizontal, vertical };
    }

    public void RunAlessiaAI(float[] action)
    {
        // Move AI in horizontal
        alessiaController.AlessiaAIMoveRight(action[0]);
        if (action[1] == 1) 
        {
            alessiaController.Jump();
        }
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
        if ((liveTime > 4 && !moveThroughPitch) || liveTime > 10 /*60 * 3.5f*/)
        {
            off = true;
            gameObject.SetActive(false);
        }
    }

    public void UpdateLiveTime()
    {
        liveTime += Time.fixedDeltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.tag.Equals("Wall"))
        //{
        //    off = true;
        //    gameObject.SetActive(false);
        //}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Pitch"))
        {
            moveThroughPitch = true;
        }
        else if (other.gameObject.tag.Equals("CollectableGem"))
        {
            alessia_AI_DNA.score++;
        }
        //else if (other.gameObject.tag.Equals("Goal"))
        //{
        //    File.WriteAllBytes("Assets/Training_Result/result.txt", alessia_AI_DNA.neuralNetwork.ToByteArray());
        //    //finish = true;
        //    off = true;
        //    gameObject.SetActive(false);
        //    hitHackingPoint++;
        //}
    }
}

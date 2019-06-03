using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

public class ZombieKnight_AI_Behaviour : MonoBehaviour
{

    [HideInInspector] public float point;

    public float moveForce;
    public ForceMode forceMode;
    public float raycastDistance;
    public LayerMask layerMask;
    [Tooltip("add a little distance to front ray")]
    public float additional;
    public bool off;
    public float maxLiveTime;

    private Rigidbody aiRigidbody;
    private ZombieKnight_AI_DNA ai_DNA;
    private ZombieKnightAIController aiController;
    private float oldFitness;
    private float newFitness;
    private bool moveThroughPitch;
    [HideInInspector] public float liveTime;
    [HideInInspector] public bool finish;
    [HideInInspector] public int hitHackingPoint;

    // Start is called before the first frame update
    void Start()
    {
        aiRigidbody = GetComponent<Rigidbody>();
        ai_DNA = GetComponent<ZombieKnight_AI_DNA>();
        aiController = GetComponent<ZombieKnightAIController>();

        //TEST
        //carDNA.InitCar(5, 5, 2);
    }

    public float[] GetOutput(int inputNodes)
    {
        float[] input = new float[inputNodes];
        //RaycastHit hitInfo;
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
        return ai_DNA.neuralNetwork.FeedForward(input);
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

    public void RunAI(float[] action)
    {
        // Move AI in horizontal
        aiController.MoveRight(action[0]);
        if (action[1] == 1)
        {
            aiController.Jump();
        }
    
    }

    public void RestartAI(Vector3 spawnPos, Quaternion spawnRotation)
    {
        gameObject.SetActive(true);
        off = false;
        moveThroughPitch = false;
        transform.position = spawnPos;
        transform.rotation = spawnRotation;
        oldFitness = newFitness = 0;
        liveTime = 0;
        hitHackingPoint = 0;
        ai_DNA.score = 0;
    }

    public void ShutDownAI()
    {
        if ((liveTime > 4 && !moveThroughPitch) || liveTime > maxLiveTime /*60 * 3.5f*/)
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
        if (other.gameObject.CompareTag("Pitch"))
        {
            moveThroughPitch = true;
        }
        else if (other.gameObject.CompareTag("CollectableGem"))
        {
            ai_DNA.score++;
        }
        else if (other.gameObject.CompareTag("DeadLine"))
        {
            off = true;
            gameObject.SetActive(false);
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

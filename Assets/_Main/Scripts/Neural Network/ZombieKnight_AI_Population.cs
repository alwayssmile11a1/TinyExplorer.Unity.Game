using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;

public class ZombieKnight_AI_Population : MonoBehaviour
{

    public Transform aiParent;
    public GameObject aiPrefab;
    public GameObject bestAIPrefab;
    public int AI_Amount;
    //public float carMaxSpeed;

    [HideInInspector] public float mutationRate;                          // Mutation rate
    [HideInInspector] public List<GameObject> aiList;                // Array to hold the current population
    //[HideInInspector] public string target;                               // Target phrase
    [HideInInspector] public int generations;                             // Number of generations
    [HideInInspector] public bool finished;                               // Are we finished evolving?

    private Vector3 spawnPosition;
    private float maxFitness;
    private int perfectScore;
    private int inputNodes;
    private int hiddenNodes;
    private int outputNodes;
    private int bestAlessiaAI;
    private bool trainingMode;
    Vector3 SmoothPosVelocity = Vector3.zero; // Velocity of Position Smoothing

    public void InitPopulation(float mutation, int inputNodes, int hiddenNodes, int outputNodes, Vector3 spawnPos, bool trainingMode)
    {
        this.trainingMode = trainingMode;
        if (trainingMode)
        {
            //spawnPosition = carPrefab.transform.position;
            spawnPosition = spawnPos;
            //this.target = target;
            mutationRate = mutation;
            finished = false;
            generations = 1;
            perfectScore = 1;
            this.inputNodes = inputNodes;
            this.hiddenNodes = hiddenNodes;
            this.outputNodes = outputNodes;

            aiList = new List<GameObject>();
            for (int i = 0; i < AI_Amount; i++)
            {
                aiList.Add(Instantiate(aiPrefab, spawnPosition, aiPrefab.transform.rotation, aiParent));
                aiList[i].GetComponent<ZombieKnight_AI_DNA>().InitDNA(inputNodes, hiddenNodes, outputNodes);
                aiList[i].name = $"AI {i + 1}";
            }
            ReadBestTrainedData("Assets/Training_Result/data2.txt", ref aiList[0].GetComponent<ZombieKnight_AI_DNA>().neuralNetwork);
        }
        else
        {
            aiList.Add(Instantiate(bestAIPrefab, bestAIPrefab.transform.position, bestAIPrefab.transform.rotation));
            aiList[0].GetComponent<ZombieKnight_AI_DNA>().InitDNA(inputNodes, hiddenNodes, outputNodes);
        }
    }

    public void CalculateFitness()
    {
        for (int i = 0; i < aiList.Count; i++)
        {
            aiList[i].GetComponent<ZombieKnight_AI_DNA>().CalculateFitness();

        }
    }

    // Generate a mating pool
    public void NaturalSelection()
    {
        float totalFitness = 0;
        for (int i = 0; i < aiList.Count; i++)
        {
            totalFitness += aiList[i].GetComponent<ZombieKnight_AI_DNA>().fitness;
        }
        for (int i = 0; i < aiList.Count; i++)
        {
            aiList[i].GetComponent<ZombieKnight_AI_DNA>().probability = (double)aiList[i].GetComponent<ZombieKnight_AI_DNA>().fitness / (double)totalFitness;
        }
    }

    // Create a new generation
    public void Generate()
    {
        NeuralNetwork[] temp = new NeuralNetwork[aiList.Count];
        // Refill the population with children from the mating pool
        for (int i = 0; i < aiList.Count - 1; i++)
        {
            if (i == bestAlessiaAI)
            {
                temp[i] = aiList[i].GetComponent<ZombieKnight_AI_DNA>().neuralNetwork;
            }
            else
            {
                ZombieKnight_AI_DNA partnerA = PickOne(aiList);
                ZombieKnight_AI_DNA partnerB = PickOne(aiList);
                NeuralNetwork child = partnerA.CrossOver(partnerB);
                Mutate(ref child, mutationRate);
                temp[i] = child;
            }
        }
        for (int i = 0; i < aiList.Count - 1; i++)
        {
            aiList[i].GetComponent<ZombieKnight_AI_DNA>().neuralNetwork = temp[i];
        }
        aiList[aiList.Count - 1].GetComponent<ZombieKnight_AI_DNA>().neuralNetwork = new NeuralNetwork(inputNodes, hiddenNodes, outputNodes);
        //if (matingPool.Count != 0)
        {
            generations++;
        }
        RestartAIs(spawnPosition);
    }

    // Compute the current "most fit" member of the population
    public string Evaluate()
    {
        float worldrecord = 0.0f;
        for (int i = 0; i < aiList.Count; i++)
        {
            //Console.WriteLine(dnas[i].fitness);
            if (aiList[i].GetComponent<ZombieKnight_AI_DNA>().fitness > worldrecord)
            {
                worldrecord = aiList[i].GetComponent<ZombieKnight_AI_DNA>().fitness;
                bestAlessiaAI = i;
            }
            if (aiList[i].GetComponent<ZombieKnight_AI_Behaviour>().finish)
            {
                finished = true;
            }
        }
        File.WriteAllBytes("Assets/Training_Result/bestAlessiaAI.txt", aiList[bestAlessiaAI].GetComponent<ZombieKnight_AI_DNA>().neuralNetwork.ToByteArray());
        //if (worldrecord == perfectScore) finished = true;
        return $"{generations}";
    }

    // Based on a mutation probability, picks a new random character
    public void Mutate(ref NeuralNetwork newNeural, float mutationRate)
    {
        DoMutate(ref newNeural.ihWeights, mutationRate);
        DoMutate(ref newNeural.hoWeights, mutationRate);
        DoMutate(ref newNeural.biasH, mutationRate);
        DoMutate(ref newNeural.biasO, mutationRate);
    }

    private void DoMutate(ref Matrix m, float mutaionRate)
    {
        for (int i = 0; i < m.rowNb; i++)
        {
            for (int j = 0; j < m.columnNb; j++)
            {
                if (Random.Range(0f, 1f) < mutaionRate)
                {
                    m[i][j] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    public bool IsFinish()
    {
        return finished;
    }

    public bool AllOff()
    {
        foreach (var ai in aiList)
        {
            if (!ai.GetComponent<ZombieKnight_AI_Behaviour>().off)
            {
                return false;
            }
        }
        return true;
    }

    public void RestartAIs(Vector3 spawnPos)
    {
        foreach (var ai in aiList)
        {
            ai.GetComponent<ZombieKnight_AI_Behaviour>().RestartAI(spawnPos, aiPrefab.transform.rotation);
        }
    }

    public void RunAIs()
    {
        float[] output = new float[outputNodes];
        float[] action;
        foreach (var ai in aiList)
        {
            if (!ai.GetComponent<ZombieKnight_AI_Behaviour>().off /*&& car.GetComponent<Rigidbody>().velocity.sqrMagnitude <= Mathf.Pow(carMaxSpeed, 2)*/)
            {
                output = ai.GetComponent<ZombieKnight_AI_Behaviour>().GetOutput(inputNodes);
                action = ai.GetComponent<ZombieKnight_AI_Behaviour>().GetActionFromOutput(output);
                ai.GetComponent<ZombieKnight_AI_Behaviour>().RunAI(action);
            }
        }

        if (!trainingMode)
        {
            Camera.main.transform.position = Vector3.SmoothDamp(Camera.main.transform.position, aiList[0].transform.GetChild(1).position, ref SmoothPosVelocity, 0.7f); // Smoothly set the position

            Camera.main.transform.rotation = Quaternion.Lerp(Camera.main.transform.rotation,
                                                             Quaternion.LookRotation(aiList[0].transform.position - Camera.main.transform.position),
                                                             0.1f); // Smoothly set the rotation
        }
    }

    public void UpdateLiveTime()
    {
        foreach (var ai in aiList)
        {
            ai.GetComponent<ZombieKnight_AI_Behaviour>().UpdateLiveTime();
        }
    }

    public void ShutDownAIs()
    {
        foreach (var ai in aiList)
        {
            ai.GetComponent<ZombieKnight_AI_Behaviour>().ShutDownAI();
        }
    }

    public float Remap(float fitness, float from1, float to1, float from2, float to2)
    {
        return from2 + (fitness - from1) * (to2 - from2) / (to1 - from1);
    }

    public void UpdateCamera()
    {
        Camera.main.transform.position = Vector3.Lerp(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10), aiList[bestAlessiaAI].transform.position, Time.fixedDeltaTime);
    }

    private ZombieKnight_AI_DNA PickOne(List<GameObject> ais)
    {
        int index = 0;
        double r = Random.Range(0f, 1f);

        while (r > 0)
        {
            r -= ais[index].GetComponent<ZombieKnight_AI_DNA>().probability;
            index++;
        }
        index--;

        return ais[index].GetComponent<ZombieKnight_AI_DNA>();
    }

    private void ReadBestTrainedData(string filePath, ref NeuralNetwork neuralNetwork)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        string str = Encoding.ASCII.GetString(bytes);
        string[] data = str.Split('\n');
        int i = 0;
        for (; i < neuralNetwork.ihWeights.rowNb; i++)
        {
            int j = 0;
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (; j < neuralNetwork.ihWeights.columnNb; j++)
            {
                neuralNetwork.ihWeights[i][j] = float.Parse(number[j]);
            }
        }
        int t = 0;
        for (; i < neuralNetwork.ihWeights.rowNb + neuralNetwork.hoWeights.rowNb; i++)
        {
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (int j = 0; j < 5; j++)
            {
                neuralNetwork.hoWeights[t][j] = float.Parse(number[j]);
            }
            t++;
        }
        t = 0;
        for (; i < neuralNetwork.ihWeights.rowNb + neuralNetwork.hoWeights.rowNb + neuralNetwork.biasH.rowNb; i++)
        {
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (int j = 0; j < neuralNetwork.biasH.columnNb; j++)
            {
                neuralNetwork.biasH[t][j] = float.Parse(number[j]);
            }
            t++;
        }
        t = 0;
        for (; i < neuralNetwork.ihWeights.rowNb + neuralNetwork.hoWeights.rowNb + neuralNetwork.biasH.rowNb + neuralNetwork.biasO.rowNb; i++)
        {
            string[] number = data[i].Replace("  ", " ").Split(' ');
            for (int j = 0; j < neuralNetwork.biasO.columnNb; j++)
            {
                neuralNetwork.biasO[t][j] = float.Parse(number[j]);
            }
            t++;
        }
        int b = 2;
    }
}

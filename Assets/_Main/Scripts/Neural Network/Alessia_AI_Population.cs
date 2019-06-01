using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows;

public class Alessia_AI_Population : MonoBehaviour {

    public Transform alessiaAIParent;
    public GameObject alessiaPrefab;
    public GameObject bestAlessiaPrefab;
    public int AI_Amount;
    //public float carMaxSpeed;

    [HideInInspector] public float mutationRate;                          // Mutation rate
    [HideInInspector] public List<GameObject> alessiaAIs;                // Array to hold the current population
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
        alessiaAIs = new List<GameObject>();
        if (trainingMode)
        {
            for (int i = 0; i < AI_Amount; i++)
            {
                alessiaAIs.Add(Instantiate(alessiaPrefab, spawnPosition, alessiaPrefab.transform.rotation, alessiaAIParent));
                alessiaAIs[i].GetComponent<Alessia_AI_DNA>().InitDNA(inputNodes, hiddenNodes, outputNodes);
                alessiaAIs[i].name = $"AlessiaAI {i + 1}";
            }
            ReadBestCarTrainedData("Assets/Training_Result/bestAlessiaAI.txt", ref alessiaAIs[0].GetComponent<Alessia_AI_DNA>().neuralNetwork);
        }
        else
        {
            alessiaAIs.Add(Instantiate(bestAlessiaPrefab, bestAlessiaPrefab.transform.position, bestAlessiaPrefab.transform.rotation, alessiaAIParent));
            alessiaAIs[0].GetComponent<Alessia_AI_DNA>().InitDNA(inputNodes, hiddenNodes, outputNodes);
            ReadBestCarTrainedData("Assets/Training_Result/bestAlessiaAI.txt", ref alessiaAIs[0].GetComponent<Alessia_AI_DNA>().neuralNetwork);
        }
    }

    public void CalculateFitness()
    {
        for (int i = 0; i < alessiaAIs.Count; i++)
        {
            alessiaAIs[i].GetComponent<Alessia_AI_DNA>().CalculateFitness();

        }
    }

    // Generate a mating pool
    public void NaturalSelection()
    {
        float totalFitness = 0;
        for (int i = 0; i < alessiaAIs.Count; i++)
        {
            totalFitness += alessiaAIs[i].GetComponent<Alessia_AI_DNA>().fitness;
        }
        for (int i = 0; i < alessiaAIs.Count; i++)
        {
            alessiaAIs[i].GetComponent<Alessia_AI_DNA>().probability = (double)alessiaAIs[i].GetComponent<Alessia_AI_DNA>().fitness / (double)totalFitness;
        }
    }

    // Create a new generation
    public void Generate()
    {
        NeuralNetwork[] temp = new NeuralNetwork[alessiaAIs.Count];
        // Refill the population with children from the mating pool
        for (int i = 0; i < alessiaAIs.Count - 1; i++)
        {
            if (i == bestAlessiaAI)
            {
                temp[i] = alessiaAIs[i].GetComponent<Alessia_AI_DNA>().neuralNetwork;
            }
            else
            {
                Alessia_AI_DNA partnerA = PickOne(alessiaAIs);
                Alessia_AI_DNA partnerB = PickOne(alessiaAIs);
                NeuralNetwork child = partnerA.CrossOver(partnerB);
                Mutate(ref child, mutationRate);
                temp[i] = child;
            }
        }
        for (int i = 0; i < alessiaAIs.Count - 1; i++)
        {
            alessiaAIs[i].GetComponent<Alessia_AI_DNA>().neuralNetwork = temp[i];
        }
        alessiaAIs[alessiaAIs.Count - 1].GetComponent<Alessia_AI_DNA>().neuralNetwork = new NeuralNetwork(inputNodes, hiddenNodes, outputNodes);
        //if (matingPool.Count != 0)
        {
            generations++;
        }
        RestartAlessiaAIs(spawnPosition);
    }

    // Compute the current "most fit" member of the population
    public string Evaluate()
    {
        float worldrecord = 0.0f;
        for (int i = 0; i < alessiaAIs.Count; i++)
        {
            //Console.WriteLine(dnas[i].fitness);
            if (alessiaAIs[i].GetComponent<Alessia_AI_DNA>().fitness > worldrecord)
            {
                worldrecord = alessiaAIs[i].GetComponent<Alessia_AI_DNA>().fitness;
                bestAlessiaAI = i;
            }
            if (alessiaAIs[i].GetComponent<Alessia_AI_Behaviour>().finish)
            {
                finished = true;
            }
        }
        File.WriteAllBytes("Assets/Training_Result/bestAlessiaAI.txt", alessiaAIs[bestAlessiaAI].GetComponent<Alessia_AI_DNA>().neuralNetwork.ToByteArray());
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
        foreach (var car in alessiaAIs)
        {
            if (!car.GetComponent<Alessia_AI_Behaviour>().off)
            {
                return false;
            }
        }
        return true;
    }

    public void RestartAlessiaAIs(Vector3 spawnPos)
    {
        foreach (var car in alessiaAIs)
        {
            car.GetComponent<Alessia_AI_Behaviour>().RestartAlessiaAI(spawnPos, alessiaPrefab.transform.rotation);
        }
    }

    public void RunAlessiaAIs()
    {
        float[] output = new float[this.outputNodes];
        float[] action;
        foreach (var ai in alessiaAIs)
        {
            if (!ai.GetComponent<Alessia_AI_Behaviour>().off /*&& car.GetComponent<Rigidbody>().velocity.sqrMagnitude <= Mathf.Pow(carMaxSpeed, 2)*/)
            {
                output = ai.GetComponent<Alessia_AI_Behaviour>().GetOutput(inputNodes);
                action = ai.GetComponent<Alessia_AI_Behaviour>().GetActionFromOutput(output);
                ai.GetComponent<Alessia_AI_Behaviour>().RunAlessiaAI(action);
            }
        }
    }

    public void UpdateLiveTime()
    {
        foreach (var car in alessiaAIs)
        {
            car.GetComponent<Alessia_AI_Behaviour>().UpdateLiveTime();
        }
    }

    public void ShutDownAlessiaAIs()
    {
        foreach (var car in alessiaAIs)
        {
            car.GetComponent<Alessia_AI_Behaviour>().ShutDownAlessiaAI();
        }
    }

    public float Remap(float fitness, float from1, float to1, float from2, float to2)
    {
        return from2 + (fitness - from1) * (to2 - from2) / (to1 - from1);
    }

    public void UpdateCamera()
    {
        Vector3 destination = trainingMode ? new Vector3(alessiaAIs[bestAlessiaAI].transform.position.x, alessiaAIs[bestAlessiaAI].transform.position.y, -10) : new Vector3(alessiaAIs[0].transform.position.x, alessiaAIs[0].transform.position.y, -10);
        Camera.main.transform.position = Vector3.Lerp(new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, -10), destination, Time.fixedDeltaTime);
    }

    private Alessia_AI_DNA PickOne(List<GameObject> cars)
    {
        int index = 0;
        double r = Random.Range(0f, 1f);

        while (r > 0)
        {
            r -= cars[index].GetComponent<Alessia_AI_DNA>().probability;
            index++;
        }
        index--;

        return cars[index].GetComponent<Alessia_AI_DNA>();
    }

    private void ReadBestCarTrainedData(string filePath, ref NeuralNetwork neuralNetwork)
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlessiaDNA : MonoBehaviour {

    // The genetic sequence
    public NeuralNetwork neuralNetwork;

    [HideInInspector] public float fitness;
    [HideInInspector] public float score;
    [HideInInspector] public double probability;

    private AlessiaBehaviour alessiaBehaviour;

    private void Start()
    {
        alessiaBehaviour = GetComponent<AlessiaBehaviour>();
    }

    // Constructor (makes a random DNA)
    public void InitDNA(int inputNodes, int hiddenNodes, int outputNodes)
    {
        neuralNetwork = new NeuralNetwork(inputNodes, hiddenNodes, outputNodes);
    }

    public void CalculateFitness()
    {
        score = Mathf.Pow(10, alessiaBehaviour.point);
        //fitness = score / Mathf.Pow(carBehaviour.driveTime, 2);
        fitness = score;
    }

    // Crossover
    public NeuralNetwork CrossOver(AlessiaDNA partner)
    {
        // A new child
        NeuralNetwork child = new NeuralNetwork(neuralNetwork.inputNodes, neuralNetwork.hiddenNodes, neuralNetwork.outputNodes);

        //Do crossover
        DoCrossOver(ref child.ihWeights, neuralNetwork.ihWeights, partner.neuralNetwork.ihWeights);
        //for(int i = 0; i < neuralNetwork.ihWeights.rowNb; i++)
        //{
        //    for(int j = 0; j < neuralNetwork.ihWeights.columnNb; j++)
        //    {
        //        child.ihWeights[i][j] = Random.Range(0, 3) == 0 ? neuralNetwork.ihWeights[i][j] : partner.neuralNetwork.ihWeights[i][j];
        //    }
        //}
        DoCrossOver(ref child.hoWeights, neuralNetwork.hoWeights, partner.neuralNetwork.hoWeights);
        //for (int i = 0; i < neuralNetwork.hoWeights.rowNb; i++)
        //{
        //    for (int j = 0; j < neuralNetwork.hoWeights.columnNb; j++)
        //    {
        //        child.hoWeights[i][j] = Random.Range(0, 3) == 0 ? neuralNetwork.hoWeights[i][j] : partner.neuralNetwork.hoWeights[i][j];
        //    }
        //}
        DoCrossOver(ref child.biasH, neuralNetwork.biasH, partner.neuralNetwork.biasH);
        //for (int i = 0; i < neuralNetwork.biasH.rowNb; i++)
        //{
        //    for (int j = 0; j < neuralNetwork.biasH.columnNb; j++)
        //    {
        //        child.biasH[i][j] = Random.Range(0, 3) == 0 ? neuralNetwork.biasH[i][j] : partner.neuralNetwork.biasH[i][j];
        //    }
        //}
        DoCrossOver(ref child.biasO, neuralNetwork.biasO, partner.neuralNetwork.biasO);
        //for (int i = 0; i < neuralNetwork.biasO.rowNb; i++)
        //{
        //    for (int j = 0; j < neuralNetwork.biasO.columnNb; j++)
        //    {
        //        child.biasO[i][j] = Random.Range(0, 3) == 0 ? neuralNetwork.biasO[i][j] : partner.neuralNetwork.biasO[i][j];
        //    }
        //}

        return child;
    }

    private void DoCrossOver(ref Matrix child, Matrix partner1, Matrix partner2)
    {
        for (int i = 0; i < partner1.rowNb; i++)
        {
            for (int j = 0; j < partner1.columnNb; j++)
            {
                child[i][j] = Random.Range(0, 3) == 0 ? partner1[i][j] : partner2[i][j];
            }
        }
    }
}

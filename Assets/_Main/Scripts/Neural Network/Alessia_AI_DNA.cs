using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alessia_AI_DNA : MonoBehaviour {

    // The genetic sequence
    public NeuralNetwork neuralNetwork;

    [HideInInspector] public float fitness;
    [HideInInspector] public float score;
    [HideInInspector] public double probability;

    private Alessia_AI_Behaviour alessiaAIBehaviour;

    private void Start()
    {
        alessiaAIBehaviour = GetComponent<Alessia_AI_Behaviour>();
    }

    // Constructor (makes a random DNA)
    public void InitDNA(int inputNodes, int hiddenNodes, int outputNodes)
    {
        neuralNetwork = new NeuralNetwork(inputNodes, hiddenNodes, outputNodes);
    }

    public void CalculateFitness()
    {
        //score = Mathf.Pow(10, alessiaAIBehaviour.point);
        //fitness = score / Mathf.Pow(carBehaviour.driveTime, 2);
        fitness = Mathf.Pow(score, 3);
    }

    // Crossover
    public NeuralNetwork CrossOver(Alessia_AI_DNA partner)
    {
        // A new child
        NeuralNetwork child = new NeuralNetwork(neuralNetwork.inputNodes, neuralNetwork.hiddenNodes, neuralNetwork.outputNodes);

        //Do crossover
        DoCrossOver(ref child.ihWeights, neuralNetwork.ihWeights, partner.neuralNetwork.ihWeights);
        DoCrossOver(ref child.hoWeights, neuralNetwork.hoWeights, partner.neuralNetwork.hoWeights);
        DoCrossOver(ref child.biasH, neuralNetwork.biasH, partner.neuralNetwork.biasH);
        DoCrossOver(ref child.biasO, neuralNetwork.biasO, partner.neuralNetwork.biasO);

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

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;

public class NeuralController : MonoBehaviour
{
    public Transform spawnPosition;

    [Header("Neural Network")]
    public int inputNodes;
    public int hiddenNodes;
    public int outputNodes;

    [Header("Genetic Algorithm")]
    public float mutationRate;
    public bool trainingMode;

    [Header("Timescale")]
    public float timeScale;

    [Header("UI")]
    public TextMeshProUGUI textMeshPro;

    private Alessia_AI_Population alessia_AI_Population;

    // Start is called before the first frame update
    void Awake()
    {
        alessia_AI_Population = GetComponent<Alessia_AI_Population>();

        alessia_AI_Population.InitPopulation(mutationRate, inputNodes, hiddenNodes, outputNodes, spawnPosition.position, trainingMode);


    }

    private void Update()
    {
        if (Time.timeScale != timeScale)
        {
            Time.timeScale = timeScale;
        }
    }

    private void FixedUpdate()
    {
        if (!alessia_AI_Population.IsFinish())
        {
            if (alessia_AI_Population.AllOff() && trainingMode)
            {
                alessia_AI_Population.CalculateFitness();

                alessia_AI_Population.NaturalSelection();

                textMeshPro.text = $"Generation: {alessia_AI_Population.Evaluate()}";

                alessia_AI_Population.Generate();
            }
            else
            {
                alessia_AI_Population.RunAlessiaAIs();
                alessia_AI_Population.UpdateDriveTime();
                alessia_AI_Population.ShutDownAlessiaAIs();
            }
        }
    }
}

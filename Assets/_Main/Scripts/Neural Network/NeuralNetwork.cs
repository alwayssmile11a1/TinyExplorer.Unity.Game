using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NeuralNetwork
{
    public int inputNodes;
    public int hiddenNodes;
    public int outputNodes;

    public Matrix ihWeights;
    public Matrix hoWeights;

    public Matrix biasH;
    public Matrix biasO;

    public NeuralNetwork(int input, int hidden, int output)
    {
        inputNodes = input;
        hiddenNodes = hidden;
        outputNodes = output;

        ihWeights = new Matrix(hiddenNodes, inputNodes);
        hoWeights = new Matrix(outputNodes, hiddenNodes);

        ihWeights.Randomize();
        hoWeights.Randomize();

        biasH = new Matrix(hiddenNodes, 1);
        biasO = new Matrix(outputNodes, 1);

        biasH.Randomize();
        biasO.Randomize();
    }

    public float[] FeedForward(float[] inputArray)
    {
        // Generate hidden layer
        Matrix inputs = Matrix.FromArray(inputArray);
        Matrix hidden = ihWeights * inputs;
        hidden += biasH;
        // Apply activatate function for hidden layer
        hidden.map(Sigmoid);

        // Generate output layer
        Matrix output = hoWeights * hidden;
        output += biasO;
        output.map(Sigmoid);

        return output.ToArray();
    }

    public byte[] ToByteArray()
    {
        string str = string.Empty;

        str += MatrixToString(ihWeights);
        str += MatrixToString(hoWeights);
        str += MatrixToString(biasH);
        str += MatrixToString(biasO);

        return Encoding.ASCII.GetBytes(str);
    }

    private float SigmoidOf2MinusOne(float x)
    {
        return (2 / (1 + Mathf.Exp(-x))) - 1;
    }

    private float Sigmoid(float x)
    {
        return 1 / (1 + Mathf.Exp(-x));
    }

    private string MatrixToString(Matrix m)
    {
        string str = string.Empty;
        for (int i = 0; i < m.rowNb; i++)
        {
            for (int j = 0; j < m.columnNb; j++)
            {
                str += $"{m[i][j]}  ";
            }
            str += "\n";
        }
        return $"{str}";
    }
}

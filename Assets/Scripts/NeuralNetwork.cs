﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;

public class NeuralNetwork {

    private int inputNeurons;
    private int hiddenLayerNeurons;
    private int outputNeurons;
    public Matrix<float> W0;
    public Matrix<float> W1;


    public NeuralNetwork(int inputNeurons, int hiddenLayerNeurons, int outputNeurons, bool initWeights){
        this.inputNeurons = inputNeurons;
        this.hiddenLayerNeurons = hiddenLayerNeurons;
        this.outputNeurons = outputNeurons;
        this.W0 = Matrix<float>.Build.Dense(hiddenLayerNeurons, inputNeurons, 0);
        this.W1 = Matrix<float>.Build.Dense(outputNeurons, hiddenLayerNeurons, 0);
        if (initWeights) initializeWeights();
    }

    public void initializeWeights()
    {
        this.W0 = Matrix<float>.Build.Random(hiddenLayerNeurons, inputNeurons, new ContinuousUniform(-1,1));
        this.W1 = Matrix<float>.Build.Random(outputNeurons, hiddenLayerNeurons, new ContinuousUniform(-1,1));
    }

    public Matrix<float> runNeuralNetwork(Matrix<float> input){
        if(input.RowCount != inputNeurons){
            throw new InvalidInputException();
        }
        return computeHiddenLayer(input);
    }

    public Matrix<float> computeHiddenLayer(Matrix<float> input){
        Matrix<float> hiddenLayer = W0.Multiply(input).Divide(inputNeurons);
        if (Input.GetKeyDown(KeyCode.Y))Debug.Log("Weight matrix: \n" + W0.ToString() + "\nInput matrix:\n" + input.ToString() + "\nResult matrix:\n" + hiddenLayer.ToString());
        return computeOutputLayer(hiddenLayer);
    }

    public Matrix<float> computeOutputLayer(Matrix<float> hiddenLayer){
        Matrix<float> outputLayer = W1.Multiply(hiddenLayer).Divide(hiddenLayerNeurons);
        if(Input.GetKeyDown(KeyCode.Y))Debug.Log("Weight matrix: \n" + W1.ToString() + "\nHidden Layer matrix:\n" + hiddenLayer.ToString() + "\nResult matrix:\n" + outputLayer.ToString());
        return outputLayer;
    }

    public int getInputNeurons(){
        return inputNeurons;
    }

    public int getHiddenLayerNeurons(){
        return hiddenLayerNeurons;
    }

    public int getOutputNeurons(){
        return outputNeurons;
    }

    public void setWeights(Chromosome chromosome) {
        for (int to = 0; to < hiddenLayerNeurons; to++) {
            for (int from = 0; from < inputNeurons; from++) {
                int row = 0;
                int column = from + (to * inputNeurons);
                float value = chromosome.data.At(row, column);
                //Debug.Log("Set row" + to + " column" + from + " to value from chromosome at row" + row + " column" + column + " value" + value);
                W0.At(to, from, value);
            }
        }

        for (int to = 0; to < outputNeurons; to++) {
            for (int from = 0; from < hiddenLayerNeurons; from++) {
                W1.At(to, from, chromosome.data.At(0, (hiddenLayerNeurons * inputNeurons) + (from + (to * outputNeurons))));
            }
        }
    }
}

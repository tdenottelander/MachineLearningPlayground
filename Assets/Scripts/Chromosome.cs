using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;

public class Chromosome {

    public Matrix<float> data;
    private int amountOfElements;
    public int fitness;
    private float mutationParameter;

    public Chromosome(int amountOfElements, float mutationParameter){
        data = Matrix<float>.Build.Random(1, amountOfElements, new ContinuousUniform(-1, 1));
        this.mutationParameter = mutationParameter;
        this.amountOfElements = amountOfElements;
    }

    public Chromosome(Matrix<float> data, float mutationParameter){
        this.data = data;
        this.mutationParameter = mutationParameter;
    }

    public void mutate(){
        int indexOfMutation = Random.Range(0, amountOfElements);
        float dataToMutate = data.At(0, indexOfMutation);
        //Debug.Log("Mutate gene " + indexOfMutation + " with value " + dataToMutate);
        dataToMutate = dataToMutate + mutationParameter * Random.Range(-1f, 1f);
        if (dataToMutate > 1) dataToMutate = 1;
        if (dataToMutate < -1) dataToMutate = -1;
        //Debug.Log("Changed to " + dataToMutate);
        data.At(0, indexOfMutation, dataToMutate);
    }

    public void setFitness(int fitness){
        this.fitness = fitness;
    }

    override public string ToString(){
        return "Chromosome:\n" + data.ToString() + "\nFitness: " + fitness + "\nMutationParameter: " + mutationParameter;
    }

}

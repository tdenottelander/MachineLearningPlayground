using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
using System.Text;

public class Chromosome : System.IComparable<Chromosome> {

    public Matrix<float> data;
    private int amountOfElements;
    private int fitness;
    private float mutationParameter;

    public Chromosome(int amountOfElements, float mutationParameter){
        data = Matrix<float>.Build.Random(1, amountOfElements, new ContinuousUniform(-1, 1));
        this.mutationParameter = mutationParameter;
        this.amountOfElements = amountOfElements;
        this.fitness = 0;
    }

    public Chromosome(Matrix<float> data, float mutationParameter){
        this.data = data;
        this.mutationParameter = mutationParameter;
        this.fitness = 0;
    }

    public Chromosome(Chromosome c){
        this.data = c.data.Clone();
        this.amountOfElements = c.amountOfElements;
        this.mutationParameter = c.mutationParameter;
        this.fitness = 0;
    }

    /// <summary>
    /// Mutate some values of this chromosome and returns it as a clone.
    /// </summary>
    /// <returns>The mutate.</returns>
    public Chromosome mutate(float mutationParameter){
        StringBuilder sb = new StringBuilder();
        sb.Append("OLD CHROMOSOME: " + this.ToString() + "\n");
        Chromosome c = Clone();

        for (int i = 0; i < 10; i++) {
            int indexOfMutation = Random.Range(0, amountOfElements);
            float dataToMutate = c.data.At(0, indexOfMutation);
            sb.Append("\nmutating gene at index " + indexOfMutation + " from " + dataToMutate);
            //Debug.Log("Mutate gene " + indexOfMutation + " with value " + dataToMutate);
            dataToMutate = dataToMutate + mutationParameter * Random.Range(-1f, 1f);
            if (dataToMutate > 1) dataToMutate = 1;
            if (dataToMutate < -1) dataToMutate = -1;
            sb.Append(" to " + dataToMutate + "\n");
            //Debug.Log("Changed to " + dataToMutate);
            c.data.At(0, indexOfMutation, dataToMutate);
        }
        sb.Append("\n NEW CHROMOSOME: " + c.ToString());
        Debug.Log(sb.ToString());
        return c;
    }

    public void setFitness(int fitness){
        this.fitness = fitness;
    }

    public int getFitness(){
        return this.fitness;
    }

    override public string ToString(){
        return "Chromosome:\n" + data.ToString() + "\nFitness: " + fitness + "\nMutationParameter: " + mutationParameter;
    }

    public int CompareTo(Chromosome other) {
        return other.getFitness() - this.getFitness();
    }

    /// <summary>
    /// Clone this instance, but sets fitness to 0.
    /// </summary>
    /// <returns>The clone.</returns>
    public Chromosome Clone() {
        return new Chromosome(this);
    }
}

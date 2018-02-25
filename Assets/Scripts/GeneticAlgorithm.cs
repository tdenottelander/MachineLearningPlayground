using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm {

    private Chromosome[] chromosomes;
    public int amountOfChromosomes;

    public GeneticAlgorithm(int amountOfChromosomes) {
        this.amountOfChromosomes = amountOfChromosomes;
        chromosomes = new Chromosome[amountOfChromosomes];
    }

    public void initializeChromosomes(int genes, float changeRate) {
        for (int i = 0; i < chromosomes.Length; i++) {
            chromosomes[i] = new Chromosome(genes, changeRate);
            //Debug.Log(chromosomes[i].ToString());
        }
    }

    public void updateChromosomes(){
        foreach(Chromosome c in chromosomes){
            c.mutate();
            Debug.Log(c.ToString());
        }
    }

    public Chromosome getChromosome(int index){
        return chromosomes[index];
    }

}

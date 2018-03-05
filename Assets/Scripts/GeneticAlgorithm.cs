using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour {

    private Chromosome[] chromosomes;
    private int amountOfChromosomes;

    public void initialize(int amountOfChromosomes){
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
            c.mutate(0.2f);
            Debug.Log(c.ToString());
        }
    }

    public Chromosome getChromosome(int index){
        return chromosomes[index];
    }

    public void createNewGeneration(int chromosomesForOffspring) {
        Array.Sort(chromosomes);
        Debug.Log("Sorted chromosomes list: \n" + ToString());
        Chromosome[] offspring = new Chromosome[amountOfChromosomes];
        for (int i = 0; i < 5; i++) {
            //TODO Don't hardcode




            //Debug.Log("old chr: " + chromosomes[i]);
            Chromosome c = chromosomes[i].Clone();
            //Debug.Log("new chr: " + c);
            offspring[i * 4] = chromosomes[i].Clone().mutate(0.05f);
            offspring[(i * 4) + 1] = chromosomes[i].Clone().mutate(0.1f);
            offspring[(i * 4) + 2] = chromosomes[i].Clone().mutate(0.2f);
            offspring[(i * 4) + 3] = chromosomes[i].Clone().mutate(0.5f);
        }
        chromosomes = offspring;
        Debug.Log("NEW GENERATION: \n" + this.ToString());
    }

    override public String ToString(){
        StringBuilder sb = new StringBuilder();
        foreach (Chromosome c in chromosomes){
            sb.Append(c.ToString());
        }
        return sb.ToString();
    }
}

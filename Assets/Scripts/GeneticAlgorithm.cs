using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MonoBehaviour {

    private Chromosome[] chromosomes;
    public int amountOfChromosomes = 1;


	// Use this for initialization
	void Start () {
        chromosomes = new Chromosome[amountOfChromosomes];
        for (int i = 0; i < chromosomes.Length; i++){
            chromosomes[i] = new Chromosome(3, 0.1f);
            Debug.Log(chromosomes[i].ToString());
        }
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.U)){
            foreach(Chromosome c in chromosomes){
                c.mutate();
                Debug.Log(c.ToString());
            }
        }
	}

}

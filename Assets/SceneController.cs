using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour {

    public int carsPerGeneration = 5;
    public GameObject carPrefab;
    public List<CarController> cars;
    public Transform startingPosition;

	// Use this for initialization
	void Start () {
        cars = new List<CarController>();
        for (int i = 0; i < carsPerGeneration; i++){
            CarController currentCar = Instantiate(carPrefab, startingPosition.position, startingPosition.rotation).GetComponent<CarController>();
            cars.Add(currentCar);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

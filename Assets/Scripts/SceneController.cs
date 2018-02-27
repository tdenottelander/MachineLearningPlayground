using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour {

    public int carsPerGeneration = 1;
    public GameObject carPrefab;
    public List<CarController> cars, activeCars;
    public Transform startingPosition;
    public GeneticAlgorithm GA;
    public static SceneController Instance;
    //public GameObject startingObstacle;
    //public float secondsBeforeObstacleDestroy = 8f;
    public CarController bestCar;
    private UIController uiController;

    private void Awake()
    {
        if(Instance == null){
            Instance = this;
        } else {
            Destroy(this);
        }
    }

    // Use this for initialization
    void Start () {

        uiController = GetComponent<UIController>();
        cars = new List<CarController>();
        GA = new GeneticAlgorithm(carsPerGeneration);
        GA.initializeChromosomes((5 * 3) + (3 * 2), 0.1f);

        for (int i = 0; i < carsPerGeneration; i++){
            CarController currentCar = Instantiate(carPrefab, startingPosition.position, startingPosition.rotation).GetComponent<CarController>();
            currentCar.initialize();
            currentCar.setName("Car " + i);
            currentCar.getNN().setWeights(GA.getChromosome(i));
            cars.Add(currentCar);
        }
        //Make a shallow copy of the cars list.
        activeCars = new List<CarController>(cars);

        uiController.updateRankingText();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.U))
            GA.updateChromosomes();

    }

    public void setBestCar(CarController bestCar){
        if(this.bestCar != null && bestCar != this.bestCar)
            this.bestCar.setBestCar(false);
        this.bestCar = bestCar;
    }

    public List<CarController> getActiveCars(){
        activeCars.Sort();
        return this.activeCars;
    }

    public CarController getBestCar(){
        return this.bestCar;
    }

    public UIController getUIController(){
        return this.uiController;
    }

}

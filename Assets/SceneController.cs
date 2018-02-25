using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour {

    public int carsPerGeneration = 1;
    public GameObject carPrefab;
    public List<CarController> cars;
    public Transform startingPosition;
    public GeneticAlgorithm GA;
    public static SceneController Instance;
    //public GameObject startingObstacle;
    //public float secondsBeforeObstacleDestroy = 8f;
    public CarController bestCar;
    [SerializeField] private Text inputText, outputText, rewardText;

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
        cars = new List<CarController>();
        GA = new GeneticAlgorithm(carsPerGeneration);
        GA.initializeChromosomes((5 * 3) + (3 * 2), 0.1f);
        for (int i = 0; i < carsPerGeneration; i++){
            CarController currentCar = Instantiate(carPrefab, startingPosition.position, startingPosition.rotation).GetComponent<CarController>();
            currentCar.initialize();
            currentCar.setName("Car " + i);
            currentCar.setTexts(inputText, outputText, rewardText);
            currentCar.getNN().setWeights(GA.getChromosome(i));
            cars.Add(currentCar);
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.U))
            GA.updateChromosomes();

        highlight();
	}

    private void highlight() {
        cars.Sort(new CarController.CarControllerComparer());
        if (bestCar == null) bestCar = cars[0];
        int index = 0;
        while (index < cars.Count) {
            if (cars[index].isAlive() && cars[index] != bestCar) {
                bestCar.setBestCar(false);
                cars[index].setBestCar(true);
                bestCar = cars[index];
                break;
            }
        }

    }


}

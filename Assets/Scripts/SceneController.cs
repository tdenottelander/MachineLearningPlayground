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
    private float timeLastUpdate;
    public float timeOut;
    private CameraController camController;
    public enum Mode {HardcodedGA}
    public Mode mode;
    //private bool running;
    private bool resetTimescaleOnNewGen = true;

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
        GA = GetComponent<GeneticAlgorithm>();
        GA.initialize(carsPerGeneration);
        GA.initializeChromosomes((5 * 3) + (3 * 2), 0.5f);
        camController = Camera.main.GetComponent<CameraController>();

        initialize();
	}

    void initialize(){
        cars = new List<CarController>();
        for (int i = 0; i < carsPerGeneration; i++) {
            CarController currentCar = Instantiate(carPrefab, startingPosition.position, startingPosition.rotation).GetComponent<CarController>();
            currentCar.initialize(timeOut);
            currentCar.setName("Car " + i);
            currentCar.setupNN(GA.getChromosome(i));
            cars.Add(currentCar);
        }
        //Make a shallow copy of the cars list.
        activeCars = new List<CarController>(cars);
        camController.setTarget(cars[0].transform);
        //running = true;
        uiController.updateRankingText();
        timeLastUpdate = Time.time;
    }

    // Update is called once per frame
    void Update() {
        //if (Input.GetKeyDown(KeyCode.U))
        //GA.updateChromosomes();

        //if (running) {
        //    float timeUntilTimeout = timeOut - (Time.time - timeLastUpdate);
        //    if (timeUntilTimeout < 0) {
        //        uiController.setTimeOutText("" + 0);
        //        running = false;
        //        StartCoroutine(endGeneration());
        //    } else {
        //        uiController.setTimeOutText(timeUntilTimeout.ToString());
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.Alpha1)) Time.timeScale -= 0.1f;
        if (Input.GetKeyDown(KeyCode.Alpha2)) Time.timeScale += 0.1f;
        if (Input.GetKeyDown(KeyCode.Alpha3)) setTimeScale(1f);
    }

    public void setTimeScale(float timeScale) {
        Time.timeScale = timeScale;
    }

    public void startCoroutineEndGeneration(){
        StartCoroutine(endGeneration());
    }

    private IEnumerator endGeneration(){
        //Freeze all cars
        foreach (CarController c in activeCars){
            c.setDead(true);
        }
        if (resetTimescaleOnNewGen) setTimeScale(1);
        uiController.setEndGenerationTextActive(true);
        yield return new WaitForSeconds(3);
        GA.createNewGeneration(5);
        destroyCars();
        uiController.setEndGenerationTextActive(false);
        initialize();
        yield return null;
    }

    public void setBestCar(CarController bestCar){
        if(this.bestCar != null && bestCar != this.bestCar)
            this.bestCar.setBestCar(false);
        this.bestCar = bestCar;
        camController.setTarget(bestCar.transform);
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

    //public void setTimeLastUpdate(){
    //    this.timeLastUpdate = Time.time;
    //}

    private void destroyCars(){
        for (int i = 0; i < cars.Count; i++){
            Destroy(cars[i].gameObject);
        }
    }

    public void setResetTimescaleOnNewGen(bool active) {
        resetTimescaleOnNewGen = active;
    }

}

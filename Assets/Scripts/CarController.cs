using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using System;

public class CarController : MonoBehaviour, System.IComparable<CarController> {

    public bool controlledByKeyboard = false;
    [Range(0,100)]public float speed;
    [Range(0,200)]public float steeringMultiplier;
    public Transform[] directions;
    public float maximumRaycast = 10;
    public LayerMask layermask;
    [SerializeField ]private int reward;
    private Matrix<float> input;
    private NeuralNetwork nn;
    new private string name;
    private bool alive;
    public bool write = false;
    private float timeLastReward;
    public float timeOut;
    private SceneController sceneController;
    private UIController uiController;
    private Chromosome chromosome;
    private List<Checkpoint> checkPoints;
    private int roundsCompleted;
    public enum CarMode { AccAndSteer, SteerOnly };
    public CarMode carMode;

    public void initialize(float timeOut){
        alive = true;
        reward = 0;
        roundsCompleted = 0;
        checkPoints = new List<Checkpoint>();
        write = false;
        input = Matrix<float>.Build.Dense(directions.Length, 1, 0);
        nn = new NeuralNetwork(directions.Length, 3, 2, false);
        timeLastReward = Time.time;
        this.timeOut = timeOut;
        sceneController = SceneController.Instance;
        uiController = sceneController.getUIController();

    }
	
	// Update is called once per frame
	void Update () {
        if (alive) {


            toggleKeyboard();

            raycast();
            if (write) uiController.setInputText(input.ToString());

            Matrix<float> output = nn.runNeuralNetwork(input);

            if (write) uiController.setOutputText(output.ToString());

            if (controlledByKeyboard) {
                float steeringInput = steeringMultiplier * Input.GetAxis("Horizontal");
                //Debug.Log(steer);
                steer(steeringInput);

                float forwardInput = speed * Input.GetAxis("Vertical");
                if (forwardInput < 0) forwardInput = 0;
                move(forwardInput);
            } else {
                steer(steeringMultiplier * output.At(1, 0));
                if (carMode.Equals(CarMode.AccAndSteer)) {
                    float forwardInput = speed * output.At(0, 0);
                    if (forwardInput < 0) forwardInput = 0;
                    move(forwardInput);
                } else if (carMode.Equals(CarMode.SteerOnly)){
                    move(speed * 0.1f);
                }
            }

            //if (Input.GetKeyDown(KeyCode.Y)) {
            //    try {
            //        nn.runNeuralNetwork(input);
            //    } catch (InvalidInputException e) {
            //        e.ToString();
            //        Debug.LogError("The input and amount of input neurons do not match.");
            //    }
            //}

            if(Time.time - timeLastReward > timeOut){
                setDead(false);
            }

        }
  	}

    internal void setupNN(Chromosome chromosome) {
        this.chromosome = chromosome;
        getNN().setWeights(chromosome);
    }

    public void raycast() {
        for (int i = 0; i < directions.Length; i++){
            RaycastHit rch;
            bool raycast = Physics.Raycast(transform.position, directions[i].position - transform.position, out rch, maximumRaycast,layermask);
            float length = maximumRaycast;
            if (raycast)
            {
                //Debug.Log("Raycast " + (raycast ? "hit" : "no hit"));
                length = (rch.point - transform.position).magnitude;
                //Debug.Log(length);
            }
            input.At(i,0,length/maximumRaycast);
            Color color = Color.Lerp(Color.red, Color.green, length / maximumRaycast);
            Debug.DrawLine(transform.position, transform.position + maximumRaycast * (directions[i].position - transform.position).normalized, color, 0, true);
        }
    }

    public void move(float speed){
        transform.Translate(0,0,speed * Time.deltaTime,Space.Self);
    }

    public void steer(float input){
        transform.Rotate(new Vector3(0, input * Time.deltaTime, 0));
    }

    void toggleKeyboard(){
        if (Input.GetKeyDown(KeyCode.T))
        {
            controlledByKeyboard = !controlledByKeyboard;
            Debug.Log("Keyboard control is " + (controlledByKeyboard ? "on" : "off") + ".");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Wall")){
            setDead(false);
        } else if(other.CompareTag("Checkpoint")){
            addReward(other.GetComponent<Checkpoint>());
        }
    }

    private void addReward(Checkpoint checkPoint){
        if (!checkPoints.Contains(checkPoint)) {


            if(checkPoint.isFinishLine()){
                //Only add if currentReward = roundsFinished * (maxAmount of Points per Round(440) + finishLine(100)) + maxAmount of Points per Round(440)
                if (getReward() < (roundsCompleted * 540) + 440){ 
                    return;
                } else {
                    roundsCompleted++;
                    checkPoints = new List<Checkpoint>();
                }
            } else {
                checkPoints.Add(checkPoint);
            }
            this.reward += checkPoint.getReward();
            if (write) uiController.setRewardText("Reward: " + reward);
            timeLastReward = Time.time;
            uiController.updateRankingText();
            //sceneController.setTimeLastUpdate();
            if (sceneController.getActiveCars()[0].Equals(this)) {
                setBestCar(true);
            }
        }
        //Debug.Log("+" + other.GetComponent<Checkpoint>().getReward() + " reward");
    }

    public void setBestCar(bool active) {
        if (active) sceneController.setBestCar(this);
        write = active;
        GetComponent<MeshRenderer>().material.color = active ? Color.yellow : (alive ? Color.white : Color.red);
        //Debug.Log("Set car " + this.name + (active ? "active" : "inactive"));
    }

    public void setDead(bool endOfGeneration){
        this.alive = false;
        this.chromosome.setFitness(getReward());
        //Debug.Log(this.name + " is ded. Got a score of " + getReward());
        uiController.updateRankingText();
        this.GetComponent<MeshRenderer>().material.color = Color.red;

        if (!endOfGeneration) {
            List<CarController> listOfActiveCars = sceneController.getActiveCars();
            listOfActiveCars.Remove(this);
            if (sceneController.getBestCar().Equals(this)) {
                if (listOfActiveCars.Count != 0)
                    listOfActiveCars[0].setBestCar(true);
                else
                    SceneController.Instance.startCoroutineEndGeneration();
            }
        }
    }

    public void setName(string name){
        this.name = name;
    }

    public NeuralNetwork getNN(){
        return this.nn;
    }

    public int getReward() {
        return reward;
    }

    public string getName(){
        return name;
    }

    public bool isAlive(){
        return alive;
    }

    public int Compare(CarController x, CarController y)
    {
        if (x == null || y == null)
        {
            Debug.Log("either car is null");
            return 0;
        }
        return y.getReward().CompareTo(x.getReward());
    }

    public int CompareTo(CarController other) {
        if (other == null || this == null) {
            Debug.Log("either car is null");
            return 0;
        }

        int result = other.getReward().CompareTo(this.getReward());
        if (result == 0)
            result = this.timeLastReward.CompareTo(other.timeLastReward);
        return result;
    }
}

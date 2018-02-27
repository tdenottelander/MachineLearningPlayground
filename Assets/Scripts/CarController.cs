using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;

public class CarController : MonoBehaviour, System.IComparable<CarController> {

    public bool controlledByKeyboard = false;
    [Range(0,1)]public float speed;
    [Range(0,2)]public float steeringMultiplier;
    public Transform[] directions;
    public float maximumRaycast = 10;
    public LayerMask layermask;
    [SerializeField ]private int reward;
    private Matrix<float> input;
    private NeuralNetwork nn;
    new private string name;
    private bool alive;
    public bool write = false;
    private float lastReward;
    private SceneController sceneController;
    private UIController uiController;

    public void initialize(){
        alive = true;
        reward = 0;
        write = false;
        input = Matrix<float>.Build.Dense(directions.Length, 1, 0);
        nn = new NeuralNetwork(directions.Length, 3, 2, false);
        lastReward = Time.time;
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
                float forwardInput = speed * output.At(0, 0);
                if (forwardInput < 0) forwardInput = 0;
                move(forwardInput);
            }

            if (Input.GetKeyDown(KeyCode.Y)) {
                try {
                    nn.runNeuralNetwork(input);
                } catch (InvalidInputException e) {
                    e.ToString();
                    Debug.LogError("The input and amount of input neurons do not match.");
                }
            }
        }
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
        transform.Translate(0,0,speed,Space.Self);
    }

    public void steer(float input){
        transform.Rotate(new Vector3(0, input, 0));
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
            setDead();
        } else if(other.CompareTag("Checkpoint")){
            reward += other.GetComponent<Checkpoint>().getReward();
            if(write) uiController.setRewardText("Reward: " + reward);
            lastReward = Time.time;
            uiController.updateRankingText();
            if(sceneController.getActiveCars()[0].Equals(this)){
                setBestCar(true);
            }
            //Debug.Log("+" + other.GetComponent<Checkpoint>().getReward() + " reward");
        }
    }

    public void setBestCar(bool active) {
        if (active) sceneController.setBestCar(this);
        write = active;
        GetComponent<MeshRenderer>().material.color = active ? Color.yellow : (alive ? Color.white : Color.red);
        //Debug.Log("Set car " + this.name + (active ? "active" : "inactive"));
    }

    private void setDead(){
        this.alive = false;
        Debug.Log(this.name + " is ded. Got a score of " + getReward());
        uiController.updateRankingText();
        this.GetComponent<MeshRenderer>().material.color = Color.red;
        List<CarController> listOfActiveCars = sceneController.getActiveCars();
        listOfActiveCars.Remove(this);
        if (sceneController.getBestCar().Equals(this)) {
            listOfActiveCars[0].setBestCar(true);
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
            result = this.lastReward.CompareTo(other.lastReward);
        return result;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;

public class CarController : MonoBehaviour {

    public bool controlledByKeyboard = true;
    [Range(0,1)]public float speed;
    [Range(0,2)]public float steeringMultiplier;
    public Transform[] directions;
    public float maximumRaycast = 10;
    public LayerMask layermask;
    private int reward;
    [SerializeField] private Text rewardText, inputText, outputText;
    private Matrix<float> input;
    private NeuralNetwork nn;
    private string name;
    private bool active;

    // Use this for initialization
    void Start()
    {
        active = true;
        name = "supercar";
        reward = 0;
        input = Matrix<float>.Build.Dense(directions.Length, 1, 0);
        nn = new NeuralNetwork(directions.Length, 3, 2, true);
    }
	
	// Update is called once per frame
	void Update () {
        if (active) {
            if (controlledByKeyboard) {
                float steeringInput = steeringMultiplier * Input.GetAxis("Horizontal");
                //Debug.Log(steer);
                steer(steeringInput);

                float forwardInput = speed * Input.GetAxis("Vertical");
                if (forwardInput < 0) forwardInput = 0;
                move(forwardInput);
            }

            toggleKeyboard();

            raycast();
            setInputText();

            Matrix<float> output = nn.runNeuralNetwork(input);
            setOutputText(output);

            if (Input.GetKeyDown(KeyCode.Y)) {
                try {
                    nn.runNeuralNetwork(input);
                } catch (InvalidInputException e) {
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

    public void setInputText() {
        if(inputText!=null)inputText.text = input.ToString();
    }

    public void setOutputText(Matrix<float> output){
        if(outputText!=null)outputText.text = output.ToString();
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
            setInactive();
        } else if(other.CompareTag("Checkpoint")){
            reward += other.GetComponent<Checkpoint>().getReward();
            if(rewardText!=null)rewardText.text = "Reward: " + reward;
            //Debug.Log("+" + other.GetComponent<Checkpoint>().getReward() + " reward");
        }
    }

    private void setInactive(){
        this.active = false;
        Debug.Log(this.name + " is ded.");
        this.GetComponent<MeshRenderer>().material.color = Color.red;

    }
}

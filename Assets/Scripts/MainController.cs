using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;

public class MainController : MonoBehaviour {

	public Text myText;
    [Range(0,2)]public float timeOut;
    private float lastTime;
    private List<GameObject> objects = new List<GameObject>();
    public GameObject prefabObject;
    public MaximalDimensions maxDim;
    [Range(1,100)] public int amountOfObjects;
    public IContinuousDistribution currentDistribution;
    public List<IContinuousDistribution> distList = new List<IContinuousDistribution>();
    private int distributionIndex = 0;

    [System.Serializable]
    public struct MaximalDimensions{
        [Range(0, 20)] public float rangeX;
        [Range(0, 20)] public float rangeZ;
    }

	// Use this for initialization
	void Start () {
        initializeDistributions();
        lastTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time - lastTime > timeOut)
        {
            //alterText();
            Matrix<float> A = Matrix<float>.Build.Random(amountOfObjects, 2, currentDistribution);
            //Debug.Log(A.ToString());
            deleteObjects();
            createObjects(A);
            lastTime = Time.time;
        }

        if(Input.GetKeyDown(KeyCode.D)){
            cycleDistribution();
        }
	}

    void deleteObjects(){
        foreach (GameObject obj in objects){
            Destroy(obj);
        }
        objects = new List<GameObject>();
    }

    void createObjects(Matrix<float> coordinates){
        for (int i = 0; i < coordinates.RowCount; i++){
            GameObject newObject = 
                Instantiate(prefabObject, 
                            new Vector3(coordinates.At(i, 0) * Random.Range(-maxDim.rangeX, maxDim.rangeX), 0, 
                                        coordinates.At(i, 1) * Random.Range(-maxDim.rangeZ, maxDim.rangeZ)), transform.rotation);
            objects.Add(newObject);
        }
    }

    void initializeDistributions(){
        distList.Add(new ContinuousUniform());
        distList.Add(new Normal());
        distList.Add(new Gamma(2,2));
        distList.Add(new MathNet.Numerics.Distributions.Exponential(0.1));
        currentDistribution = distList[distributionIndex];
        setDistributionText();
    }

    void setDistributionText(){
        myText.text = "" + currentDistribution.ToString();
    }

    void cycleDistribution(){
        distributionIndex = (distributionIndex + 1) % distList.Count;
        currentDistribution = distList[distributionIndex % distList.Count];
        setDistributionText();
    }

    void alterText(){
        myText.text = "" + Random.value;
        myText.color = new Color(Random.value, Random.value, Random.value);
    }
}

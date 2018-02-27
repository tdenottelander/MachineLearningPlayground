using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField] private Text inputText, outputText, rewardText, rankingText;
    private SceneController sceneController;

    void Start(){
        sceneController = GetComponent<SceneController>();
    }

    public void setInputText(string text){
        inputText.text = text;
    }

    public void setOutputText(string text){
        outputText.text = text;
    }

    public void setRewardText(string text){
        rewardText.text = text;
    }

    public void setRankingText(string text){
        rankingText.text = text;
    }

    public void updateRankingText(){
        if(sceneController != null){
            List<CarController> cars = new List<CarController>(sceneController.cars);
            cars.Sort();
            StringBuilder sb = new StringBuilder();
            foreach (CarController c in cars){
                if (!c.isAlive()) sb.Append("<color=red>");
                sb.Append(c.getName() + " - " + c.getReward() + "\n");
                if (!c.isAlive()) sb.Append("</color>");
            }
            rankingText.text = sb.ToString();
        } else {
            rankingText.text = "Can't find scenecontroller";
        }
    }
}

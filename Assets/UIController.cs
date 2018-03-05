using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField] private Text inputText, outputText, rewardText, rankingText, endGenerationText, timeOutText;
    [SerializeField] private GameObject helpPanelMinimized, helpPanelMaximized;
    private SceneController sceneController;

    void Start(){
        sceneController = GetComponent<SceneController>();
        setEndGenerationTextActive(false);
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

    public void setEndGenerationTextActive(bool active){
        endGenerationText.gameObject.SetActive(active);
    }

    public void setTimeOutText(string text){
        timeOutText.text = text;
    }

    public void setHelpPanel(bool active){
        helpPanelMinimized.SetActive(!active);
        helpPanelMaximized.SetActive(active);
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

    void Update(){
        if(Input.GetKeyDown(KeyCode.H)){
            setHelpPanel(helpPanelMinimized.activeSelf);
        }
    }

    public void setTimeScale(float timeScale){
        SceneController.Instance.setTimeScale(timeScale);
    }

    public void setResetTimescaleOnNewGen(bool active){
        SceneController.Instance.setResetTimescaleOnNewGen(active);
    }
}

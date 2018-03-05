using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

    [SerializeField] private int reward;
    [SerializeField] private bool finishLine;

    public int getReward(){
        return reward;
    }

    public bool isFinishLine(){
        return finishLine;
    }
}

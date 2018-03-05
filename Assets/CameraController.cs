using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private Transform target;
    public Vector3 offset;
    public Vector3 rotation;
    [Range(0,0.5f)] public float smoothing;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(target != null){
            transform.position = Vector3.Lerp(transform.position, target.position + offset, smoothing);
            transform.eulerAngles = rotation;
        } else {
            transform.position = offset;
            transform.eulerAngles = rotation;
        }
	}

    public void setTarget(Transform target){
        this.target = target;
    }
}

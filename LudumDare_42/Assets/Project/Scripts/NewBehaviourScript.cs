using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
		ListenInputs();

	}

	void ListenInputs() {
		if(Input.GetKeyUp(KeyCode.UpArrow)) {
			Vector3 tempPosition = gameObject.transform.position;
			tempPosition.z += 1;
			gameObject.transform.position = tempPosition;
		}
	}
}

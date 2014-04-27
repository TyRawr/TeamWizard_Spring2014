using UnityEngine;
using System.Collections;

public class TurnMeshOnAfterTime : MonoBehaviour {
	float time = 0f;
	public float threshold = 1f;
	// Use this for initialization
	void Start () {
		GetComponent<MeshRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > threshold)
			GetComponent<MeshRenderer>().enabled = true;
	}
}

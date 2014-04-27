﻿using UnityEngine;
using System.Collections;

public class TriggerAfterTime : MonoBehaviour {
	float time = 0f;
	public float threshold = 1f;
	// Use this for initialization
	void Start () 
	{
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;
		if (time > threshold)
			GetComponent<Animator>().SetBool("Running" , true);
	}
}

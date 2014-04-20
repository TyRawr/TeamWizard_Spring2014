using UnityEngine;
using System.Collections;

public class CameraFollowBall : MonoBehaviour {


	GameObject ball;
	// Use this for initialization
	void Start () {
		ball = GameObject.Find("Ball") as GameObject;
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt(ball.transform.position);
	}
}

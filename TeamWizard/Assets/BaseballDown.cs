using UnityEngine;
using System.Collections;

public class BaseballDown : MonoBehaviour {
	private int currentFrame, down;
	
	// Use this for initialization
	void Start () {
		currentFrame = 0;
		down = 43;
		StartCoroutine(WaitFrames());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	IEnumerator WaitFrames() {
		while(currentFrame < down) {
			currentFrame++;
			float newY = transform.position.y - .6f;
			if(newY < 4) { newY = 4; }
			transform.position = new Vector3(transform.position.x, newY, transform.position.z);
			yield return 0;
		}
	}
}

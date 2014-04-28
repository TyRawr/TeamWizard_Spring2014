using UnityEngine;
using System.Collections;

public class TurnAndJumpHigh : MonoBehaviour {
	private int currentFrame, runAndCrouch, jump;

	// Use this for initialization
	void Start () {
		currentFrame = 0;
		//runAndCrouch = 80;
		//jump = 40;
		runAndCrouch = 40;
		jump = 40;
		StartCoroutine(WaitFrames());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	IEnumerator WaitFrames() {
		while(currentFrame < runAndCrouch) {
			currentFrame++;
			yield return 0;
		}
		while(currentFrame < runAndCrouch + jump) {
			currentFrame++;
			transform.Rotate(new Vector3(0, -3, 0));
			transform.position = new Vector3(transform.position.x, transform.position.y + .3f, transform.position.z);
			yield return 0;
		}

	}
}

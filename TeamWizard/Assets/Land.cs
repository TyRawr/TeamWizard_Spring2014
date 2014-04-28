using UnityEngine;
using System.Collections;

public class Land : MonoBehaviour {
	private int currentFrame, land;
	
	// Use this for initialization
	void Start () {
		currentFrame = 0;
		land = 43;
		StartCoroutine(WaitFrames());
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	IEnumerator WaitFrames() {
		while(currentFrame < land) {
			currentFrame++;
			float newY = transform.position.y - .6f;
			if(newY < 0) { newY = 0; }
			transform.position = new Vector3(transform.position.x, newY, transform.position.z);
			yield return 0;
		}
	}
}

using UnityEngine;
using System.Collections;

public class RunOnKeyPress : MonoBehaviour {

	public KeyCode keyID = KeyCode.C;
	private bool active = false;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( Input.GetKeyDown(keyID) )
		{
			active = !active;
			this.GetComponent<Animator>().SetBool("Running" , active );
		}
	}
}

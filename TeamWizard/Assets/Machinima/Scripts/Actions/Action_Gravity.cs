using UnityEngine;
using System.Collections;

public class Action_Gravity : MonoBehaviour {

	public string actionID;

	private Rigidbody rbody;
	private bool canRun;

	private void Start ()
	{
		//checks to see if there's a rigidbody component attached, if so lets the script run
		rbody = this.gameObject.GetComponent<Rigidbody>();
		if ( rbody != null ) { canRun = true; } else { Debug.Log ( this.name + ": This game object needs a rigidbody component" );}
	}

	public void Trigger_Action (string ID)
	{
		//toggles the gravity on/off for the Game Object this script is attached to
		if ( ID == actionID && canRun )
		{
			if ( rbody.useGravity == false ) { rbody.useGravity = true; }
			else { rbody.useGravity = false; }
		}
	}
}

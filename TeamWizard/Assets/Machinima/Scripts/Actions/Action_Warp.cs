using UnityEngine;
using System.Collections;

public class Action_Warp : MonoBehaviour {

	public string actionID;
	public Vector3 destination;

	public void Trigger_Action (string ID)
	{
		if ( ID == actionID )
		{
			this.transform.position = destination;
		}
	}
}

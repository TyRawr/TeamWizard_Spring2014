/*
 * This script is needed when your event script uses a Trigger_Object cue, but it is located on a seperate game object. 
 * Or when you are using the Event_Locomotion script and are using the Parent Object option.
 * */

using UnityEngine;
using System.Collections;

public class Collision_Connection : MonoBehaviour {

	public GameObject eventController;

	private void OnTriggerEnter (Collider c)
	{
		if (eventController != null)
		{
			eventController.gameObject.SendMessage("OnTriggerEnter", c, SendMessageOptions.DontRequireReceiver);
		}
	}
}

using UnityEngine;
using System.Collections;

public class Action_Rotate : MonoBehaviour {

	public string actionID;

	public Vector3 rotationAngles;
	public float duration;

	private bool isPlaying = false;
	private float rotationTimer = 0;
	
	void Update () 
	{
		if (isPlaying)
		{
			rotationTimer += Time.deltaTime;
			
			if ( rotationTimer < duration )
			{
				this.transform.Rotate(rotationAngles*Time.deltaTime);
			}
			else
			{
				rotationTimer = 0;
				isPlaying = false;
			}
		}
	}

	public void Trigger_Action (string ID)
	{
		if ( ID == actionID )
		{
			isPlaying = true;
		}
	}
}

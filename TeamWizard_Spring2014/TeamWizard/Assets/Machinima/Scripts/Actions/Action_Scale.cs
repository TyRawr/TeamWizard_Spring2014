using UnityEngine;
using System.Collections;

public class Action_Scale : MonoBehaviour {

	public string actionID;

	public Vector3 startScale;
	public Vector3 endScale;
	public float speed;

	private bool isPlaying = false;	
	private float lerpTimer = 0;

	void Update () 
	{
		if ( isPlaying )
		{
			lerpTimer += Time.deltaTime * speed;

			if ( lerpTimer < 1 )
			{
				this.transform.localScale = new Vector3 
					( 
					 Mathf.Lerp(startScale.x,endScale.x,lerpTimer), 
					 Mathf.Lerp(startScale.y,endScale.y,lerpTimer), 
					 Mathf.Lerp(startScale.z,endScale.z,lerpTimer)
					 );
			}
			else
			{
				isPlaying = false;
				lerpTimer = 0;
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

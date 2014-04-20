using UnityEngine;
using System.Collections;

public class Action_LookAt : MonoBehaviour {

	public string actionID;	
	public GameObject lookAtTarget;
	public float lookDuration;
	public float lookSpeed = 1f;

	private bool startLook = false;
	private bool lookLock = false;
	private bool returnLook = false;

	private bool canRun = false;
	private bool isPlaying = false;

	private float lerpTimer = 0;
	private float lookTimer = 0;

	private GameObject lookAtObject;
	private Vector3 startRotation;

	void Start ()
	{
		//checks to make sure there's a look target assigned
		if ( lookAtTarget != null ) { canRun = true; } else { Debug.Log (this.name+": Your Look At Action on is missing a Look At Target.");}

		//set the look at object and parent it to the character
		lookAtObject = new GameObject(this.name+"'s look target");
		lookAtObject.transform.position = this.transform.position;
		lookAtObject.transform.rotation = this.transform.rotation;
		lookAtObject.transform.parent = this.transform;
	}

	//Late update happens at the end of every frame after the other update functions have run. This is needed because it overrides the animation that controls rotation of joints. 
	//This causes a visual glitch when playing back the scene, but will work fine when you render your movie out
	void LateUpdate () 
	{
		if ( isPlaying )
		{
			//if the look is at the end of the duration return the look
			if ( lookTimer > lookDuration ) 
			{ 
				returnLook = true; 
				lookLock = false; 
				startLook = false; 
			}

			else { lookTimer += Time.deltaTime; }

			//Lerps the game object to look at the target at the given look speed
			if ( startLook )
			{
				lookAtObject.transform.LookAt(lookAtTarget.transform.position);

				lerpTimer += Time.deltaTime * lookSpeed;

				this.transform.eulerAngles = new Vector3 
				( 
					 Mathf.LerpAngle(startRotation.x,lookAtObject.transform.eulerAngles.x,lerpTimer), 
					 Mathf.LerpAngle(startRotation.y,lookAtObject.transform.eulerAngles.y,lerpTimer), 
					 Mathf.LerpAngle(startRotation.z,lookAtObject.transform.eulerAngles.z,lerpTimer)
				 );

				if ( lerpTimer > 1 ) { startLook = false; lookLock = true; lerpTimer = 0; }
			}

			//keeps the game object looking at the target
			else if ( lookLock )
			{
				this.transform.LookAt(lookAtTarget.transform.position);	
			}

			//returns the game object to look at the original orientation
			else if ( returnLook )
			{
				lerpTimer += Time.deltaTime * lookSpeed;
				
				this.transform.eulerAngles = new Vector3 
					( 
					 Mathf.LerpAngle(lookAtObject.transform.eulerAngles.x,startRotation.x,lerpTimer), 
					 Mathf.LerpAngle(lookAtObject.transform.eulerAngles.y,startRotation.y,lerpTimer), 
					 Mathf.LerpAngle(lookAtObject.transform.eulerAngles.z,startRotation.z,lerpTimer)
					 );
				
				if ( lerpTimer > 1 ) { returnLook = false; isPlaying = false; lerpTimer = 0;}
			}
		}
	}

	public void Trigger_Action (string ID)
	{
		if ( ID == actionID && canRun )
		{
			//sets the rotation of the look at object & saves the original rotation for when the look is finished
			lookAtObject.transform.LookAt(lookAtTarget.transform.position);
			startRotation = this.gameObject.transform.eulerAngles;
			startLook = true;

			//sets the playing var to true
			isPlaying = true;
		}
	}
}

using UnityEngine;
using System.Collections;

public class Event_Locomotion : MonoBehaviour {
	
	
	public enum CueType
	{
		Start,
		Trigger_Object,
		Keyboard_Input,
		Previous_Event,
	}

	public enum LocoType
	{
		Move,
		Walk,
		Run,
	}

	//Public member variables
	public CueType cueType = CueType.Start;
	public GameObject triggerObject;
	public KeyCode keyboardKey;
	public string eventID;
	public float startDelay;
	public AudioClip soundFile;

	public string parentObjectName;

	public LocoType locomotionType = LocoType.Move;
	public GameObject destinationObject;
	public float turnSpeed = 1f;
	public float moveSpeedOverride;
	public float acceleration;
	public float deceleration;

	public string nextEventID;
	public GameObject[] nextEventObjects;
	
	//Private member variables
	private Animator anim;
	private AudioSource audio;

	private GameObject parentObject;
	
	private bool hasCue = false;
	private bool hasDestination = false;
	private bool canRun = false;
	private bool hasParent = false;
	
	private bool isPlaying = false;
	private bool inTarget = false;
	private float locomotionTimer = 0;
	private string locoTypeString;

	private bool checkDelay = false;
	private float delayTimer = 0;
	
	private float moveVelocity = 0;

	private Vector3 destinationCoordinates;
	private Vector3 startRotation;
	private GameObject lookAtObject;


	void Start () 
	{
		//check if there's a cue for your event
		if ( cueType == CueType.Start){ hasCue = true; checkDelay = true; }
		if ( cueType == CueType.Trigger_Object ) { if ( triggerObject != null ) { hasCue = true; } else { Debug.Log (this.name+": Your Trigger Object field needs a Game Object"); } }
		if ( cueType == CueType.Keyboard_Input ) { if ( keyboardKey != KeyCode.None ) { hasCue = true; } else { Debug.Log (this.name+": Your Keyboard Key cue field needs a value"); } }
		if ( cueType == CueType.Previous_Event ) { if ( eventID.Length > 0) { hasCue = true; } else { Debug.Log (this.name+": Your Event ID field needs a value"); } }

		if ( parentObjectName.Length > 0 ) 
		{ 
			parentObject = GameObject.Find(parentObjectName); 
			if ( parentObject == null ) 
			{ 
				hasParent = false; Debug.Log (this.name+": You have entered a parentObjectName but it doesn't exist in the scene!"); 
			} 
			else 
			{
				hasParent = true;
			}

		} 
		else { hasParent = true; }

		if ( locomotionType == LocoType.Run || locomotionType == LocoType.Walk )
		{
			locoTypeString = (string)locomotionType.ToString();
			if ( parentObject != null )
			{
				anim = parentObject.GetComponent<Animator>();
			}
			else
			{
				anim = this.GetComponent<Animator>();
			}
		}

		if ( destinationObject != null ) { hasDestination = true; destinationCoordinates = destinationObject.transform.position; } else { Debug.Log (this.name+": You need to add a destination object to your locomotion script"); }

		//if all the public properties of the script have been set up, let the script run 
		if ( hasCue && hasDestination && hasParent ) { canRun = true; }

		//set up the sound cue
		if ( soundFile != null )
		{
			audio = (AudioSource)gameObject.AddComponent("AudioSource");
			audio.clip = soundFile;
		}

		if ( parentObject != null )
		{
			//set the look at object and parent it to the character
			lookAtObject = new GameObject(parentObject.name+"'s locomotion look target");
			lookAtObject.transform.position = parentObject.transform.position;
			lookAtObject.transform.rotation = parentObject.transform.rotation;
			lookAtObject.transform.parent = parentObject.transform;
		}
		else
		{
			//set the look at object and parent it to the character
			lookAtObject = new GameObject(this.name+"'s locomotion look target");
			lookAtObject.transform.position = this.transform.position;
			lookAtObject.transform.rotation = this.transform.rotation;
			lookAtObject.transform.parent = this.transform;
		}


		//set the move speed, acceleration and deceleration if the public variables aren't declared

		if ( locomotionType == LocoType.Walk ) 
		{
			if ( acceleration == 0 ) { acceleration = 0.2f; }
			if ( deceleration == 0 ) { deceleration = 0.15f; }
			if ( moveSpeedOverride == 0 ) { moveSpeedOverride = 1.5f; }

		}
		else if ( locomotionType == LocoType.Run ) 
		{
			if ( acceleration == 0 ) { acceleration = 0.2f; }
			if ( deceleration == 0 ) { deceleration = 0.5f; }
			if ( moveSpeedOverride == 0 ) { moveSpeedOverride = 5f; }
		}
		else
		{
			if ( acceleration == 0 ) { acceleration = 0.5f; }
			if ( deceleration == 0 ) { deceleration = 0.5f; }
			if ( moveSpeedOverride == 0 ) { moveSpeedOverride = 5f; }
		}
	}
	
	void Update () 
	{
		if ( canRun && isPlaying == false )
		{
			if ( cueType == CueType.Keyboard_Input && Input.GetKeyDown(keyboardKey) ) 
			{
				if ( startDelay > 0 ) { checkDelay = true; }
				else { Cue_Locomotion (); }
			} 

			if ( checkDelay )
			{
				delayTimer += Time.deltaTime;
				if ( delayTimer > startDelay )
				{
					Cue_Locomotion (); 
				}
			}
		}

		if ( isPlaying ) 
		{
			locomotionTimer += Time.deltaTime * turnSpeed;

			if (inTarget == false)
			{
				lookAtObject.transform.LookAt(destinationCoordinates);

				if ( parentObject != null )
				{
					parentObject.transform.eulerAngles = new Vector3 
						(
							Mathf.LerpAngle(startRotation.x,lookAtObject.transform.eulerAngles.x,locomotionTimer),
							Mathf.LerpAngle(startRotation.y,lookAtObject.transform.eulerAngles.y,locomotionTimer),
							Mathf.LerpAngle(startRotation.z,lookAtObject.transform.eulerAngles.z,locomotionTimer)
							);
				}
				else
				{
					this.transform.eulerAngles = new Vector3 
						(
							Mathf.LerpAngle(startRotation.x,lookAtObject.transform.eulerAngles.x,locomotionTimer),
							Mathf.LerpAngle(startRotation.y,lookAtObject.transform.eulerAngles.y,locomotionTimer),
							Mathf.LerpAngle(startRotation.z,lookAtObject.transform.eulerAngles.z,locomotionTimer)
							);
				}


			}

			if ( moveVelocity < moveSpeedOverride  ) { if ( inTarget == false) {moveVelocity += acceleration;} }
			else if ( moveVelocity > moveSpeedOverride ) { moveVelocity = moveSpeedOverride; }

			if ( inTarget ) 
			{ 
				if (moveVelocity > 0) { moveVelocity -= deceleration; }
				
				else
				{
					moveVelocity = 0;
					canRun = false;
					isPlaying = false;
					Call_Next_Event ();
				}
			}

			if (parentObject != null)
			{
				parentObject.transform.Translate(0,0,moveVelocity * Time.deltaTime);
			}
			else
			{
				this.transform.Translate(0,0,moveVelocity * Time.deltaTime);
			}
		}
	}

	private void Cue_Locomotion () 
	{
		if ( soundFile != null ) { audio.Play(); }
		if ( canRun ) { isPlaying = true; }

		if (parentObject != null)
		{
			startRotation = parentObject.transform.eulerAngles;
			lookAtObject.transform.eulerAngles = parentObject.transform.eulerAngles;
		}
		else
		{
			startRotation = this.transform.eulerAngles;
			lookAtObject.transform.eulerAngles = this.transform.eulerAngles;
		}

		if (locomotionType == LocoType.Walk || locomotionType == LocoType.Run)
		{
			anim.SetBool ( locoTypeString, true );
		}


		lookAtObject.transform.Translate(0,0,0.1f);
		lookAtObject.transform.LookAt(destinationCoordinates);
	}

	private void Stop_Locomotion () 
	{
		if ( locomotionType == LocoType.Walk || locomotionType == LocoType.Run ) 
		{ 
			anim.SetBool( locoTypeString, false );
		}

		inTarget = true;
	}

	private void OnTriggerEnter (Collider c)
	{
		if ( cueType == CueType.Trigger_Object && c.gameObject == triggerObject.gameObject)
		{
			if ( startDelay > 0 ) { checkDelay = true; }
			else { Cue_Locomotion (); }
		}

		if ( c.gameObject == destinationObject.gameObject )
		{
			Stop_Locomotion ();
		}
	}
	
	private void Call_Next_Event () 
	{
		if ( nextEventID.Length > 0)
		{
			if ( nextEventObjects.Length == 0 ) 
			{
				this.gameObject.SendMessage("Cue_Event", nextEventID, SendMessageOptions.DontRequireReceiver);
			}
			else
			{
				foreach (GameObject events in nextEventObjects)
				{
					events.gameObject.SendMessage("Cue_Event", nextEventID, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
	}
	
	public void Cue_Event (string ID) 
	{
		if (eventID == ID && cueType == CueType.Previous_Event) 
		{ 
			if ( startDelay > 0 ) { checkDelay = true; }
			else { Cue_Locomotion (); }
		}
	}
}

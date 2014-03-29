using UnityEngine;
using System.Collections;

public class Event_Action : MonoBehaviour {

	public enum CueType
	{
		Start,
		Trigger_Object,
		Keyboard_Input,
		Previous_Event,
	}

	//Public Cue Variables
	public CueType cueType = CueType.Start;
	public GameObject triggerObject;
	public KeyCode keyboardKey;
	public string eventID;

	//Public Action Variables
	public float startDelay;
	public AudioClip soundFile;

	public string actionID;
	public GameObject[] actionObjects;

	//Private memeber variables
	private AudioSource audio;
	private bool checkDelay;
	private float delayTimer;

	private bool canRun = false;
	private bool hasCue = false;

	void Start () 
	{
		//set up the Audio source and sound file
		if ( soundFile != null )
		{
			audio = (AudioSource)gameObject.AddComponent("AudioSource");
			audio.clip = soundFile;
		}

		//check if there's a cue for your event
		if ( cueType == CueType.Start){ hasCue = true; checkDelay = true; }
		if ( cueType == CueType.Trigger_Object ) { if ( triggerObject != null ) { hasCue = true; } else { Debug.Log (this.name+": Your Trigger Object field needs a Game Object"); } }
		if ( cueType == CueType.Keyboard_Input ) { if ( keyboardKey != KeyCode.None ) { hasCue = true; } else { Debug.Log (this.name+": Your Keyboard Key cue field needs a value"); } }
		if ( cueType == CueType.Previous_Event ) { if ( eventID.Length > 0) { hasCue = true; } else { Debug.Log (this.name+": Your Event ID field needs a value"); } }
	
		//if the event has a cue and an action object the script can run
		if ( hasCue ) { canRun = true; }
	}
	
	void Update () 
	{
		if ( canRun )
		{
			//if the cue type is the keyboard, and the key is pressed...
			if ( cueType == CueType.Keyboard_Input && Input.GetKeyDown(keyboardKey) ) 
			{
				if ( startDelay > 0 ) { checkDelay = true; }
				else { Cue_Action (); }
			}
			
			if ( checkDelay )
			{
				delayTimer += Time.deltaTime;
				if (delayTimer > startDelay)
				{
					Cue_Action ();
					delayTimer = 0;
				}
			}
		}
	}

	//cue the action
	private void Cue_Action ()
	{
		//if no game objects have been specified in the action objecs array, send message to this object
		if ( actionObjects.Length == 0 ) 
		{
			this.gameObject.SendMessage("Trigger_Action",actionID,SendMessageOptions.DontRequireReceiver);
			if ( soundFile != null ) { audio.Play(); }
			canRun = false;
		}
		//otherwise send messages to all the action objects in the array
		else
		{
			foreach (GameObject action in actionObjects)
			{
				if ( action != null ) { action.SendMessage("Trigger_Action",actionID,SendMessageOptions.DontRequireReceiver); }
			}

			if ( soundFile != null ) { audio.Play(); }

			canRun = false;
		}
	}

	//when this object enters a trigger, check the delay and start the action
	private void OnTriggerEnter (Collider c)
	{
		if ( cueType == CueType.Trigger_Object && c.gameObject == triggerObject.gameObject)
		{
			if ( startDelay > 0 ) { checkDelay = true; }
			else { Cue_Action (); }
		}
	}

	//cues an event from a previous event
	public void Cue_Event (string ID) 
	{
		if (eventID == ID && cueType == CueType.Previous_Event) 
		{ 
			if ( startDelay > 0 ) { checkDelay = true; }
			else { Cue_Action (); }
		}
	}
}

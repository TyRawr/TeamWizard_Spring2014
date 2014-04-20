using UnityEngine;
using System.Collections;

public class Event_CameraCut : MonoBehaviour {

	public enum CueType
	{
		Start,
		Trigger_Object,
		Keyboard_Input,
		Previous_Event,
	}

	public CueType cueType = CueType.Start;
	public GameObject triggerObject;
	public KeyCode keyboardKey;
	public string eventID;
	public float startDelay;
	public GameObject cutFromCamera;
	public GameObject cutToCamera;

	private bool checkDelay = false;
	private float delayTimer = 0;

	private Camera camera1;
	private Camera camera2;
	private AudioListener aListener1;
	private AudioListener aListener2;

	private bool canRun = false;
	private bool hasCue = false;
	private bool hasCamera1 = false;
	private bool hasCamera2 = false;

	void Start () 
	{

		//check if there's a cue for your event
		if ( cueType == CueType.Start){ hasCue = true; checkDelay = true; }
		if ( cueType == CueType.Trigger_Object ) { if ( triggerObject != null ) { hasCue = true; } else { Debug.Log (this.name+": Your Trigger Object field needs a Game Object"); } }
		if ( cueType == CueType.Keyboard_Input ) { if ( keyboardKey != KeyCode.None ) { hasCue = true; } else { Debug.Log (this.name+": Your Keyboard Key cue field needs a value"); } }
		if ( cueType == CueType.Previous_Event ) { if ( eventID.Length > 0) { hasCue = true; } else { Debug.Log (this.name+": Your Event ID field needs a value"); } }
		
		if ( cutFromCamera != null )
		{
			cutFromCamera.SetActive(true);

			camera1 = cutFromCamera.GetComponent<Camera>();
			aListener1 = cutFromCamera.GetComponent<AudioListener>();

			if ( camera1 != null && aListener1 != null ) { hasCamera1 = true; }
		}

		if ( cutToCamera != null )
		{
			cutToCamera.SetActive(true);

			camera2 = cutToCamera.GetComponent<Camera>();
			aListener2 = cutToCamera.GetComponent<AudioListener>();
			
			if ( camera2 != null && aListener2 != null ) { hasCamera2 = true; }
		}

		if ( hasCamera1 && hasCamera2 )
		{
			canRun = true;

			if ( camera2.name != "Camera_1" )
			{
				camera2.enabled = false;
				aListener2.enabled = false;
			}

			if ( camera1.name != "Camera_1" )
			{
				camera1.enabled = false;
				aListener1.enabled = false;
			}
		}
	}
	
	void Update () 
	{
		//if the cue type is the keyboard, and the key is pressed...
		if ( cueType == CueType.Keyboard_Input && Input.GetKeyDown(keyboardKey) ) 
		{
			if ( startDelay > 0 ) { checkDelay = true; }
			else { Cut_Camera (); }
		}

		if ( checkDelay )
		{
			delayTimer += Time.deltaTime;
			if ( delayTimer > startDelay )
			{
				Cut_Camera ();
				delayTimer = 0;
			}
		}
	}

	private void Cut_Camera ()
	{
		if ( canRun )
		{
			camera1.enabled = false;
			aListener1.enabled = false;

			camera2.enabled = true;
			aListener2.enabled = true;

			canRun = false;
			checkDelay = false;
		}
	}

	private void OnTriggerEnter (Collider c)
	{
		if ( cueType == CueType.Trigger_Object && c.gameObject == triggerObject.gameObject)
		{
			if ( startDelay > 0 ) { checkDelay = true; }
			else { Cut_Camera (); }
		}
	}

	public void Cue_Event (string ID) 
	{
		if (eventID == ID && cueType == CueType.Previous_Event) 
		{ 
			if ( startDelay > 0 ) { checkDelay = true; }
			else { Cut_Camera (); }
		}
	}
}

using UnityEngine;
using System.Collections;

public class Event_Animation : MonoBehaviour {
	
	
	public enum CueType
	{
		Start,
		Trigger_Object,
		Keyboard_Input,
		Previous_Event,
	}

	//public cue variables
	public CueType cueType = CueType.Start;
	public GameObject triggerObject;
	public KeyCode keyboardKey;
	public string eventID;

	//public animation variables
	public float startDelay;
	public AudioClip soundFile;

	public string parentObjectName;

	public string animatorParameter;
	public float duration;	

	//call next
	public string nextEventID;
	public GameObject[] nextEventObjects;
	
	private Animator animator;
	private AudioSource audio;
	private GameObject parentObject;

	private bool hasCue = false;
	private bool hasAnimator = false;
	private bool hasAnimatorController = false;
	private bool hasDuration = false;
	private bool hasName = false;
	private bool hasParent = false;
	private bool canRun = false;
	
	private bool isPlaying = false;
	private float animationTimer = 0;
	private float delayTimer = 0;
	private bool checkDelay = false;

	
	void Start () 
	{
		//check if there's a cue for your animation
		if ( cueType == CueType.Start ){ hasCue = true; checkDelay = true; }
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

		//make sure this game object has the animator, animator controllor components and parameter name
		if ( parentObject != null )
		{
			if ( parentObject.GetComponent<Animator>() != null ) { animator = parentObject.GetComponent<Animator>(); hasAnimator = true; } else { Debug.Log (parentObject.name+": To trigger an animation you'll need an Animator component" ); }
		}
		else
		{
			if ( this.GetComponent<Animator>() != null ) { animator = this.GetComponent<Animator>(); hasAnimator = true; } else { Debug.Log (this.name+": To trigger an animation you'll need an Animator component" ); }
		}

		if ( hasAnimator && animator.runtimeAnimatorController != null ) { hasAnimatorController = true; } else { Debug.Log (this.name+": To trigger an animation you need to drop in an Animator Controller"); }
		if ( animatorParameter.Length > 0 ) { hasName = true; } else { Debug.Log (this.name+": You need to enter a parameter name that corresponds to a parameter in your Animator Controller"); } 
	
		if ( duration > 0 ) { hasDuration = true; } else { Debug.Log (this.name+": You need a duration in order for the animation to play"); }

		//if all the public properties of the script have been set up, let the script run 
		if ( hasCue && hasAnimator && hasAnimatorController && hasDuration && hasName && hasParent ) { canRun = true; }

		if ( soundFile != null )
		{
			audio = (AudioSource)gameObject.AddComponent("AudioSource");
			audio.clip = soundFile;
		}		
	}
	
	void Update () 
	{
		if ( canRun && isPlaying == false )
		{
			if ( cueType == CueType.Keyboard_Input && Input.GetKeyDown(keyboardKey) ) 
			{
				Cue_Animation ();
			} 

			if ( checkDelay )
			{
				delayTimer += Time.deltaTime;
				if ( delayTimer > startDelay )
				{
					Cue_Animation ();
				}
			}
		}

		if ( isPlaying )
		{
			if ( animationTimer < duration ) 
			{ 
				animationTimer += Time.deltaTime; 
			} 
			else 
			{ 
				animator.SetBool( animatorParameter, false );
				isPlaying = false; 
				canRun = false;
				Call_Next_Event ();
			}
		}
	}

	private void Cue_Animation ()
	{
		if ( canRun ) 
		{ 
			isPlaying = true;
			animator.SetBool( animatorParameter, true ); 	
		}

		if ( soundFile != null ) { audio.Play(); }
	}

	private void OnTriggerEnter (Collider c)
	{
		if ( cueType == CueType.Trigger_Object && c.gameObject == triggerObject.gameObject)
		{
			if ( startDelay > 0 ) { checkDelay = true; }
			else { Cue_Animation (); }
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
			else { Cue_Animation (); }
		}
	}
}

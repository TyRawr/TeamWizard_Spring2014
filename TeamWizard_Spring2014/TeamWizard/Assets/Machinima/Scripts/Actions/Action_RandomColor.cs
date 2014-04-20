using UnityEngine;
using System.Collections;

public class Action_RandomColor : MonoBehaviour {

	public string actionID;
	public float duration;
	public float rate;

	private Material mat;
	private bool isPlaying;
	private bool canRun;

	private float durationTimer;
	private float rateTimer;

	void Start () 
	{
		//checks to see if there is a material attached to the game object
		mat = this.gameObject.renderer.material;
		if (mat != null ) {canRun = true;}
	}
	
	void Update () 
	{
		if ( isPlaying )
		{
			durationTimer += Time.deltaTime;
			rateTimer += Time.deltaTime;

			//switches the color to a new random color
			if (rateTimer > rate)
			{
				mat.color = new Color (Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f),1);
				rateTimer = 0;
			}

			//turns off the script
			if (durationTimer>duration)
			{
				isPlaying = false;
				durationTimer = 0;
				rateTimer = 0;
			}
		}
	}

	public void Trigger_Action (string ID)
	{
		if ( ID == actionID && canRun )
		{
			isPlaying = true;
		}
	}
}

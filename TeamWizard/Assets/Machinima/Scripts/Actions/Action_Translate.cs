using UnityEngine;
using System.Collections;

public class Action_Translate : MonoBehaviour {

	public string actionID;
	public Vector3 translationDirection;
	public float duration;

	public float easeIn;
	public float easeOut;

	private float translationTimer = 0;
	private Vector3 translationVector;

	private bool isPlaying = false;
	private bool easingIn = false;
	private bool translating = false;
	private bool easingOut = false;

	void Start ()
	{
		if (easeIn > 0) { easingIn = true; } else { translating = true; translationVector = translationDirection; }

	}

	void Update () 
	{
		if (isPlaying)
		{
			if ( easingIn )
			{
				translationTimer += Time.deltaTime;

				if (translationTimer < 1)
				{
					translationVector.x = Mathf.Lerp(0,translationDirection.x, translationTimer);
					translationVector.y = Mathf.Lerp(0,translationDirection.y, translationTimer);
					translationVector.z = Mathf.Lerp(0,translationDirection.z, translationTimer);
				}
				else
				{
					easingIn = false;
					translating = true;
					translationTimer = 0;
					translationVector = translationDirection; 
				}

				this.transform.Translate(translationVector*Time.deltaTime);
			}

			if ( translating )
			{
				translationTimer += Time.deltaTime;

				if (translationTimer<duration)
				{
					this.transform.Translate(translationVector*Time.deltaTime);
				}
				else
				{
					if (easeOut > 0 )
					{
						easingOut = true;
					}
					else
					{
						isPlaying = false;
					}

					translating = false;
					translationTimer = 0;
				}

				this.transform.Translate(translationVector*Time.deltaTime);
			}

			if (easingOut)
			{
				translationTimer += Time.deltaTime;
				
				if (translationTimer < 1)
				{
					translationVector.x = Mathf.Lerp(translationDirection.x, 0,translationTimer);
					translationVector.y = Mathf.Lerp(translationDirection.y, 0, translationTimer);
					translationVector.z = Mathf.Lerp(translationDirection.z, 0, translationTimer);
				}
				else
				{
					easingOut = false;
					isPlaying = false;
					translationTimer = 0;
				}
				
				this.transform.Translate(translationVector*Time.deltaTime);
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

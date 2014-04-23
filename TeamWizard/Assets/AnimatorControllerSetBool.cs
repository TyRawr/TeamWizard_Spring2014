using UnityEngine;
using System.Collections;

public class AnimatorControllerSetBool : MonoBehaviour 
{
	public string boolName = "";
	public bool boolValue = false;
	// Use this for initialization
	void Start () {
		this.GetComponent<Animator>().SetBool(boolName /* "Talking" */, boolValue /* true or false */ );
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

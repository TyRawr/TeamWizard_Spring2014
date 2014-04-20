using UnityEngine;
using System.Collections;


public class RandomCrowdAnimation : MonoBehaviour {
    string[] animationList = { "idle", "applause", "applause2", "celebration", "celebration2" , "celebration3" };
	// Use this for initialization
	void Start () {
        Debug.Log("Jere");
        Random r = new Random();
        int rand = Random.Range( 0 , 5 );
	    animation.Play( animationList[ rand ].ToString() );
        
	}
	
	// Update is called once per frame
	void Update () {
        if (!animation.isPlaying)   // this never gets run because all the audience animations are set to loop
        {
            Random r = new Random();
            int rand = Random.Range(0, 5);
            animation.Play(animationList[rand].ToString());
        }
	}
}

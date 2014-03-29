using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class Cameraman : MonoBehaviour {

	public bool Enable_Parenting;
	public GameObject Parent;
	public Vector3 Distance_From_Parent;

	public bool Enable_Look_At;
	public GameObject lookAtTarget;

	public float Framing;
	public float Leading;
	public float Dutch;
	public float Zoom = 45;

	private string paramID;

	void Update () 
	{

		if (Enable_Parenting && Parent != null) 
		{ 
			this.transform.position = new Vector3
				(
					this.Parent.transform.position.x + Distance_From_Parent.x,
					this.Parent.transform.position.y + Distance_From_Parent.y,
					this.Parent.transform.position.z + Distance_From_Parent.z
					);
		}
		
		if (Enable_Look_At && lookAtTarget != null )
		{
			this.transform.LookAt(lookAtTarget.transform.position);	
			this.transform.eulerAngles = new Vector3 (this.transform.eulerAngles.x + Framing, this.transform.eulerAngles.y + Leading, this.transform.eulerAngles.z + Dutch );

		}


		this.camera.fieldOfView = Zoom;

	}

	public void Recieve_Message (string parameterID)
	{
		Debug.Log (parameterID);
		paramID = parameterID;
	}

	public void Recieve_Value ()
	{
		
	}
}

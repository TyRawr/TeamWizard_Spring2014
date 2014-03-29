using UnityEngine;
using System.Collections;

public class Action_Visibility : MonoBehaviour {
	
	public string actionID;

	public void Trigger_Action (string ID)
	{
		if ( ID == actionID )
		{
			SkinnedMeshRenderer[] childSkinMeshes = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

			foreach ( SkinnedMeshRenderer mr in childSkinMeshes )
			{
				if (mr.enabled == false )
				{
					mr.enabled = true;
				}
				else
				{
					mr.enabled = false;
				}
			}

			MeshRenderer[] childMeshes = gameObject.GetComponentsInChildren<MeshRenderer>();
			
			foreach ( MeshRenderer mr in childMeshes )
			{
				if (mr.enabled == false )
				{
					mr.enabled = true;
				}
				else
				{
					mr.enabled = false;
				}
			}
		}
	}
}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mixamo;

public class FacePlusExtras : MonoBehaviour {

	public Transform HeadJoint;
	public Transform JawJoint;
	public float MouthOpenJawValue;
	public int headJointSmoothingSteps = 1;
	public SkinnedMeshRenderer FaceMesh;
	public SkinnedMeshRenderer TeethMesh;
	public Transform PointerJoint;

	private Vector3 startingJaw;

	private Vector3 startingHead;

	private Queue<Quaternion> previousRotations;

	private int mouthOpenIndex = 0;
	private int smileLeftIndex = 0;
	private int smileRightIndex = 0;

#if UNITY_4_3

	// Use this for initialization
	void Start () {
		previousRotations = new Queue<Quaternion> ();
		previousRotations.Enqueue (HeadJoint.localRotation);

		startingJaw = JawJoint.localEulerAngles;
		startingHead = HeadJoint.localEulerAngles;

		for(int i=0; i<FaceMesh.sharedMesh.blendShapeCount; i++) {
			string name = FaceMesh.sharedMesh.GetBlendShapeName (i);
			switch (name) {
			case "Facial_Blends.MouthOpen":
				mouthOpenIndex = i;
				break;
			case "Facial_Blends.Smile_Left":
				smileLeftIndex = i;
				break;
			case "Facial_Blends.Smile_Right":
				smileRightIndex = i;
				break;
			}
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		//if (FaceCapture.Instance == null || FaceCapture.Instance.channelMapping == null) return;

//		float mouthOpenVal = FaceCapture.Instance.channelMapping["Mix::MouthOpen"].Amount;
//		float smileLeftVal = FaceCapture.Instance.channelMapping ["Mix::Smile_Left"].Amount;
//		float smileRightVal = FaceCapture.Instance.channelMapping ["Mix::Smile_Right"].Amount;
		float mouthOpenVal = FaceMesh.GetBlendShapeWeight (mouthOpenIndex);
		float smileLeftVal = FaceMesh.GetBlendShapeWeight (smileLeftIndex);
		float smileRightVal = FaceMesh.GetBlendShapeWeight (smileRightIndex);
		float JawRot = mouthOpenVal / 100f * MouthOpenJawValue;
		JawJoint.localRotation = Quaternion.Euler (startingJaw + (new Vector3 (JawRot+5, 0f, 0f)));
		TeethMesh.SetBlendShapeWeight (0, smileLeftVal);
		TeethMesh.SetBlendShapeWeight (1, smileRightVal);
		PointerJoint.localRotation = Quaternion.Euler (319.1f, 248.173f, 180f);

		if (previousRotations.Count >= headJointSmoothingSteps) {
			previousRotations.Dequeue ();
		}

		previousRotations.Enqueue (HeadJoint.localRotation);
//
//		Quaternion result = HeadJoint.localRotation;
//		for (int i=previousRotations.Count-1; i>=0; i--) {
//			result = Quaternion.Lerp (HeadJoint.localRotation, previousRotations.ElementAt (i), i/previousRotations.Count);
//		}


		HeadJoint.localRotation = previousRotations.Aggregate ((a, b) => {
			return Quaternion.Lerp (a, b, 0.5f);
		});


		//HeadJoint.localRotation = Quaternion.Lerp(HeadJoint.localRotation, curMinus1, 0.5f);
		//curMinus1 = HeadJoint.localRotation;

	}

#endif
}

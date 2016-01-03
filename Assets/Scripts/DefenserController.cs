using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

//struct BodyJoints {
//	public Transform hips;
//	public Transform spine;
//	public Transform neck;
//	public Transform head;
//
//	public Transform leftShoulder;
//	public Transform leftUpperArm;
//	public Transform leftLowerArm;
//	public Transform leftHand;
//	public Transform leftIndexProximal;
//
//	public Transform rightShoulder;
//	public Transform rightUpperArm;
//	public Transform rightLowerArm;
//	public Transform rightHand;
//	public Transform rightIndexProximal;
//}

public struct Joint {
	public Vector3 position;
	public Quaternion rotation;
}

public class DefenserController : MonoBehaviour {

	public Animator enemy;

	public bool isRecording = false;
	public bool isReacting = false;
	public Button record;

	public int numJoints = 18;
	protected List<Joint[]> bones = new List<Joint[]>();
//	private HumanBodyBones enemyJoints;

	void Awake() {
		if (numJoints > 18) {
			numJoints = 18;
		}
	}

	void Start () {
		record.image.color =  Color.white;
	}
	
	// Update is called once per frame
	void Update () {
		if (isRecording) {
			Joint[] joints = new Joint[numJoints];
			for (int i = 0; i < numJoints; i++) {
				Transform tmp = enemy.GetBoneTransform (boneIndex [i]);
				joints [i] = new Joint {
					position = new Vector3 (tmp.position.x, tmp.position.x, tmp.position.z),
					rotation = new Quaternion (tmp.rotation.x, tmp.rotation.y, tmp.rotation.z, tmp.rotation.w),
				};
			}
			bones.Add (joints);
		}
	}

	void LateUpdate() {
//		Transform trans = enemy.GetBoneTransform (boneIndex [4]) ;
//		trans.rotation = new Quaternion ();

		if (isReacting) {
			isReacting = false;
		
			StartCoroutine ("Playback");
		}
	}

	public void toggleRecord() {
		if (isRecording) {
			record.image.color =  Color.white;
			Text[] textEle = record.GetComponentsInChildren<Text>();
			textEle[0].text = "Record";

			isRecording = false;
			startAnalysis ();
		} else {
			bones.Clear ();
			isRecording = true;
			record.image.color =  Color.red;
			Text[] textEle = record.GetComponentsInChildren<Text>();
			textEle[0].text = "Recording...";
		}
	}

	private void startAnalysis() {
		attackPathAnalysis ();
		defenseStrategyAnalysis ();
	}

	void OnCollisionEnter(Collision collision) {
		Debug.Log ("hit");
		// Debug-draw all contact points and normals
//		foreach (ContactPoint contact in collision.contacts) {
//			Debug.DrawRay(contact.point, contact.normal, Color.white);
//		}
//
//		// Play a sound if the colliding objects had a big impact.		
//		if (collision.relativeVelocity.magnitude > 2)
//			audio.Play();

	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		Debug.Log ("hit");

//		Rigidbody body = hit.collider.attachedRigidbody;
//		if (body == null || body.isKinematic)
//			return;
//
//		if (hit.moveDirection.y < -0.3F)
//			return;
//
//		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
//		body.velocity = pushDir * pushPower;
	}

	private void attackPathAnalysis() {

		// 1. Find enemy attack from right or left

		// 2. Find the coordinate that attacked. (coordinate origin is ???)

		// 3. Find the parts that will be attacked.
		foreach (Joint[] joints in bones) {
//			Debug.Log ("LeftShoulder" + joints [4].rotation.eulerAngles);
			Debug.Log ("LeftShoulder" + joints [4].position);

		}

		foreach (Joint[] joints in bones) {
//			Debug.Log ("LeftUpperArm" + joints [5].rotation.eulerAngles);
			Debug.Log ("LeftUpperArm" + joints [5].position);
		}

		foreach (Joint[] joints in bones) {
//			Debug.Log ("LeftLowerArm" + joints [6].rotation.eulerAngles);
			Debug.Log ("LeftLowerArm" + joints [6].position);
		}

		foreach (Joint[] joints in bones) {
//			Debug.Log ("LeftHand" + joints [7].rotation.eulerAngles);
			Debug.Log ("LeftHand" + joints [7].position.x);
		}

//		foreach (Joint[] joints in bones) {
//			Debug.Log ("LeftIndexProximal" + joints [7].rotation.eulerAngles);
//		}

//		foreach(Joint[] joints in bones) {
//			Debug.Log ("LeftShoulder" + joints [4].rotation.eulerAngles);
//			Debug.Log ("LeftUpperArm" + joints [5].rotation.eulerAngles);
//			Debug.Log ("LeftLowerArm" + joints [6].rotation.eulerAngles);
//			Debug.Log ("RightHand" + joints [7].rotation.eulerAngles);
//			Debug.Log ("LeftIndexProximal" + joints [8].rotation.eulerAngles);

			//			for (int j = 0; j < joints.Length; j++) {
			//				Transform trans = enemy.GetBoneTransform (boneIndex [j]) ;
			//				trans.rotation = new Quaternion ();
			//
			//				trans.position = joints [j].position;
			//				trans.rotation = joints [j].rotation;
			//			}
//		}
	}

	private void defenseStrategyAnalysis() {
		
	}

	IEnumerator Playback ()
	{
		foreach(Joint[] joints in bones) {
			for (int j = 0; j < joints.Length; j++) {
				Transform trans = enemy.GetBoneTransform (boneIndex [j]) ;
				trans.rotation = new Quaternion ();

				trans.position = joints [j].position;
				trans.rotation = joints [j].rotation;
			}

			Debug.Log (joints [5].rotation.eulerAngles);
			Debug.Log (enemy.GetBoneTransform (boneIndex [5]).rotation.eulerAngles);

			yield return null;
		}

	}

	private readonly Dictionary<int, HumanBodyBones> boneIndex = new Dictionary<int, HumanBodyBones>
	{
		{0, HumanBodyBones.Hips},
		{1, HumanBodyBones.Spine},
		{2, HumanBodyBones.Neck},
		{3, HumanBodyBones.Head},

		{4, HumanBodyBones.LeftShoulder}, 
		{5, HumanBodyBones.LeftUpperArm}, 
		{6, HumanBodyBones.LeftLowerArm}, 
		{7, HumanBodyBones.LeftHand}, 
//		{8, HumanBodyBones.LeftIndexProximal},

		{8, HumanBodyBones.RightShoulder},
		{9, HumanBodyBones.RightUpperArm},
		{10, HumanBodyBones.RightLowerArm},
		{11, HumanBodyBones.RightHand},
//		{13, HumanBodyBones.RightIndexProximal},

		{12, HumanBodyBones.LeftUpperLeg},
		{13, HumanBodyBones.LeftLowerLeg},
		{14, HumanBodyBones.LeftFoot},
//		{15, HumanBodyBones.LeftToes},

		{15, HumanBodyBones.RightUpperLeg},
		{16, HumanBodyBones.RightLowerLeg},
		{17, HumanBodyBones.RightFoot},
//		{19, HumanBodyBones.RightToes},
	};
}
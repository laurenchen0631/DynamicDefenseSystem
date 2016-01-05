using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public struct Joint {
	public Vector3 position;
	public Quaternion rotation;
}
	
public enum Direction {
	Left,
	Middle,
	Right,

	Upper,
	Center,
	Lower,

	Front,
	Back,
}

public enum DenfenseAction {
	// Block middle
	BlockMiddleHigh = 1, // in 15-36 frame
	BlockMiddleLow, // in 26-46 frame

	// Block left
	DodgeRight, // in 18-46 frame
	DuckRight, // in 20-40 frame
	BlockLeftHigh, // in 24-32 frame
	BlockLeftMiddle, // in 20-36 frame
	BlockLeftLow,  // in 24-37 frame

	// Block right
	DodgeLeft, // in 18-46 frame
	DuckLeft, // in 30-50 frame
	BlockRightHigh, // 19-40
	BlockRightMiddle, // 24-30
	BlockRightLow, // 30 - 46
}

public class DefenserController : MonoBehaviour {

	public Animator enemy;

	private Animator anim;

	public bool isRecording = false;
	public bool isReacting = false;
	public Button record;

	public int numJoints = 18;


	public Transform defenseOrigin;
	public float defenseThreshold = 0.025f;
	protected List<Joint[]> bones = new List<Joint[]>();
	protected bool hasHitPoint = false;
	protected Vector3 hitPoint;


	void Awake() {
		if (numJoints > 18) {
			numJoints = 18;
		}
	}

	void Start () {
		record.image.color =  Color.white;
		anim = GetComponent<Animator> ();

		AddTagRecursively (enemy.gameObject.transform, "Attacker");
		AddTagRecursively (gameObject.transform, "Defenser");
	}
	
	// Update is called once per frame
	void Update () {
		if (isRecording) {
			Joint[] joints = new Joint[numJoints];
			for (int i = 0; i < numJoints; i++) {
				Transform tmp = enemy.GetBoneTransform (boneIndex [i]);
				joints [i] = new Joint {
					position = new Vector3 (tmp.position.x, tmp.position.y, tmp.position.z),
					rotation = new Quaternion (tmp.rotation.x, tmp.rotation.y, tmp.rotation.z, tmp.rotation.w),
				};
			}
			bones.Add (joints);
		}
	}

	void LateUpdate() {
		if (isReacting) {
			isReacting = false;
		
			StartCoroutine ("Playback");
		}
	}

	void FixedUpdate() {
		if (Input.GetKeyUp (KeyCode.R)) {
			toggleRecord ();
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
			hasHitPoint = false;
			record.image.color =  Color.red;
			Text[] textEle = record.GetComponentsInChildren<Text>();
			textEle[0].text = "Recording...";
		}
	}

	private void startAnalysis() {
		attackPathAnalysis ();
		defenseStrategyAnalysis ();
	}

	void OnCollisionEnter(Collision hit) {
		if (
			hit.gameObject.tag == "Attacker" &&
			hit.gameObject.GetComponentInParent<AttackerController>().isAttacking &&
			! anim.GetCurrentAnimatorStateInfo(0).IsName("DAMAGED") 
		) {
			if (isRecording) {
//				Debug.Log (hasHitPoint);
				if (!hasHitPoint) {
					hasHitPoint = true;
					hitPoint = hit.contacts [0].point;
//					Debug.Log (getPosition(hit.contacts [0].point));
				}
			} else {
				anim.SetTrigger ("hit");
			}
		}
	}
		
	private void attackPathAnalysis() {

//		for (int i = 1; i < bones.Count; i++) {
//			Debug.DrawLine (bones [i-1] [7].position, bones [i] [7].position, Color.red, 60);
//		}

		int lastIndex = 0;
		Direction side = Direction.Left;

		// 1. Find enemy attack from right or left
		findAttackPart (ref lastIndex, ref side); //If find acttacker side, hasHitPoint muse be true;
		if (!hasHitPoint) {
			return;
		}

//		int jointsIndex = side == Direction.Left ? 7 : 11;
		Debug.Log ("The " + lastIndex + "th frame recorded in total " + bones.Count + " frames.");
		Debug.Log ("Attacked by " + side + " hand.");

		// 2. Find the coordinate that attacked. (coordinate origin is ???)

		// 3. Find the parts that will be attacked.

	}

	private void findAttackPart(ref int index, ref Direction dir) {
		
		if (hasHitPoint) {
			float min = float.MaxValue;
//			Debug.Log ("Contact Point: " + getPosition (hitPoint));

			for (int i = 0; i < bones.Count; i++) {
				float leftHand2Contact = Vector3.Distance (bones [i] [7].position, hitPoint);
				float rightHand2Contact = Vector3.Distance (bones [i] [11].position, hitPoint);

				float localMin = leftHand2Contact > rightHand2Contact ? rightHand2Contact : leftHand2Contact;
				if (localMin < min) {
					min = localMin;
					index = i;
					dir = leftHand2Contact > rightHand2Contact ? Direction.Right : Direction.Left;
				}
			}

//			int jointsIndex = side == Direction.Left ? 7 : 11;
//			Debug.Log ("close point: " + getPosition ( bones [lastIndex] [jointsIndex].position));
//			Debug.Log ("The " + lastIndex + "th frame recorded in total " + bones.Count + " frames.");
//			Debug.Log (side);
		} else {
			float min = defenseThreshold;
//			int lastIndex = 0;
//			Direction side = Direction.Left;

			for (int i = 1; i < bones.Count; i++) {

				RaycastHit leftHandHit;
				float leftHandDistance = defenseThreshold;
				if (
					Physics.Raycast (
						bones [i-1] [7].position, 
						bones [i] [7].position - bones [i-1] [7].position,
						out leftHandHit,
						defenseThreshold
					)
				) {
					if (leftHandHit.collider.tag == "Defenser") {
//						Debug.Log (leftHandHit.collider.name);
//						Debug.Log (getPosition (leftHandHit.point));
//						Debug.DrawLine (bones [i] [7].position, leftHandHit.point, Color.red, 60);
						leftHandDistance = Vector3.Distance (leftHandHit.point, bones [i] [7].position);
					}
				}

				RaycastHit rightHandHit;
				float rightHandDistance = defenseThreshold;
				if (
					Physics.Raycast (
						bones [i-1] [11].position, 
						bones [i] [11].position - bones [i-1] [11].position,
						out rightHandHit,
						defenseThreshold
					)
				) {
					if (rightHandHit.collider.tag == "Defenser") {
						//						Debug.Log (rightHandHit.collider.name);
						//						Debug.Log (getPosition (rightHandHit.point));
						//						Debug.DrawLine (bones [i] [11].position, rightHandHit.point, Color.red, 60);
						rightHandDistance = Vector3.Distance (rightHandHit.point, bones [i] [11].position);
					}
				}

				float localMin = leftHandDistance > rightHandDistance ? rightHandDistance : leftHandDistance;
				if (localMin < min) {
					min = localMin;
					index = i;
					dir = leftHandDistance > rightHandDistance ? Direction.Right : Direction.Left;
					hitPoint = leftHandDistance > rightHandDistance ? rightHandHit.point : leftHandHit.point;
					hasHitPoint = true;
				}
			}

//			if (min < defenseThreshold) {
//				int jointsIndex = side == Direction.Left ? 7 : 11;
//				Debug.Log ("close point: " + getPosition ( bones [lastIndex] [jointsIndex].position));
//				Debug.Log ("The " + lastIndex + "th frame recorded in total " + bones.Count + " frames.");
//				Debug.Log (side);
//			}
		}
	}
		
	private void defenseStrategyAnalysis() {
		
	}

	public void AddTagRecursively(Transform trans, string tag)
	{
		trans.gameObject.tag = tag;
		if(trans.childCount > 0) {
			foreach(Transform t in trans) AddTagRecursively(t, tag);
		}
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

	public string getPosition(Vector3 pos) {
//		Vector3 pos = trans.position;

		return "( " + pos.x + ", " + pos.y + ", " + pos.z + ")";
	}
}
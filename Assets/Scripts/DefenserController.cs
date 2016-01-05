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
//	DuckRight, // in 20-40 frame
	BlockLeftHigh, // in 24-32 frame
	BlockLeftMiddle, // in 20-36 frame
	BlockLeftLow,  // in 24-37 frame

	// Block right
	DodgeLeft, // in 18-46 frame
//	DuckLeft, // in 30-50 frame
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
	public float sideThreshold = 0.1f;
	public float centerThreshold = 0.1f;

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
		if (Input.GetKeyDown (KeyCode.R)) {
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
//		defenseStrategyAnalysis ();
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

		// Too few recorded frames
		if (bones.Count < 3) {
			return;
		}

		int lastIndex = 0;
		Direction attackSide = Direction.Left;

		// 1. Find enemy attack from right or left
		findAttackPart (ref lastIndex, ref attackSide); //If find acttacker side, hasHitPoint muse be true;
		if (!hasHitPoint) {
			return;
		}

		Debug.Log ("The " + lastIndex + "th frame recorded in total " + bones.Count + " frames.");
		Debug.Log ("Attacked by " + attackSide + " hand.");

		// 2. Find the coordinate that attacked. (coordinate origin is defenseOrigin)

		int jointsIndex = attackSide == Direction.Left ? 7 : 11;
		Vector3 firstPoint = defenseOrigin.InverseTransformPoint (bones [0] [jointsIndex].position);
//		Vector3 middlePoint = defenseOrigin.InverseTransformPoint (bones [Mathf.FloorToInt(lastIndex / 2f)] [jointsIndex].position);
		Vector3 lastPoint = defenseOrigin.InverseTransformPoint (hitPoint);


		Direction side = Direction.Middle;
		Direction height = Direction.Center;

		if (
			firstPoint.y < -centerThreshold 
		) {
			height = Direction.Lower;
		} else if (
			firstPoint.y > centerThreshold 
		) {
			height = Direction.Upper;
		}

		if (
			firstPoint.x < -sideThreshold
		) {
			side = Direction.Left;
		} else if (
			firstPoint.x > sideThreshold
		) {
			side = Direction.Right;
		}

		Debug.Log (getPosition (firstPoint));
//		Debug.Log (side.ToString ());

		defenseStrategyAnalysis (side, height);

		// if isAbove choose high 
		// if isBelow choose low
		// else choose middle


		// if isRight choose right 
		// if isLeft choose left
		// else choose middle 
		
//		Debug.Log (getPosition (firstPoint));
//		Debug.Log (getPosition (middlePoint));
//		Debug.Log (getPosition (lastPoint));

//		Debug.DrawLine (defenseOrigin.position + firstPoint, defenseOrigin.position + middlePoint, Color.blue, 60f);
		Debug.DrawLine (defenseOrigin.position + firstPoint, defenseOrigin.position + lastPoint, Color.blue, 60f);

//		for (int i = 0; i < lastIndex; i++) {
			
//		}

		// 3. Find the parts that will be attacked.

	}

	private void findAttackPart(ref int index, ref Direction dir) {
		
		if (hasHitPoint) {
			float min = float.MaxValue;
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
		} else {
			float min = defenseThreshold;

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
		}
	}
		
	private void defenseStrategyAnalysis(Direction side, Direction height) {

		Debug.Log (side.ToString () + ", " + height.ToString ());

		if (
			side == Direction.Middle &&
			(height == Direction.Center || height == Direction.Upper)
		) {
			// use BlockMiddleHigh in frame lastIndex - 15 
			anim.Play ("BlockMiddleHigh", 0);
		} else if (
			side == Direction.Middle &&
			height == Direction.Lower
		) {
			// use BlockMiddleLow in frame lastIndex - 24
			anim.Play ("BlockMiddleLow", 0);
		} else if (
			side == Direction.Left &&
			height == Direction.Upper
		) {
			// use BlockLeftHigh/DodgeRight in frame lastIndex - 24/18	
			anim.Play ("DodgeRight", 0);
		}else if (
			side == Direction.Left &&
			height == Direction.Center
		) {
			//  use BlockLeftMiddle in frame lastIndex - 20
			anim.Play ("BlockLeftMiddle", 0);
		} else if (
			side == Direction.Left &&
			height == Direction.Lower
		) {
			// use BlockLeftLow in frame lastIndex - 24
			anim.Play ("BlockLeftLow", 0);
		} else if (
			side == Direction.Right &&
			height == Direction.Upper
		) {
			// use BlockRightHigh/DodgeLeft in frame lastIndex - 18
			anim.Play ("DodgeLeft", 0);
		}else if (
			side == Direction.Right &&
			height == Direction.Center
		) {
			// use BlockRightMiddle in frame lastIndex - 24
			anim.Play ("BlockRightMiddle", 0);
		} else if (
			side == Direction.Right &&
			height == Direction.Lower
		) {
			// use BlockRightLow in frame lastIndex - 30
			anim.Play ("BlockRightLow", 0);
		}
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

//			Debug.Log (joints [5].rotation.eulerAngles);
//			Debug.Log (enemy.GetBoneTransform (boneIndex [5]).rotation.eulerAngles);

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
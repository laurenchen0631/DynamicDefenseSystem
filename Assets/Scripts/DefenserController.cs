using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DefenserController : MonoBehaviour {

	public Animator enemy;
//	private HumanBodyBones enemyJoints;

	// Use this for initialization
	void Start () {
//		enemy = 
//		Animator anmi = enemy.GetComponents<Animator> () as Animator;
//		enemyJoints 
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (enemy.GetBoneTransform (boneIndex[0]).position);

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
		{8, HumanBodyBones.LeftIndexProximal},

		{9, HumanBodyBones.RightShoulder},
		{10, HumanBodyBones.RightUpperArm},
		{11, HumanBodyBones.RightLowerArm},
		{12, HumanBodyBones.RightHand},
		{13, HumanBodyBones.RightIndexProximal},

//		{14, HumanBodyBones.LeftUpperLeg},
//		{15, HumanBodyBones.LeftLowerLeg},
//		{16, HumanBodyBones.LeftFoot},
//		{17, HumanBodyBones.LeftToes},
//
//		{18, HumanBodyBones.RightUpperLeg},
//		{19, HumanBodyBones.RightLowerLeg},
//		{20, HumanBodyBones.RightFoot},
//		{21, HumanBodyBones.RightToes},
	};
}
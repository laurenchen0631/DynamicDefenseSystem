using UnityEngine;
using System.Collections;

public class AttackerController : MonoBehaviour {

	private Animator anim;
	private AnimatorStateInfo currentState;
	private AnimatorStateInfo previousState;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

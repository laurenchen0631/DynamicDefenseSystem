using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AttackerController : MonoBehaviour {

	private Animator anim;
	private AnimatorStateInfo currentState;
	private AnimatorStateInfo previousState;

	public Dropdown actionDirection;
	public Dropdown action;
	public Transform target;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		anim.SetBool ("Left", true);
		anim.SetInteger ("Action", 0);

		actionDirection.onValueChanged.AddListener (delegate {
			onChangeDirection ();
		});
		action.onValueChanged.AddListener (delegate {
			onChangeAction ();
		});
	}
	
	// Update is called once per frame
	void Update () {
		if (
			anim.IsInTransition (0) &&
			anim.GetNextAnimatorStateInfo (0).fullPathHash == Animator.StringToHash ("Base Layer.Idle")
		) {
			action.value = 0;
			anim.SetInteger ("Action", 0);
		}
	}

	void FixedUpdate() {
		if (anim.GetInteger ("Action") != 0) {
			return;
		}

		if (Input.GetKey (KeyCode.Q)) {
			actionDirection.value = 0;
			anim.SetBool ("Left", true);
			anim.SetBool ("Right", false);
		} else if  (Input.GetKey (KeyCode.E)) {
			actionDirection.value = 1;
			anim.SetBool ("Left", false);
			anim.SetBool ("Right", true);
		}

		if (Input.GetKey (KeyCode.B)) {
			action.value = 1;
			anim.SetInteger ("Action", 1);
		} else if (Input.GetKey (KeyCode.N)) {
			action.value = 2;
			anim.SetInteger ("Action", 2);
		} else if (Input.GetKey (KeyCode.M)) {
			action.value = 3;
			anim.SetInteger ("Action", 3);
		} else if (Input.GetKey (KeyCode.Comma)) {
			action.value = 4;
			anim.SetInteger ("Action", 4);
		} else if (Input.GetKey (KeyCode.H)) {
			action.value = 5;
			anim.SetInteger ("Action", 5);
		} else if (Input.GetKey (KeyCode.J)) {
			action.value = 6;
			anim.SetInteger ("Action", 6);
		} else if (Input.GetKey (KeyCode.K)) {
			action.value = 7;
			anim.SetInteger ("Action", 7);
		}
	}

	void LateUpdate() {
//		transform.LookAt (target);
	}

	public void onChangeDirection() {
		if (actionDirection.value == 0) {
			anim.SetBool ("Left", true);
			anim.SetBool ("Right", false);
		} else {
			anim.SetBool ("Left", false);
			anim.SetBool ("Right", true);
		}
	}

	public void onChangeAction() {
		if (action.value == 0) {
			anim.SetInteger ("Action", 0);
		} else if (action.value == 1) {
			anim.SetInteger ("Action", 1);
		} else if (action.value == 2) {
			anim.SetInteger ("Action", 2);
		} else if (action.value == 3) {
			anim.SetInteger ("Action", 3);
		} else if (action.value == 4) {
			anim.SetInteger ("Action", 4);
		} else if (action.value == 5) {
			anim.SetInteger ("Action", 5);
		} else if (action.value == 6) {
			anim.SetInteger ("Action", 6);
		} else if (action.value == 7) {
			anim.SetInteger ("Action", 7);
		}
	}
}

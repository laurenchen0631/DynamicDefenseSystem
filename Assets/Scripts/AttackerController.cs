using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AttackerController : MonoBehaviour {

	private Animator anim;
	private AnimatorStateInfo currentState;
	private AnimatorStateInfo previousState;

	public Dropdown actionDirection;
	public Dropdown action;

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
			anim.IsInTransition(0) &&
			anim.GetNextAnimatorStateInfo(0).fullPathHash == Animator.StringToHash ("Base Layer.Idle")
		) {
			action.value = 0;
			anim.SetInteger ("Action", 0);
		}
	}

	void LateUpdate() {
//		transform.Translate(0, 0, Time.deltaTime * 1);
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

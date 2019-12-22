using UnityEngine;

[AddComponentMenu("Utility/Animation Open-Close")]

public class AnimationOpenClose : MonoBehaviour {

	[SerializeField]
	private bool hasHideAnimation = false;

	static readonly int open_Anim = Animator.StringToHash ("open");
	static readonly int hide_Anim = Animator.StringToHash ("hide");

	private Animator animator;
	
	void Awake () {
		animator = GetComponent<Animator> ();

		if (animator.parameters.Length > 1) {
			hasHideAnimation = true;
		}
	}
	
	public void Click () {
		if (IsOpen()) {
			Close ();
		}
		else {
			Open ();
		}
	}

	public void Open () {
		if (!IsOpen ()) {
			animator.SetBool (open_Anim, true);
		}
	}

	public void Close () {
		if (IsOpen ()) {
			animator.SetBool (open_Anim, false);
		}
	}

	public bool IsOpen() {
		return animator.GetBool (open_Anim);
	}

	public void Hide() {
		if (hasHideAnimation) {
			animator.SetBool (hide_Anim, true);
		}
	}

	public void Show() {
		if (hasHideAnimation) {
			animator.SetBool (hide_Anim, false);
		}
	}

	public bool IsShow() {
		if (hasHideAnimation) {
			return animator.GetBool (hide_Anim);
		} else {
			return true;
		}
	}
}

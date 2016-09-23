using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SpineParticle : MonoBehaviour {

	public GameObject TargetObject;
	SkeletonAnimation _SkeletonAnimation;

	public string AnimationName;

	// Use this for initialization
	public virtual void Start () {
		_SkeletonAnimation = TargetObject.GetComponent<SkeletonAnimation> ();

		TargetObject.SetActive (false);

	}

	protected void ShowAnimation() {
		TargetObject.SetActive (true);
//		_SkeletonAnimation.AnimationName = "1.Drink";
		_SkeletonAnimation.AnimationName = AnimationName;
		_SkeletonAnimation.loop = false;
		_SkeletonAnimation.AnimationState.Complete += (state, trackIndex, loopCount) => {
			HideAnimation();
		};
	}

	void HideAnimation() {
		TargetObject.SetActive (false);
	}
}

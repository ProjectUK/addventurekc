using UnityEngine;
using System.Collections;
using Spine.Unity;
using System.Collections.Generic;

public class SippingCrow : MonoBehaviour {

	[System.Serializable]
	public class AnimationModel{
		public string Animation;
		public bool Loop;
		public float Time;
	}

	[SerializeField]
	SkeletonGraphic _SkeletonGraphic;
	[SerializeField]
	List<AnimationModel> AnimationList = new List<AnimationModel>();

	// Use this for initialization
	void Start () {
		StartCoroutine (AnimateRoutine ());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator AnimateRoutine () {
		int i = 0;
		while (true) {
			yield return null;

			AnimationModel animationModel = AnimationList[i];

			if (animationModel.Animation == null || animationModel.Animation == "") {
				_SkeletonGraphic.AnimationState.ClearTracks ();
			} else {
				_SkeletonGraphic.AnimationState.SetAnimation (0, animationModel.Animation, animationModel.Loop);
			}

			yield return IndieTime.Instance.WaitForSeconds (animationModel.Time);

			i++;
			if (i >= AnimationList.Count)
				i = 0;


		}
		yield return null;
	}
}

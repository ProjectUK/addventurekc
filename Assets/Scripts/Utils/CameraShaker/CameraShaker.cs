using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CameraShaker : MonoBehaviour {

	Vector3 initPos;

	// Use this for initialization
	void Start () {
		initPos = this.transform.position;

		EventManager.Instance.AddListener<CameraShakeEvent> (OnCameraShakeEvent);
	}
	
	void OnCameraShakeEvent(CameraShakeEvent eve) {

		Sequence shakeSeq = DOTween.Sequence ();
		shakeSeq.Append (this.transform.DOShakePosition (eve.Duration, eve.Strength, eve.Vibrato, eve.Randomness, false))
			.AppendCallback (() => {
				this.transform.position = initPos;
		});
	}
}

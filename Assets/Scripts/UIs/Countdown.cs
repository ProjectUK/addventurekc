using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class Countdown : MonoBehaviour {

	private RectTransform _RectTransform;
	public Text _Text;

	Coroutine CountRoutine;

	public delegate void _OnEnd();
	public _OnEnd OnEnd;

	float elapsedTime = 0;

	// Use this for initialization
	void Start () {
		_RectTransform = GetComponent<RectTransform> ();
	}

	public void StartCountdown(int number, float inbetweenTime, string endString) {
		this.gameObject.SetActive (true);
		if (CountRoutine != null)
			StopCoroutine (CountRoutine);
		CountRoutine = StartCoroutine (Counting(number, inbetweenTime, endString));	
	}

	IEnumerator Counting(int number, float inbetweenTime, string endString) {

		while (number >= 0) {
			
			// punching animation
			Sequence seq = DOTween.Sequence ();
			seq
				.Append (_RectTransform.DOScale (new Vector3 (0.3f, 0.3f), 0.1f).SetUpdate(true) )
				.AppendCallback( () => {
					if (number > 0)
						_Text.text = number.ToString ();
					else
						_Text.text = endString;
				})
				.Append (_RectTransform.DOScale (new Vector3 (1, 1), 0.1f).SetUpdate(true) )
				.Append (_RectTransform.DOPunchScale(new Vector3 (0.2f, 0.2f),  1, 4, 1).SetUpdate(true) );
			
			elapsedTime = 0;
			while (elapsedTime < inbetweenTime) {
//				elapsedTime += IndieTime.Instance.deltaTime;
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			number--;
		}

		if (OnEnd != null)
			OnEnd ();

		// out animation
		_RectTransform.DOScale (new Vector3 (0.3f, 0.3f), 0.1f).SetEase(Ease.InExpo).SetUpdate (true);
		elapsedTime = 0;
		while (elapsedTime < 0.1f) {
			elapsedTime += IndieTime.Instance.deltaTime;
			yield return null;
		}

		_Text.text = "";
		this.gameObject.SetActive (false);
	}

	// Update is called once per frame
	void Update () {
	
	}
}

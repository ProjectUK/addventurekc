using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class HitScoreText : MonoBehaviour {

	[SerializeField] Text ScoreText;

	public float InDuration = 0.3f;
	public float WaitDuration = 0.3f;
	public float OutDuration = 0.3f;
	public Vector3 ShowOffset;
	public Ease InEase;
	public Ease OutEase;
	private Vector3 EndScale = new Vector3(0.09423602f, 0.09423602f, 0.09423602f);

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void SetText(string text) {
		ScoreText.text = text;
	}

	public void Show(string text) {
		SetText (text);

		StartCoroutine (ShowRoutine ());
	}

	IEnumerator ShowRoutine() {
		Vector3 endPos = new Vector3 (
			this.transform.position.x + ShowOffset.x,
			this.transform.position.y + ShowOffset.y,
			this.transform.position.z + ShowOffset.z);

		Color prevColor = ScoreText.color;
		ScoreText.color = new Color (prevColor.r, prevColor.g, prevColor.b, 0);
		this.transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);

		// tween in
		DOTween.To (() => ScoreText.color, x => ScoreText.color = x, new Color (prevColor.r, prevColor.g, prevColor.b, 1f), InDuration).SetEase(InEase);
		DOTween.To (() => this.transform.localScale, x => this.transform.localScale = x, EndScale, InDuration).SetEase(InEase);

		Tween moveInTween = this.transform.DOMove (endPos, InDuration, false).SetEase (InEase);
		yield return moveInTween.WaitForCompletion ();

		yield return new WaitForSeconds (WaitDuration);

		// tween out
		DOTween.To (() => ScoreText.color, x => ScoreText.color = x, new Color (prevColor.r, prevColor.g, prevColor.b, 0f), OutDuration).SetEase(OutEase);
		DOTween.To (() => this.transform.localScale, x => this.transform.localScale = x, new Vector3 (0.001f, 0.001f, 0.001f), OutDuration).SetEase(OutEase);

		Tween moveOutTween = this.transform.DOMove (endPos, OutDuration, false).SetEase (OutEase);
		yield return moveOutTween.WaitForCompletion ();



		ScoreText.color = new Color (prevColor.r, prevColor.g, prevColor.b, 0);
		this.gameObject.SetActive (false);


	}
}

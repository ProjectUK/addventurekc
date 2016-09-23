using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class StackingButtonController : MonoBehaviour {

	private Vector3[] firstPositions;

	public Button ToggleButton;
	public Image CollapseImage;

	public Button[] Buttons;
	public RectTransform StackedPosition;
	public Transform _ToggleBtnStartPos;


	public bool DoCollapse;
	public bool DoUncollapse;

	public float CollapseTime = 0.5f;
	public Ease CollapseEase;

	public float UncollapseTime  = 0.5f;
	public Ease UncollapseEase;

	public bool Collapsed;

	Coroutine BouncingRoutine;
	Coroutine WaitForBouncingRoutine;

	Tweener currentBounceTween;


	public bool Hidden;

	bool _IsAnimating = false;

	// Use this for initialization
	void Start () {
	


		ToggleButton.onClick.AddListener (delegate() {
			AudioManager.Instance.Play("button_click", false, 1f, 0f);
			ToggleCollapse();	
		});

		// save positions
		firstPositions = new Vector3[Buttons.Length];
		for (int i = 0; i < Buttons.Length; i++) {
			firstPositions [i] = Buttons [i].transform.position;
		}

		// set to collapsed position
		for (int i = 0; i < Buttons.Length; i++) {
			Buttons [i].transform.position = StackedPosition.transform.position;
			if (Buttons[i] != ToggleButton) {
				Buttons[i].gameObject.SetActive(false);
			}
		}
		Collapsed = true;
		CollapseImage.enabled = false;

		StartBouncing ();

	}
	
	// Update is called once per frame
	void Update () {

		if (DoCollapse) {
			DoCollapse = false;
			Collapse ();
		}

		if (DoUncollapse) {
			DoUncollapse = false;
			Uncollapse ();
		}

	}

	public void Collapse() {

		if (!Collapsed) {
			for (int i = 0; i < Buttons.Length; i++) {
				
				Button currentButton = Buttons [i];
				
				Buttons [i].transform.DOMove (StackedPosition.transform.position, CollapseTime, false)
					.SetEase (CollapseEase)
					.SetUpdate(true)
					.OnComplete(()=>{
						if (currentButton != ToggleButton) {
							currentButton.gameObject.SetActive(false);
						}
					});
			}
			
			StartCoroutine (CollapseRoutine ());
			WaitForBouncingRoutine = StartCoroutine(WaitForBouncing(CollapseTime, true));
			CollapseImage.enabled = false;
			Collapsed = true;
		}
	}

	public void Uncollapse() {

		if (Collapsed) {
			for (int i = 0; i < Buttons.Length; i++) {
				Buttons[i].gameObject.SetActive(true);
				Buttons [i].transform.DOMove (firstPositions [i], UncollapseTime, false)
					.SetEase (UncollapseEase)
					.SetUpdate (true);
			}

			StartCoroutine (UncollapseRoutine ());
			StopBouncing ();
			CollapseImage.enabled = true;
			Collapsed = false;
		}


	}

	IEnumerator CollapseRoutine() {
		_IsAnimating = true;
		yield return StartCoroutine (IndieTime.Instance.WaitForSeconds (CollapseTime));
		_IsAnimating = false;
	}

	IEnumerator UncollapseRoutine() {
		_IsAnimating = true;
		yield return StartCoroutine (IndieTime.Instance.WaitForSeconds (UncollapseTime));
		_IsAnimating = false;
	}


	public void ToggleCollapse() {
		if (!_IsAnimating) {
			if (Collapsed) {
				Uncollapse ();
			} else {
				Collapse ();
			}
		}
	}


	IEnumerator WaitForBouncing(float waitTime, bool isBouncing) {
		yield return StartCoroutine (IndieTime.Instance.WaitForSeconds (waitTime));
		if (Collapsed) {
			StartBouncing ();
		}
	}

	public void ImmediateCollapse() {
		for (int i = 0; i < Buttons.Length; i++) {

			Button currentButton = Buttons [i];

			Buttons [i].transform.position = StackedPosition.transform.position;
			if (currentButton != ToggleButton) {
				currentButton.gameObject.SetActive(false);
			}
		}
		Collapsed = true;
		CollapseImage.enabled = false;
	}

	public void StartBouncing() {
		if (BouncingRoutine != null)
			StopCoroutine (BouncingRoutine);
		if (currentBounceTween != null)
			currentBounceTween.Kill ();
		BouncingRoutine = StartCoroutine (Bounce ());

	}

	public void StopBouncing() {
		if (BouncingRoutine != null)
			StopCoroutine (BouncingRoutine);
		if (WaitForBouncingRoutine != null)
			StopCoroutine (WaitForBouncingRoutine);
		if (currentBounceTween != null)
			currentBounceTween.Kill ();
		ToggleButton.transform.position = _ToggleBtnStartPos.position;
	}

	IEnumerator Bounce() {


		while (true) {
			yield return null;


			yield return StartCoroutine (IndieTime.Instance.WaitForSeconds (1f));
			currentBounceTween = ToggleButton.transform.DOMove (
				new Vector3 (_ToggleBtnStartPos.position.x, _ToggleBtnStartPos.position.y + 0.5f),
				0.2f,
				false)
				.SetUpdate (true);
			yield return StartCoroutine (IndieTime.Instance.WaitForSeconds (0.2f));

			currentBounceTween = ToggleButton.transform.DOMove (
				new Vector3 (_ToggleBtnStartPos.position.x, _ToggleBtnStartPos.position.y),
				1f,
				false)
			.SetEase (Ease.OutBounce)
				.SetUpdate (true);				
			yield return StartCoroutine (IndieTime.Instance.WaitForSeconds (1f));

		}

	}

}

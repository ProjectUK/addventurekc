using UnityEngine;
using System.Collections;

public class CafeActionController : MonoBehaviour {

	public float Countdown;

	float _CurrentTime = 0;

	public GameObject[] ActionUIs;

	// Use this for initialization
	void Start () {
		EventManager.Instance.AddListener<CafeActionStartedEvent> (OnCafeActionStartedEvent);
	}
	
	// Update is called once per frame
	void Update () {

		if (_CurrentTime > 0) {

			// receive tap
			if (Input.anyKeyDown) {
				EventManager.Instance.TriggerEvent(new ReceiveWakeEvent(3));
			}

			// countdown
			_CurrentTime -= IndieTime.Instance.deltaTime;
			if (_CurrentTime <= 0) {
				OnTimesUp ();
			}
		}
	}


	void StartCountdown() {
		_CurrentTime = Countdown;
	}

	void OnTimesUp() {
		Time.timeScale = 1;
		EventManager.Instance.TriggerEvent (new CafeActionEndedEvent ());
		SetActionUIsActive (false);
	}

	void OnCafeActionStartedEvent(CafeActionStartedEvent eve) {
		Time.timeScale = 0;
		SetActionUIsActive (true);
		StartCountdown ();
	}

	void SetActionUIsActive(bool active){
		for (int i = 0; i < ActionUIs.Length; i++) {
			ActionUIs [i].SetActive (active);
		}
	}
}

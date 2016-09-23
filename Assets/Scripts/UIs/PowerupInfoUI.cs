using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class PowerupInfoUI : MonoBehaviour {

	[System.Serializable]
	public class PowerupModel {
		public string Id;
		public string Name;
		public Sprite IconImage;
	}

	[Header("UI Positions")]
	public RectTransform BG1;
	public RectTransform BG2;
	public Image IconImage;
	public Text InfoText;

	[Header("Database")]
	public PowerupModel[] Powerups;

	Animator _Animator;

	public bool IsPlaying;

	// Use this for initialization
	void Start () {
		_Animator = GetComponent<Animator> ();
		EventManager.Instance.AddListenerOnce<PowerUpNotification> (OnPowerUpNotification);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		

	public void ShowInfo(string powerUpId) {

		// search in database
		PowerupModel powerUp = FindPowerUp(powerUpId);

		Debug.Log (powerUpId + " = " + powerUp);
		if (powerUp != null) { 
			Debug.Log (powerUp.Name);

			IconImage.sprite = powerUp.IconImage;
			InfoText.text = powerUp.Name;
		}

		StartCoroutine (IEShowInfo(powerUpId));

	}

	IEnumerator IEShowInfo(string powerUpId) {
		float timeElapsed = 0;

		//wait if not playing
		while (!IsPlaying)
			yield return null;

		if (IsPlaying) {
			// slow down time. haha
			Time.timeScale = 0.05f;
			
			// play idle first so we can override previous animation
			_Animator.Play ("Idle");
			timeElapsed = 0;
			while (timeElapsed < 0.1f) {
				yield return null;
				timeElapsed += IndieTime.Instance.deltaTime;
			}
			
			
			_Animator.Play ("Entering");
			timeElapsed = 0;
			while (timeElapsed < 1f) {
				yield return null;
				timeElapsed += IndieTime.Instance.deltaTime;
			}

			if (IsPlaying) {
				// resume time
				Time.timeScale = 1f;
			}
		}

		yield return null;
	}

	PowerupModel FindPowerUp(string powerUpId) {
		int i = 0;
		while (i < Powerups.Length) {

			PowerupModel currentPowerup = Powerups [i];
			if (currentPowerup.Id == powerUpId) {
				return currentPowerup;
			}

			i++;
		}
		return null;
	}

	private void OnPowerUpNotification(PowerUpNotification eve) {
		ShowInfo (eve.PowerUpId);
	}
}

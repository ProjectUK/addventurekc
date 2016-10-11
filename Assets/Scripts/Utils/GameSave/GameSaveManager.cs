using UnityEngine;
using System.Collections;

public class GameSaveManager : MonoBehaviour {

	const string GAME_SAVE_MANAGER_DB_VERSION = "1";

	const string DB_VERSION = "CROWS_COFFEE";
	const string FIRST_PLAY = "first_play";
	const string COIN = "coin";
	const string LANGUAGE = "language"; 
	const string BACKGROUND = "background";
	const string SFX = "sfx";
	const string BGM = "bgm";

	const string CONTROL = "control";	//0 = tilt, 1 = swipe

	const string SHOP_ITEM_KEY_PREFIX 	= "shop~";

	

	public enum GameLanguages
	{
		INDONESIA = 0,
		INGGRIS = 1
	}

	private static GameSaveManager s_Instance = null;

	// override so we don't have the typecast the object
	public static GameSaveManager Instance {
		get {
			if (s_Instance == null) {
				s_Instance = GameObject.FindObjectOfType (typeof(GameSaveManager)) as GameSaveManager;
			}
			return s_Instance;
		}
	}

	public bool DoReset;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (DoReset) {
			PlayerPrefs.DeleteAll ();
			DoReset = false;
		}
	}


	#region Firstplay

	public bool IsFirstPlay() {
		if (PlayerPrefs.HasKey (FIRST_PLAY)) {
			int intFirstPlay = PlayerPrefs.GetInt (FIRST_PLAY);
			if (intFirstPlay == 0) {
				return false;
			} else {
				//toggle first play
				SetFirstPlay (false);

				return true;
			}
		} else {

			//toggle first play
			SetFirstPlay (false);

			return true;
		}
	}

	public void SetFirstPlay (bool firstPlay){
		int intFirstPlay = firstPlay ? 1 : 0;
		PlayerPrefs.SetInt (FIRST_PLAY, intFirstPlay);
	}

	#endregion

	#region Audio

	public void SetSFX (bool isTurnedOn) {
		PlayerPrefs.SetInt (SFX, (isTurnedOn?1:0));
	}

	public bool IsSFXPlay() {
		if (PlayerPrefs.HasKey (SFX)) {
			int SFXValue = PlayerPrefs.GetInt (SFX);
			if (SFXValue == 0) {
				// it's muted
				return false;
			} else {
				// it's playing
				return true;
			}
		} else {
			// it's playing on default
			return true;
		}
	}

	public void SetBGM (bool isTurnedOn) {
		PlayerPrefs.SetInt (BGM, (isTurnedOn?1:0));
	}

	public bool IsBGMPlay() {
		if (PlayerPrefs.HasKey (BGM)) {
			int BGMValue = PlayerPrefs.GetInt (BGM);
			if (BGMValue == 0) {
				// it's muted
				return false;
			} else {
				// it's playing
				return true;
			}
		} else {
			// it's playing on default
			return true;
		}
	}

	#endregion


	#region Language
	public void SetLanguage(GameLanguages language) {
		switch (language) {
			case GameLanguages.INGGRIS:
			{
				PlayerPrefs.SetInt (LANGUAGE, (int)GameLanguages.INGGRIS);
			}
			break;
			case GameLanguages.INDONESIA:
			{
				PlayerPrefs.SetInt (LANGUAGE, (int)GameLanguages.INDONESIA);
			}
			break;
		}
	}

	public GameLanguages GetLanguage() {
		if (PlayerPrefs.HasKey (LANGUAGE)) {
			int savedLanguage = PlayerPrefs.GetInt (LANGUAGE);

			return (GameLanguages)savedLanguage;
		} else {
			// don't have any language yet, just throw default language
			return GameLanguages.INGGRIS;
		}
	}

	#endregion


	#region Monetization
	public void IncreaseCoins(int coins) {
		int currentCoin = GetCoins ();

		currentCoin += coins;

		// save coins
		SetCoins(currentCoin);
	}

	public bool DecreaseCoins(int coins) {

		// check coin sufficient
		int currentCoint = GetCoins();
		int balance = currentCoint - coins;
		if (balance >= 0) {
			// succeed decrease coin
			SetCoins(balance);
			return true;
		} else {
			// no success decrease coin
			return false;
		}

	}

	public bool CheckCoinSufficient(int price) {

		// check coin sufficient
		int currentCoint = GetCoins();
		int balance = currentCoint - price;
		if (balance >= 0) {
			return true;
		} else {
			// no success decrease coin
			return false;
		}

	}

	public int GetCoins() {
		int currentCoin = 0;
		if (PlayerPrefs.HasKey(COIN)) {
			currentCoin = PlayerPrefs.GetInt (COIN);
		}

		return currentCoin;
	}

	public void SetCoins(int coins) {
		PlayerPrefs.SetInt (COIN, coins);
	}


	public bool CheckPurchase(string itemId) {
		if (PlayerPrefs.HasKey (SHOP_ITEM_KEY_PREFIX+ itemId.ToUpper())) {
			return true;
		} else {
			return false;
		}
	}

	public void SetPurchase(string itemId) {
		PlayerPrefs.SetInt (SHOP_ITEM_KEY_PREFIX + itemId.ToUpper (), 1);
	}

	public void DeletePurchase(string itemId) {
		PlayerPrefs.DeleteKey (SHOP_ITEM_KEY_PREFIX + itemId.ToUpper ());
	}

	#endregion

	#region Background

	public void SetBackground (string bgName) {
		PlayerPrefs.SetString (BACKGROUND, bgName);
	}

	public string GetBackground () {
		if (PlayerPrefs.HasKey (BACKGROUND)) {
			return PlayerPrefs.GetString (BACKGROUND);
		} else {
			return "default";
		}
	}

	public bool IsCurrentBackground(string bgName) {
		Debug.Log ("current(" + GetBackground () + ") == " + bgName);

		if (GetBackground () == bgName) {
			return true;
		} else {
			return false;
		}
	}

	#endregion

	#region Control Options

	public void SetControlSwipe (bool isSwipe) {
		PlayerPrefs.SetInt (CONTROL, (isSwipe?1:0));
	}

	public bool IsControlSwipe() {
		if (PlayerPrefs.HasKey (CONTROL)) {
			int ControlValue = PlayerPrefs.GetInt (CONTROL);
			if (ControlValue == 0) {
				// it's tilt
				return false;
			} else {
				// it's swipe
				return true;
			}
		} else {
			// it's tilt by default
			return false;
		}
	}

	#endregion

	#region Utilities
	// Warning! Be careful!
	public void DeleteAll() {
		PlayerPrefs.DeleteAll ();
	}

	public void SetDBVersion(int version) {
		PlayerPrefs.SetInt (DB_VERSION, version);
	}

	public int GetDBVersion() {
		int dbVersion = PlayerPrefs.GetInt (DB_VERSION, 0);
		return dbVersion;
	}

	#endregion


}

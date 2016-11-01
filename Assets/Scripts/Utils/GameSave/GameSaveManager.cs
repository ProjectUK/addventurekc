using UnityEngine;
using System.Collections;

public class GameSaveManager : MonoBehaviour {

	const int GAME_SAVE_MANAGER_DB_VERSION = 1;

	const string INT_DB_INITIALIZED = "init_db";	// 0 = false, 1 = true
	const string INT_DB_VERSION = "CROWS_COFFEE_DB_VERSION";
	const string INT_FIRST_PLAY = "first_play";
	const string INT_COIN = "coin";
	const string INT_LANGUAGE = "language"; 
	const string INT_SFX = "sfx";
	const string INT_BGM = "bgm";
	const string STR_BACKGROUND = "background";

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

	public int MockupCoin;

	public bool DoReset;
	public bool DoMockup;

	// Use this for initialization
	void Start () {

		CheckInitDB ();

		MockupCoin = GetCoins ();
	}
	
	// Update is called once per frame
	void Update () {
		if (DoReset) {
			DoReset = false;
			PlayerPrefs.DeleteAll ();
		}

		if (DoMockup) {
			DoMockup = false;
			SetCoins (MockupCoin);
		}
	}

	#region InitDb

	void CheckInitDB () {

		if (PlayerPrefs.HasKey (INT_DB_INITIALIZED)) {

			if (PlayerPrefs.GetInt (INT_DB_INITIALIZED, 0) == 1) {
				// initialized

				// check version same with hardcoded version (such as updated version)
				int dbVersion = PlayerPrefs.GetInt (INT_DB_VERSION, 0);
				
				if (dbVersion != GAME_SAVE_MANAGER_DB_VERSION) {
					// do something with differences
					UpdateDB (dbVersion);
				}
			} else {
				// not initialized
				InitDB();
			}

		} else {
			InitDB ();
		}

	}

	void InitDB () {

		// default value of coin
		SetCoins(125);


		PlayerPrefs.SetInt (INT_DB_INITIALIZED, 1);
		PlayerPrefs.SetInt (INT_DB_VERSION, GAME_SAVE_MANAGER_DB_VERSION);
	}

	void UpdateDB(int prevVersion) {

		switch(prevVersion) {
		case 0:
			break;
		case 1:
			break;
		default:
			break;
		}

		PlayerPrefs.SetInt (INT_DB_VERSION, GAME_SAVE_MANAGER_DB_VERSION);

	}

	#endregion

	#region Firstplay

	public bool IsFirstPlay() {
		if (PlayerPrefs.HasKey (INT_FIRST_PLAY)) {
			int intFirstPlay = PlayerPrefs.GetInt (INT_FIRST_PLAY);
			if (intFirstPlay == 0) {
				return false;
			} else {
				//toggle first play
//				SetFirstPlay (false);

				return true;
			}
		} else {

			//toggle first play
//			SetFirstPlay (false);

			return true;
		}
	}

	public void SetFirstPlay (bool firstPlay){
		int intFirstPlay = firstPlay ? 1 : 0;
		PlayerPrefs.SetInt (INT_FIRST_PLAY, intFirstPlay);
	}

	#endregion

	#region Audio

	public void SetSFX (bool isTurnedOn) {
		PlayerPrefs.SetInt (INT_SFX, (isTurnedOn?1:0));
	}

	public bool IsSFXPlay() {
		if (PlayerPrefs.HasKey (INT_SFX)) {
			int SFXValue = PlayerPrefs.GetInt (INT_SFX);
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
		PlayerPrefs.SetInt (INT_BGM, (isTurnedOn?1:0));
	}

	public bool IsBGMPlay() {
		if (PlayerPrefs.HasKey (INT_BGM)) {
			int BGMValue = PlayerPrefs.GetInt (INT_BGM);
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
				PlayerPrefs.SetInt (INT_LANGUAGE, (int)GameLanguages.INGGRIS);
			}
			break;
			case GameLanguages.INDONESIA:
			{
				PlayerPrefs.SetInt (INT_LANGUAGE, (int)GameLanguages.INDONESIA);
			}
			break;
		}
	}

	public GameLanguages GetLanguage() {
		if (PlayerPrefs.HasKey (INT_LANGUAGE)) {
			int savedLanguage = PlayerPrefs.GetInt (INT_LANGUAGE);

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
		if (PlayerPrefs.HasKey(INT_COIN)) {
			currentCoin = PlayerPrefs.GetInt (INT_COIN);
		}

		return currentCoin;
	}

	public void SetCoins(int coins) {
		PlayerPrefs.SetInt (INT_COIN, coins);
	}

	public void AddCoins(int coins) {

		int prevCoin = GetCoins ();
		int totalCoin = prevCoin + coins;

		PlayerPrefs.SetInt (INT_COIN, totalCoin);
	}

	public bool CheckPurchase(string itemId) {
		if (PlayerPrefs.HasKey (SHOP_ITEM_KEY_PREFIX+ itemId.ToUpper())) {
			return true;
		} else {
			return false;
		}
	}

	public int GetPurchaseLevel(string itemId) {
		if (CheckPurchase (itemId)) {
			return PlayerPrefs.GetInt(SHOP_ITEM_KEY_PREFIX + itemId.ToUpper(), 0);
		} else {
			return 0;
		}
	}

	public void SetPurchase(string itemId, int level) {
		PlayerPrefs.SetInt (SHOP_ITEM_KEY_PREFIX + itemId.ToUpper (), level);
	}

	public void DeletePurchase(string itemId) {
		PlayerPrefs.DeleteKey (SHOP_ITEM_KEY_PREFIX + itemId.ToUpper ());
	}

	#endregion

	#region Utilities
	// Warning! Be careful!
	public void DeleteAll() {
		PlayerPrefs.DeleteAll ();
	}

	public void SetDBVersion(int version) {
		PlayerPrefs.SetInt (INT_DB_VERSION, version);
	}

	public int GetDBVersion() {
		int dbVersion = PlayerPrefs.GetInt (INT_DB_VERSION, 0);
		return dbVersion;
	}

	#endregion


}

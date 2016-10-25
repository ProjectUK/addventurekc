using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIOGodMode : UIOverlay {

	[SerializeField] GameSaveManager _GameSaveManager;

	[SerializeField] InputField _CoinInputField;
	[SerializeField] Button _MockupCoinButton;
	[SerializeField] Button _ResetDataButton;
	[SerializeField] Text _LogText;

	public override void Start ()
	{
		base.Start ();

		_CoinInputField.keyboardType = TouchScreenKeyboardType.NumberPad;

		_MockupCoinButton.onClick.AddListener (() => {
			int coin = int.Parse(_CoinInputField.text);

			_GameSaveManager.SetCoins(coin);

			_LogText.text = "Coin set to " + coin.ToString();
		});

		_ResetDataButton.onClick.AddListener (() => {
			_GameSaveManager.DeleteAll();
			_LogText.text = "Data resetted. ((GAME WILL RESTART....))";
			StartCoroutine(DataResetRoutine());
		});
	}

	IEnumerator DataResetRoutine() {
		yield return IndieTime.Instance.WaitForSeconds (1);
		SceneManager.LoadScene ("MainScene");
	}

	public void ToggleShow() {
		if (IsHidden) {
			Show (true);
		} else {
			Hide (true);
		}
	}
}

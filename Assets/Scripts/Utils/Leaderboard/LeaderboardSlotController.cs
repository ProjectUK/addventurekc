using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LeaderboardSlotController : MonoBehaviour {

	public Text Number;
	public Text Name;
	public Text Score;

	public void SetVal(int number, string name, float score) {
		Number.text = number.ToString ();
		Name.text = name;
		Score.text = score.ToString ();
	}
}

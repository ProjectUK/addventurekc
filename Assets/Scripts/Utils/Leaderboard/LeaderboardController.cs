using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LeaderboardController : MonoBehaviour {

	[System.Serializable]
	public class PlayerModel{
		public string Name;
		public float Score;
	}

	public PlayerModel[] Players;

	public GameObject SlotsHolder;
	LeaderboardSlotController[] _Slots;

	// Use this for initialization
	void Start () {
		_Slots = SlotsHolder.GetComponentsInChildren<LeaderboardSlotController> ();

		InitScoreData();



	}
	
	public void InitScoreData() {
		//TODO: Do loading and set score data here

		// set DUMMY values
		for (int i = 0; i < _Slots.Length;i++) {
			_Slots [i].SetVal (i+1, Players[i].Name, Players[i].Score);
		}
	}
}

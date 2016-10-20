using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemRemainings : MonoBehaviour {

	public int Value;
	public int MaxValue;
	public GameObject[] ObjectRemainings;
	public GameObject[] ObjectRemainingHolders;

	// Update is called once per frame
	void Update () {
		for (int i = 0 ; i < ObjectRemainings.Length; i++) {
			if (i <= Value - 1) {
				ObjectRemainings [i].SetActive (true);
			} else {
				ObjectRemainings [i].SetActive (false);
			}
		}
		for (int i = 0 ; i < ObjectRemainingHolders.Length; i++) {
			if (i <= MaxValue - 1) {
				ObjectRemainingHolders [i].SetActive (true);
			} else {
				ObjectRemainingHolders [i].SetActive (false);
			}
		}
	}
}

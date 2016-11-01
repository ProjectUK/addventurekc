using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopCabinet : MonoBehaviour {

	public const int MAX_ITEM = 2;

	public CabinetItem Item1;
	public CabinetItem Item2;

	RectTransform _RectTransform;

	// Use this for initialization
	void Start () {
		_RectTransform = GetComponent <RectTransform> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetPosition(Vector2 position) {
		_RectTransform.anchoredPosition = position;
	}

	public Vector2 GetPosition() {
		return _RectTransform.anchoredPosition;
	}

	public Vector2 GetSize() {
		return _RectTransform.sizeDelta;
	}
}

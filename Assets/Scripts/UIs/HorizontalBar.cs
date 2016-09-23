using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HorizontalBar : MonoBehaviour {

	public Image HorizontalImage;

	[Range(0,1)]
	public float Value;

	void Update () {
		HorizontalImage.fillAmount = Value;
	}
		
}

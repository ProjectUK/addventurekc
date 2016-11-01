using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollingText : MonoBehaviour {

	public Vector2 StartPosition;
	public Vector2 EndPosition;

	public RectTransform TargetRect;

	public float Percentage;
	public float Speed = 0.01f;
	public float FastSpeed = 0.07f;
	public bool Running;

	public delegate void _OnEnd();
	public _OnEnd OnEnd;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if (Running) {
			TargetRect.anchoredPosition = Vector2.Lerp (StartPosition, EndPosition, Percentage);


			float currentSpeed = Speed;
			if (Input.GetMouseButton(0)) {
				currentSpeed = FastSpeed;
				Debug.Log ("Test test");
			}

			Percentage += currentSpeed * Time.unscaledDeltaTime;

			if (Percentage > 1) {
				Percentage = 0;

				if (OnEnd != null) {
					OnEnd ();
				}

			}else if (Percentage < 0) {
				Percentage = 1;
			}



		}

	}
}

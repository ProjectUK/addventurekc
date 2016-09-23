/**
 * Touch based joystick
 * last update: @igrir (25/June/2016)
 * 
 * How to use:
 * 1. Add a game object with this class and use DeltaTouch for the input
 * 
 */ 

using UnityEngine;
using System.Collections;

public class JoystickController : MonoBehaviour {

	public GameObject Marker;
	public GameObject InsideMarker;
	public float MarkerRadius = 1;
	public Vector3 MaxTouch;

	public bool IsRunning;

	bool _isTouching = false;
	Vector3 _TouchStartPos;
	public Vector3 DeltaTouch;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		UpdateJoystick ();


	}


	void UpdateJoystick() {

		if (!IsRunning) {
			Marker.SetActive (false);
			return;
		}


		Vector3 mousePos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		// ------- detect touches ------- 
		if (!_isTouching) {
			// set first position of offset
			if (Input.GetMouseButtonDown (0)) {
				_TouchStartPos = mousePos;
				_isTouching = true;

				this.transform.position = mousePos;
				Marker.SetActive (true);

			}
		} else {
			// set movement by previous delta position
			if (Input.GetMouseButton(0)) {

				DeltaTouch = (mousePos - _TouchStartPos)*Time.timeScale;
				DeltaTouch = new Vector3 (
					Mathf.Clamp(DeltaTouch.x, -MaxTouch.x, MaxTouch.x) ,
					Mathf.Clamp(DeltaTouch.y, -MaxTouch.y, MaxTouch.y)
				);


				// ------ set marker position

				// check marker position distance from center
				float targetDistance = Vector3.Distance(mousePos, this.transform.position);

				if (targetDistance < MarkerRadius) {
					InsideMarker.transform.position = new Vector3(mousePos.x, mousePos.y, InsideMarker.transform.position.z);
				}else{
					//outside boundary

					// get angle
					float angle = Mathf.Atan2(mousePos.y - this.transform.position.y, mousePos.x - this.transform.position.x);

					float x = this.transform.position.x + (MarkerRadius *(Mathf.Cos (angle)));
					float y = this.transform.position.y + (MarkerRadius *(Mathf.Sin (angle)));
					InsideMarker.transform.position = new Vector3 (x, y, InsideMarker.transform.position.z);
				}



			}
		}
		// reset delta movement when touch up
		if (Input.GetMouseButtonUp(0)) {
			_isTouching = false;
			DeltaTouch = Vector3.zero;

			InsideMarker.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y, InsideMarker.transform.position.z);
			Marker.SetActive (false);
		}
	}

}

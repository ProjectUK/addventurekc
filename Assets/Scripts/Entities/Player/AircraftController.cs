using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AircraftController : MonoBehaviour {

	public enum ControlMethod
	{
		TILT,
		SWIPE
	}

	public enum AircraftMovementState
	{
		IDLE,
		TILT_LEFT,
		TILT_RIGHT
	}


	public bool Playing;
	public bool Shooting;
	public bool InitialGameStarting;

	private bool _Shielded;
	public bool Shielded{
		get{
			return _Shielded;
		}
	}

	public ControlMethod Control;

	public JoystickController Joystick;

	[Header("Super power")]
	public GameObject ShieldObj;


	[Header("Movement objects")]
	public GameObject IdleObj;
	public GameObject TiltLeftObj;
	public GameObject TiltRightObj;


	[Header("X positions")]
	public Transform MaxX;
	public Transform MinX;

	[Header("Y positions")]
	public Transform MaxY;
	public Transform MinY;

	[Space(2)]

	[Header("Default Start Pos")]
	public Transform DefaultStartPos;

	// offset from the first position of player's device
	public Vector3 InputOffset;

	public Vector3 TiltSpeed;
	public Vector3 TouchSpeed;

	[Header("Gun")]
	public GunModel[] Guns;

	GameObjectPool BulletPool;

	Coroutine ShootingRoutine;

	// delegates
	public delegate void _OnExploding();
	public _OnExploding OnExploding;

	public delegate void _OnExploded();
	public _OnExploded OnExploded;

	public float InitTime = 1;
	public float InitInvulnerableTime = 1;
	float _CurrentInvulnerableTime;
	float _CurrentInitTime;

	public float XMovementSpeed;

	private Vector3 _DeltaMovement;
	public float MinXMovementTolerance;
	public float CountdownStopTolerance = 0.1f;

	AircraftMovementState _CurrentMovementState = AircraftMovementState.IDLE;
	bool doStop = false;
	float countdownStop = 0;

	List<Coroutine> _ShootingRoutine = new List<Coroutine>();

	List<float> _TmpShootingDelays = new List<float>();

	public bool IsPlaying;

	#region Unity Methods

	// Use this for initialization
	void Start () {
		BulletPool = PoolManager.Instance.CreatePool ("BulletPool", Resources.Load ("Prefabs/Bullets/PlayerBullet") as GameObject, 20, true);		

		EventManager.Instance.AddListener<ShieldPowerEvent> (OnShieldPowerEvent);
		EventManager.Instance.AddListener<BulletBoostEvent> (OnBulletBoostEvent);
		EventManager.Instance.AddListener<AircraftSetControlSwipeEvent> (OnAircraftSetControlEvent);

		// save shooting delays
		for (int i = 0; i < Guns.Length; i++) {
			float delay = Guns [i].ShootingDelay;
			_TmpShootingDelays.Add (delay);
		}

//		SetControlMode (GameSaveManager.Instance.IsControlSwipe());

		// force to swipe
		SetControlMode(true);


	}

	void OnEnable() {
		_CurrentInvulnerableTime = 0;
		StartCoroutine (IEUpdateAircraftState ());
		StartCoroutine (IEUpdateMovement ());
	}
	
	// Update is called once per frame
	void Update () {


	}


	void OnTriggerEnter2D(Collider2D coll) {
		if (coll.tag == "Bullet/Enemy") {
			Die ();
		}
	}
		
	#endregion

	IEnumerator IEUpdateMovement() {
		while (true) {
			Vector3 prevPos = this.transform.position;

			if (Playing) {
				UpdateMovement ();
				UpdateShield ();
			}

			yield return null;

		}
	}

	void UpdateShield () {

		if (IsPlaying) {
			if (_CurrentInvulnerableTime > 0) {
				
				AudioManager.Instance.Play ("shield", false, 2f, 0.1f, true);
				
				ShieldObj.SetActive (true);
				_Shielded = true;
				_CurrentInvulnerableTime -= Time.deltaTime;
			} else {
				ShieldObj.SetActive (false);
				_Shielded = false;
			}
		}

	}

	void UpdateMovement() {




		if (_CurrentInitTime <= 0) {

			if (Playing && !InitialGameStarting) {

				if (Control == ControlMethod.TILT) {
					float xAcceleration = (Input.acceleration.x - InputOffset.x) * TiltSpeed.x * Time.deltaTime;
					float xPos = this.transform.position.x + xAcceleration;
					xPos = Mathf.Clamp (xPos, MinX.position.x, MaxX.position.x);
											
					float yAcceleration = (Input.acceleration.y - InputOffset.y) * TiltSpeed.y * Time.deltaTime;
					float yPos = this.transform.position.y + yAcceleration;
					yPos = Mathf.Clamp (yPos, MinY.position.y, MaxY.position.y);
						
					Vector3 position = new Vector3 (
						                   xPos,
						                   yPos,
						                   this.transform.position.z);
						
					transform.position = position;

				} else if (Control == ControlMethod.SWIPE) {
					

					float xAcceleration = (Joystick.DeltaTouch.x) * TouchSpeed.x * Time.deltaTime;
					float xPos = this.transform.position.x + xAcceleration;
					xPos = Mathf.Clamp (xPos, MinX.position.x, MaxX.position.x);

					float yAcceleration = (Joystick.DeltaTouch.y) * TouchSpeed.y * Time.deltaTime;
					float yPos = this.transform.position.y + yAcceleration;
					yPos = Mathf.Clamp (yPos, MinY.position.y, MaxY.position.y);

					Vector3 position = new Vector3 (
						xPos,
						yPos,
						this.transform.position.z);

					transform.position = position;

						
				}

			}
		} else {
			_CurrentInitTime -= Time.deltaTime;
		}

	}

	IEnumerator IEUpdateAircraftState() {

		while (true) {
			yield return null;

			// Calculate movement
			Vector3 prevPos = this.transform.position;
			yield return new WaitForSeconds(0.1f);
			_DeltaMovement = this.transform.position - prevPos;

			SetSpriteMovement (_CurrentMovementState);

			if (!doStop) {
				// wait before stop
				if (Mathf.Abs(_DeltaMovement.x) < MinXMovementTolerance) {
					countdownStop += Time.deltaTime;
				} else {
					countdownStop = 0;
				}
				
				if (countdownStop > CountdownStopTolerance) {
					doStop = true;
					countdownStop = 0;
				}
			}
			if (_CurrentMovementState == AircraftMovementState.TILT_LEFT) {	
				
				//sudden movement to opposite direction
				if (_DeltaMovement.x > 0) {
					if (Mathf.Abs (_DeltaMovement.x) > MinXMovementTolerance) {
						_CurrentMovementState = AircraftMovementState.TILT_RIGHT;
						countdownStop = 0;
					}
				}
				if (doStop) {
					// set to idle
					_CurrentMovementState = AircraftMovementState.IDLE;
					// play back animation
					TiltLeftObj.GetComponent<TiltingAircraftController> ().SpriteAnimator.Play ("Aircraft Tilt Left Inverse");
					yield return new WaitForSeconds (0.2f);
					doStop = false;
				}

			} else if (_CurrentMovementState == AircraftMovementState.TILT_RIGHT) {

				//sudden movement to opposite direction
				if (_DeltaMovement.x < 0) {
					if (Mathf.Abs (_DeltaMovement.x) > MinXMovementTolerance) {
						_CurrentMovementState = AircraftMovementState.TILT_LEFT;
						countdownStop = 0;
					}
				} 
				if (doStop) {
					// set to idle
					_CurrentMovementState = AircraftMovementState.IDLE;
					// play back animation
					TiltRightObj.GetComponent<TiltingAircraftController> ().SpriteAnimator.Play ("Aircraft Tilt Right Inverse");
					yield return new WaitForSeconds (0.2f);
					doStop = false;
				}

			} else if (_CurrentMovementState == AircraftMovementState.IDLE) {
				if (Mathf.Abs (_DeltaMovement.x) > MinXMovementTolerance) {
					if (_DeltaMovement.x < 0) {
						TiltLeftObj.GetComponent<TiltingAircraftController> ().SpriteAnimator.Play ("Aircraft Tilt Left");
						_CurrentMovementState = AircraftMovementState.TILT_LEFT;
					} else {
						TiltLeftObj.GetComponent<TiltingAircraftController> ().SpriteAnimator.Play ("Aircraft Tilt Right");
						_CurrentMovementState = AircraftMovementState.TILT_RIGHT;
					}
				} else {
					_CurrentMovementState = AircraftMovementState.IDLE;
				}
			}

		}
			
	}


	// set image of aircraft
	void SetSpriteMovement(AircraftMovementState movement) {
		switch (movement) {
		case AircraftMovementState.IDLE:
			{
				IdleObj.SetActive (true);
				TiltLeftObj.SetActive (false);
				TiltRightObj.SetActive (false);
			}
			break;
		case AircraftMovementState.TILT_LEFT:
			{
				IdleObj.SetActive (false);
				TiltLeftObj.SetActive (true);
				TiltRightObj.SetActive (false);
			}
			break;
		case AircraftMovementState.TILT_RIGHT:
			{
				IdleObj.SetActive (false);
				TiltLeftObj.SetActive (false);
				TiltRightObj.SetActive (true);
			}
			break;
		}
	}
		

	public void Init() {
		InputOffset = new Vector3 (Input.acceleration.x, Input.acceleration.y, Input.acceleration.z);
		transform.position = DefaultStartPos.position;


	}

	public void Restart() {
		_CurrentInvulnerableTime = InitInvulnerableTime;
		_CurrentInitTime = InitTime;

		Playing = true;
		Shooting = true;
		transform.position = DefaultStartPos.position;

		//reset shooting delays
		for (int i = 0; i < Guns.Length; i++) {
			Guns [i].ShootingDelay = _TmpShootingDelays [i];
		}
	}

	void Shoot(GunModel gunModel) {
		GameObject bullet = BulletPool.Get ();

		if (bullet != null) {
			bullet.transform.position = gunModel.GunPosition.position;

			BulletController bc = bullet.GetComponent<BulletController> ();
			if (bc != null)
				bc.Speed = gunModel.BulletSpeed;

			bullet.SetActive (true);
		}
	}

	public void StartShooting () {
		// reset previous routines
		StopShooting();

		for (int i = 0; i < Guns.Length; i++) {

			GunModel currentGun = Guns [i];

			Coroutine shootRoutine = StartCoroutine (UpdateShoot (currentGun));
			_ShootingRoutine.Add (shootRoutine);
		}
	}

	public void StopShooting() {
		for (int i = 0; i < _ShootingRoutine.Count; i++) {			
			StopCoroutine (_ShootingRoutine [i]);
		}
		_ShootingRoutine.Clear ();
	}

	IEnumerator UpdateShoot(GunModel currentGunModel) {
		yield return null;
		while (true) {

			// shooting boost
			if (currentGunModel.BoostTime > 0) {
				float elapsedTime = Time.realtimeSinceStartup;
				while (currentGunModel.BoostTime > 0) {
					elapsedTime = Time.realtimeSinceStartup;
					Shoot (currentGunModel);
					yield return new WaitForSeconds (currentGunModel.BoostDelay);
					currentGunModel.BoostTime -= Time.realtimeSinceStartup - elapsedTime;
				}
			}

			Shoot (currentGunModel);
			yield return new WaitForSeconds (currentGunModel.ShootingDelay);
		}
	}

	#region Actions

	public void Die() {

		if (_CurrentInvulnerableTime <= 0) {
			Explode ();
		}

	}

	public void Explode (){

		if (OnExploding != null)
			OnExploding ();		

		if (ShootingRoutine != null)
			StopCoroutine (ShootingRoutine);

		// make it unmovable
		Playing = false;

		Tinker.ParticleManager.Instance.Spawn ("Explosion", this.transform.position);
		StartCoroutine(WaitAndHide(0.2f));

	}
	#endregion

	IEnumerator WaitAndHide(float wait) {
		yield return new WaitForSeconds (wait);

		if (OnExploded != null) {
			OnExploded ();
		}
		this.gameObject.SetActive (false);
	}

	void SetControlMode(bool isSwipe) {
		// set currently saved movement mode
		if (isSwipe) {
			Control = ControlMethod.SWIPE;
			Joystick.IsRunning = true;
		} else {
			Control = ControlMethod.TILT;
			Joystick.IsRunning = false;
		}
	}

	#region Event Listeners
	void OnShieldPowerEvent(ShieldPowerEvent eve ){
		_CurrentInvulnerableTime = eve.ShieldTime;
	}

	void OnBulletBoostEvent(BulletBoostEvent eve) {

		// add to all gun
		for (int i = 0; i < Guns.Length; i++) {
			GunModel currentGunModel = Guns [i];

			currentGunModel.BoostDelay = eve.BoostDelay;
			currentGunModel.BoostTime = eve.BoostTime;

//			// prevent shooting delay to 0
//			if (currentGunModel.ShootingDelay-eve.BoostTime > 0) {
//				if (currentGunModel.ShootingDelay > eve.BoostTime) {
//					currentGunModel.ShootingDelay -= eve.BoostTime;
//				}
//			}
		}
	}

	void OnAircraftSetControlEvent(AircraftSetControlSwipeEvent eve) {
//		SetControlMode (eve.IsSwipe);
	}
	#endregion
}

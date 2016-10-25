using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AircraftController : MonoBehaviour {

	public enum ControlMethod
	{
		SWIPE
	}

	public enum AircraftMovementState
	{
		IDLE,
		TILT_LEFT,
		TILT_RIGHT
	}

	public bool IsPlaying;
	public bool Shooting;
	public bool InitialGameStarting;
	public bool UseDeltaMovement = false;

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
	public GameObject ShadowAircraft;
	public GameObject LaserObj;

	[Header("Animator")]
	public Animator AirplaneAnimator;

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

	[Header("Delta Movement Properties")]
	public float MinXMovementTolerance;
	public float CountdownStopTolerance = 0.1f;

	public AircraftMovementState _CurrentMovementState = AircraftMovementState.IDLE;
	bool doStop = false;
	float countdownStop = 0;

	List<Coroutine> _ShootingRoutine = new List<Coroutine>();

	List<float> _TmpShootingDelays = new List<float>();
	private Vector3 _DeltaMovement;
	private float _XAcceleration;

	float _CurrentInvulnerableTime;
	float _CurrentInitTime;

	float _BulletDamage = 1;

	// Purchased item upgrades
	float _PurchasedSpeedMultiplier = 1;
	float _PurchasedBulletDamageMultiplier = 1;
	float _PurchasedBulletSpeedMultiplier = 1;

	Vector2 INITIAL_BULLET_SPEED = new Vector2(0, 4);

	float _SpeedboostPowerTime = 0;
	float _SpeedboostMultiplier = 1;

	float _SpreadGunPowerTime = 0;
	bool _SpreadGunActive = false;

	float _ShadowAircraftPowerTime = 0;
	bool _ShadowAircraftActive = false;

	float _LaserPowerTime = 0;
	bool _LaserActive = false;

	#region Unity Methods

	// Use this for initialization
	void Start () {
		BulletPool = PoolManager.Instance.CreatePool ("BulletPool", Resources.Load ("Prefabs/Bullets/PlayerBullet") as GameObject, 20, true);

		EventManager.Instance.AddListener<ShieldPowerEvent> (OnShieldPowerEvent);
		EventManager.Instance.AddListener<BulletBoostPowerEvent> (OnBulletBoostEvent);
		EventManager.Instance.AddListener<SpeedBoostPowerEvent> (OnSpeedBoostPowerEvent);
		EventManager.Instance.AddListener<SpreadGunPowerEvent> (OnSpreadGunPowerEvent);
		EventManager.Instance.AddListener<ShadowAircraftPowerEvent> (OnShadowAircraftPowerEvent);
		EventManager.Instance.AddListener<LaserPowerEvent> (OnLaserPowerEvent);

		// save shooting delays
		for (int i = 0; i < Guns.Length; i++) {
			float delay = Guns [i].ShootingDelay;
			_TmpShootingDelays.Add (delay);
		}

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

			if (IsPlaying) {
				UpdateMovement ();
				UpdateShield ();
				UpdatePowerUps ();
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

			if (IsPlaying && !InitialGameStarting) {
				_XAcceleration = (Joystick.DeltaTouch.x) * TouchSpeed.x * Time.deltaTime * _PurchasedSpeedMultiplier * _SpeedboostMultiplier;
				float xPos = this.transform.position.x + _XAcceleration;
				xPos = Mathf.Clamp (xPos, MinX.position.x, MaxX.position.x);

				float yAcceleration = (Joystick.DeltaTouch.y) * TouchSpeed.y * Time.deltaTime * _PurchasedSpeedMultiplier * _SpeedboostMultiplier;
				float yPos = this.transform.position.y + yAcceleration;
				yPos = Mathf.Clamp (yPos, MinY.position.y, MaxY.position.y);

				Vector3 position = new Vector3 (
					xPos,
					yPos,
					this.transform.position.z);

				transform.position = position;
			}
		} else {
			_CurrentInitTime -= Time.deltaTime;
		}

	}
	 
	// Change to based on input state (not delta movement state)
	IEnumerator IEUpdateAircraftState() {

		while (true) {
			yield return null;
			UpdateInputMovement ();
			UpdateAnimator ();
		}
			
	}

	void UpdateInputMovement () {
		// set movement state
		if (_XAcceleration > 0) {
			_CurrentMovementState = AircraftMovementState.TILT_RIGHT;
		} else if (_XAcceleration < 0) {
			_CurrentMovementState = AircraftMovementState.TILT_LEFT;
		} else {
			_CurrentMovementState = AircraftMovementState.IDLE;
		}
	}

	void UpdateAnimator() {
		if (_CurrentMovementState == AircraftMovementState.IDLE) {
			AirplaneAnimator.SetBool ("idle", true);
			AirplaneAnimator.SetBool ("turn_right", false);
			AirplaneAnimator.SetBool ("turn_left", false);
		} else if (_CurrentMovementState == AircraftMovementState.TILT_RIGHT) {
			AirplaneAnimator.SetBool ("idle", false);
			AirplaneAnimator.SetBool ("turn_right", true);
			AirplaneAnimator.SetBool ("turn_left", false);
		}else if (_CurrentMovementState == AircraftMovementState.TILT_LEFT) {
			AirplaneAnimator.SetBool ("idle", false);
			AirplaneAnimator.SetBool ("turn_right", false);
			AirplaneAnimator.SetBool ("turn_left", true);
		}
	}

	void UpdatePowerUps() {
		UpdateSpeedboostPower ();
		UpdateSpreadGunPower ();
		UpdateShadowAircraftPower ();
		UpdateLaserPower ();
	}

	public void Init() {
		// Turn on joystick
		Joystick.IsRunning = true;

		InputOffset = new Vector3 (Input.acceleration.x, Input.acceleration.y, Input.acceleration.z);
		transform.position = DefaultStartPos.position;

		//---- Purchased items initializations

		// Speed (Engine)
		int purchasedSpeed = GameSaveManager.Instance.GetPurchaseLevel(GameConst.ITEM_ENGINE);
		if (purchasedSpeed == 0) {
			_PurchasedSpeedMultiplier = 1f;
		}else if (purchasedSpeed == 1) {
			_PurchasedSpeedMultiplier = 1.25f;
		}else if (purchasedSpeed == 2) {
			_PurchasedSpeedMultiplier = 1.5f;
		}

		// Bullet damage (Fire Power, Crows Cannon)
		int purchasedDamage = GameSaveManager.Instance.GetPurchaseLevel(GameConst.ITEM_CROWSCANNON);
		if (purchasedDamage == 0) {
			_PurchasedBulletDamageMultiplier = 1f;
		}else if (purchasedDamage == 1) {
			_PurchasedBulletDamageMultiplier = 1.5f;
		}else if (purchasedDamage == 2) {
			_PurchasedBulletDamageMultiplier = 3f;
		}

		// Bullet speed (Machine Guns)
		int purchasedBulletSpeed = GameSaveManager.Instance.GetPurchaseLevel(GameConst.ITEM_MACHINEGUNS);
		if (purchasedBulletSpeed == 0) {
			_PurchasedBulletSpeedMultiplier = 1f;
		}else if (purchasedBulletSpeed == 1) {
			_PurchasedBulletSpeedMultiplier = 1.5f;
		}else if (purchasedBulletSpeed == 2) {
			_PurchasedBulletSpeedMultiplier = 2f;
		}
		SetBulletSpeed (INITIAL_BULLET_SPEED * _PurchasedBulletSpeedMultiplier);


		SetSpreadGunActive (false);
		SetShadowAircraftActive (false);
		SetLaserActive (false);
	}

	public void Restart() {
		_CurrentInvulnerableTime = InitInvulnerableTime;
		_CurrentInitTime = InitTime;

		IsPlaying = true;
		Shooting = true;
		transform.position = DefaultStartPos.position;

		//reset shooting delays
		for (int i = 0; i < Guns.Length; i++) {
			Guns [i].ShootingDelay = _TmpShootingDelays [i];
		}
	}

	#region Guns

	void Shoot(GunModel gunModel) {

		if (!gunModel.Active)
			return;

		GameObject bullet = BulletPool.Get ();

		if (bullet != null) {
			bullet.transform.position = gunModel.GunPosition.position;

			BulletController bc = bullet.GetComponent<BulletController> ();
			if (bc != null) {
				bc.Speed = gunModel.BulletSpeed;
				bc.Damage = _BulletDamage * _PurchasedBulletDamageMultiplier;
			}

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

	void SetGunDelayTime(float delayTime) {
		for (int i = 0; i < Guns.Length; i++) {
			GunModel currentGun = Guns [i];
			currentGun.ShootingDelay = delayTime;
		}
	}

	void SetBulletSpeed(Vector2 bulletSpeed) {
		for (int i = 0; i < Guns.Length; i++) {
			GunModel currentGun = Guns [i];
			currentGun.BulletSpeed = new Vector2 (currentGun.BulletSpeed.x, bulletSpeed.y);
//				bulletSpeed;
		}
	}

	void SetGunActive(string GunID, bool isActive) {
		for (int i = 0; i < Guns.Length; i++) {
			GunModel currentGun = Guns [i];
			if (Guns[i].Gun_Id == GunID) {
				currentGun.Active = isActive;
				return;
			}
		}
	}


	#endregion


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
		IsPlaying = false;

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

	#region Event Listeners
	void OnShieldPowerEvent(ShieldPowerEvent eve ){
		_CurrentInvulnerableTime = eve.ShieldTime;
	}

	void OnBulletBoostEvent(BulletBoostPowerEvent eve) {
		// add to all gun
		for (int i = 0; i < Guns.Length; i++) {
			GunModel currentGunModel = Guns [i];

			currentGunModel.BoostDelay = eve.BoostDelay;
			currentGunModel.BoostTime = eve.BoostTime;
		}
	}
		
	void OnSpeedBoostPowerEvent(SpeedBoostPowerEvent eve) {
		_SpeedboostPowerTime = eve.BoostTime;
	}

	void OnSpreadGunPowerEvent(SpreadGunPowerEvent eve) {
		_SpreadGunPowerTime = eve.BoostTime;
	}

	void OnShadowAircraftPowerEvent(ShadowAircraftPowerEvent eve) {
		_ShadowAircraftPowerTime = eve.BoostTime;
	}

	void OnLaserPowerEvent(LaserPowerEvent eve) {
		_LaserPowerTime = eve.BoostTime;
	}

	#endregion

	#region Powerups update

	void UpdateSpeedboostPower() {
		if (_SpeedboostPowerTime > 0) {
			_SpeedboostPowerTime -= Time.deltaTime;
			_SpeedboostMultiplier = 2;
		} else {
			_SpeedboostMultiplier = 1;
		}
	}

	void UpdateSpreadGunPower() {
		if (_SpreadGunPowerTime > 0) {	
			_SpreadGunPowerTime -= Time.deltaTime;

			// shooting diagonally

			// turn on spreadgun
			if (!_SpreadGunActive) {
				SetSpreadGunActive (true);
			}

		} else {
			// turn off spreadgun
			if (_SpreadGunActive) {
				SetSpreadGunActive (false);
			}

		}
	}
		
	void UpdateShadowAircraftPower() {
		if (_ShadowAircraftPowerTime > 0) {
			_ShadowAircraftPowerTime -= Time.deltaTime;

			// turn on shadow aircraft
			if (!_ShadowAircraftActive) {
				SetShadowAircraftActive (true);
			}

		} else {
			// turn off shadow aircraft
			if (_ShadowAircraftActive) {
				SetShadowAircraftActive (false);
			}
		}
	}

	void UpdateLaserPower() {
		if (_LaserPowerTime > 0) {
			_LaserPowerTime -= Time.deltaTime;

			// turn on laser
			if (!_LaserActive) {
				SetLaserActive (true);
			}

		} else {
			// turn off laser
			if (_LaserActive) {
				SetLaserActive (false);
			}
		}
	}

	#endregion

	#region Powerups setup
	void SetSpreadGunActive(bool setActive) { 
		SetGunActive ("SpreadGunRight", setActive);
		SetGunActive ("SpreadGunLeft", setActive);
		_SpreadGunActive = setActive;
	}

	void SetShadowAircraftActive(bool setActive) {
		ShadowAircraft.SetActive (setActive);
		_ShadowAircraftActive = setActive;
	}

	void SetLaserActive(bool setActive) {
		LaserObj.SetActive (setActive);
		_LaserActive = setActive;
	}
	#endregion

}

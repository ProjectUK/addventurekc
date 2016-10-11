using UnityEngine;
using System.Collections;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class LoginGPlay : MonoBehaviour {

	[SerializeField] SplashController _SplashController;

	// Use this for initialization
	void Start () {
		// authenticate user:
		PlayGamesPlatform.Activate();
		Social.localUser.Authenticate((bool success) => {
			_SplashController.Running = true;
		});
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

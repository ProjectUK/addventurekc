using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour {

	public string NextSceneName = "";
	public float WaitTime = 2;
	public float UninterruptedTime = 1.3f;

	private float _RealEndTime;
	private float _RealUninterruptedTime;

	public Image LoadingBar;

	public bool _ActivatingNextScene = false;

	public bool Running;


	// Use this for initialization
	void Start () {
		_ActivatingNextScene = false;
		_RealEndTime = Time.realtimeSinceStartup + WaitTime;
		_RealUninterruptedTime = Time.realtimeSinceStartup + UninterruptedTime;
	}
	
	// Update is called once per frame
	void Update () {

		if (Running) {
			if (Time.realtimeSinceStartup > _RealEndTime) {
				LoadNextScene (NextSceneName);
			} else {
				
				if (Time.realtimeSinceStartup > _RealUninterruptedTime) {
					if (Input.GetMouseButtonDown(0)) {
						LoadNextScene (NextSceneName);
					}
				}
			}
		}

	}

	void LoadNextScene(string nextSceneName) {
		if (!_ActivatingNextScene) {
			StartCoroutine(AsynchronousLoad(nextSceneName));
			_ActivatingNextScene = true;
		}
	}

	IEnumerator AsynchronousLoad (string scene)
	{
		yield return null;

		AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
		ao.allowSceneActivation = false;

		while (! ao.isDone)
		{
			// [0, 0.9] > [0, 1]
			float progress = Mathf.Clamp01(ao.progress / 0.9f);
			Debug.Log("Loading progress: " + (progress * 100) + "%");

//			ProgressLoadingText.text = "Loading " + (progress * 100).ToString() + "%";
			if (LoadingBar != null) {
				LoadingBar.fillAmount = progress;
			}

			// Loading completed
			if (ao.progress == 0.9f)
			{
				//				Debug.Log("Press a key to start");
				//				if (Input.anyKey)
				ao.allowSceneActivation = true;
			}

			yield return null;
		}
	}
}

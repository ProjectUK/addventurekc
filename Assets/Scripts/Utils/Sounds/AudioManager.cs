/******
 *	AudioManager 
 *	by: @igrir
 *	last updated: 29 June 2016
 * 	
 * 	Modified version of particle manager for audio
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour {

	public bool IsBgmPlay = true;
	public bool IsSfxPlay = true;

	//Resource path
	private const string MUSIC_PATH = "Music/";
	
	private static AudioManager s_Instance = null;

	public enum AudioType{
		DEFAULT = 0,
		SFX = 1,
		BGM = 2
	}

	[System.Serializable]
	public struct AudioPool {
		public AudioClip 	audioClip;
		public string 		name;
		public int 			poolNum;
		public bool 		isExpanding;
		public AudioType	audioType;
	}
	
	//list of sounds added by user
	public List<AudioPool> AudioPrefabs = new List<AudioPool>();
	
	//dictionary of sounds
	private static Dictionary<string, AudioPool> m_AudioDictionary = new Dictionary<string, AudioPool>();
	
	//dictionary of sounds' pool
	private static Dictionary<string, List<AudioSource>> m_AudioPool = new Dictionary<string, List<AudioSource>>();
	
	//list of running objects
	//TODO: Running objects placed here? [In consideration]
	private static List<AudioSource> m_ActiveSound = new List<AudioSource>();
	private static List<AudioSource> m_PauseSound = new List<AudioSource>();
	
	private bool m_IsPause = false;
	
	public delegate void _OnUnspawn(GameObject go);
	public _OnUnspawn OnUnspawn;
	
	// instance getter
	public static AudioManager Instance {
		get{
			if (s_Instance == null) {
				s_Instance = GameObject.FindObjectOfType(typeof(AudioManager)) as AudioManager;

			}

			return s_Instance;
		}
	}

	void Start() {
			//put all particles to dictionary
			foreach (AudioPool sound in AudioPrefabs) {
				initPool(sound);
			}
	}

	void Awake () {

//		if (s_Instance != null) {
//			// destroy other audiomanager
//			AudioManager[] otherObjects = GameObject.FindObjectsOfType(typeof(AudioManager)) as AudioManager[];
//			foreach(AudioManager am in otherObjects) {
//				if (am != s_Instance) {
//					Debug.Log("Destroy other");
//					Destroy(am.gameObject);
//				}
//			}
//		}
//		
//		//put all particles to dictionary
//		foreach (AudioPool sound in AudioPrefabs) {
//			initPool(sound);
//		}
//
		DontDestroyOnLoad(this.gameObject);

	}
	
	/**
	 * Create the prefabs for the pool
	 */
	GameObject SpawnForPool (AudioPool audioPool, List<AudioSource> pool) {
		GameObject instance = new GameObject(audioPool.audioClip.name);
		//disable this at the moment
		instance.SetActive(false);
		
		//add the ParticleClip so it can accessed by this ParticleManager
		AudioSource audioSource = instance.AddComponent<AudioSource>();
		audioSource.clip = audioPool.audioClip;

		AudioManagerClip amClip = instance.AddComponent<AudioManagerClip>();
		amClip.audioType = audioPool.audioType;

		pool.Add(audioSource);
		
		//set the parent to this (ParticleManager's) game object
		//so our hierarchy looks clean
		instance.transform.parent = this.gameObject.transform;
		
		//we don't want our pool instance destroyed when we change the scene
//		DontDestroyOnLoad(instance);
		
		
		return instance;
	}

	/**
	 * Spawn the object, set the position, and let it die after some seconds
	 */
	public GameObject Play(string soundName, bool isLoop, float volume, float fadingTime) {
		return Play(soundName, isLoop, volume, fadingTime, false);
	}

	public GameObject Play(string soundName, bool isLoop, float volume, float fadingTime, bool once) {
		
		if (m_IsPause) return null;
		
		//get the Particle info from dictionary
		AudioPool audioPool;
		if (m_AudioDictionary.TryGetValue(soundName, out audioPool))
		{
			//get the list from the pool dictionary
			List<AudioSource> pool;
			GameObject spawnObject = null;
			if (m_AudioPool.TryGetValue(soundName, out pool))
			{

				//search the inactive game object
				bool found = false;
				int i = 0;
				while (!found && i < pool.Count)
				{
					GameObject currentGameObject = pool[i].gameObject;
					if (!currentGameObject.activeInHierarchy) {
						spawnObject = currentGameObject;
						found = true;

					} else {
						// check it's active but being silent
						AudioManagerClip amClip = currentGameObject.GetComponent<AudioManagerClip>();


						// Don't play at all if it's once and found someone playing
						if (once)
						{
							if (amClip.GetVolume() > 0f || !amClip.GetAudioSource().mute) {
								return null;
							}
						}

						if (amClip.GetVolume() <= 0f || amClip.GetAudioSource().mute) {
							spawnObject = currentGameObject;
							found = true;
						}
					}
					i++;
				}
				
				//check the game object available
				if (spawnObject != null)
				{

					// find other playing audio if it's only once
					if (once)
					{
						//search the inactive game object
						bool foundPlaying = false;
						int itPlaying = 0;
						while (!foundPlaying && itPlaying < pool.Count) {
							GameObject currentGameObject = pool[itPlaying].gameObject;
							// check it's active but being silent
							AudioManagerClip playingClip = currentGameObject.GetComponent<AudioManagerClip>();
							if (playingClip.GetVolume() <= 0f || playingClip.GetAudioSource().mute) {
								foundPlaying = true;
							}
							itPlaying++;
						}

						if (foundPlaying)
							return null;
					}

					AudioManagerClip amClip = spawnObject.GetComponent<AudioManagerClip>();

					// if it's BGM, check the BGM play and
					// if it's SFX, check the SFX play
					if ( (amClip.audioType == AudioType.BGM && IsBgmPlay) ||
					     (amClip.audioType == AudioType.SFX && IsSfxPlay)) {
						
						AudioSource audioSource = spawnObject.GetComponent<AudioSource>();
						//set the GameObject active
						spawnObject.SetActive(true);

						audioSource.Play();

						// check muted
						if (amClip.audioType == AudioType.BGM) {
							if (GameSaveManager.Instance.IsBGMPlay ()) {
								amClip.Unmute ();
							} else {
								amClip.Mute ();
							}
						}
						if (amClip.audioType == AudioType.SFX) {
							if (GameSaveManager.Instance.IsSFXPlay ()) {
								amClip.Unmute ();
							} else {
								amClip.Mute ();
							}
						}


						if (fadingTime > 0)
							amClip.Fade (volume, fadingTime);
						else
							amClip.SetVolume(volume);

						audioSource.loop = isLoop;
						
						//add the spawned object to our active list
						m_ActiveSound.Add(audioSource);
						
					}
					
					//return the succeedly found object. YAY!
					return spawnObject;
				}else{
					
					//create new object for particle pool
					if (audioPool.isExpanding) {
						GameObject go = SpawnForPool(audioPool, pool);
						return go;
					}else{
						Debug.LogWarning("No inactive gameobjects for " + soundName);
						
						return null;
					}
					
				}
				
			} else {
				Debug.LogWarning("No prefabs called "+soundName +" in particle pool dictionary");
				return null;
			}
		}else{
			Debug.LogWarning("No prefabs called " + soundName + " in dictionary");
			return null;
		}
	}

	public void StopAll() {
		//cek inactive sound to... inactive... you see..
		List<AudioSource> tmpAudioSourceList = new List<AudioSource>();
		foreach (AudioSource aSource in m_ActiveSound ) {
			if (aSource != null) {
				aSource.Stop();
				aSource.gameObject.SetActive(false);

				tmpAudioSourceList.Add(aSource);
			}
		}

		foreach(AudioSource aSource in tmpAudioSourceList) {
			//remove from active
			m_ActiveSound.Remove(aSource);
		}
		tmpAudioSourceList.Clear();
	}

	// check whether there's on clip playing
	public bool IsAtLeastOnePlaying(string soundName) {
		//get the Particle info from dictionary
		AudioPool audioPool;
		if (m_AudioDictionary.TryGetValue(soundName, out audioPool)) {
			//get the list from the pool dictionary
			List<AudioSource> pool;
			if (m_AudioPool.TryGetValue(soundName, out pool)) {

				//search the ACTIVE game object
				int i = 0;
				while (i < pool.Count) {
					GameObject currentGameObject = pool[i].gameObject;
					if (currentGameObject.activeInHierarchy) {
						return true;
					}
					i++;
				}
			} else {
				return false;
			}
		}else{
			return false;
		}	
		return false;
	}

	public void SetVolume(AudioType audioType, float volume, float fadingTime) {

		foreach (AudioSource aSource in m_ActiveSound) {
			AudioManagerClip amc = aSource.GetComponent<AudioManagerClip> ();
			if (amc != null) {
				if (amc.audioType == audioType) {
					Debug.Log ("Fade si:" + amc.gameObject.name + "v:" + volume + " fadeTime:" + fadingTime);
					amc.Fade (volume, fadingTime);
				}
			}
		}

	}

	public void SetVolume(string soundName, float volume, float fadingTime) {
		SetVolume (soundName, AudioType.DEFAULT, volume, fadingTime);
	}

	public void SetVolume(string soundName, AudioType audioType, float volume, float fadingTime) {
		//get the Particle info from dictionary
		AudioPool audioPool;
		if (m_AudioDictionary.TryGetValue(soundName, out audioPool)) {
			//get the list from the pool dictionary
			List<AudioSource> pool;
			if (m_AudioPool.TryGetValue(soundName, out pool)) {

				//search the ACTIVE game object
				int i = 0;
				while (i < pool.Count) {
					GameObject currentGameObject = pool[i].gameObject;
					Debug.Log ("CurrentGameObject:::::" + currentGameObject);
					if (currentGameObject.activeInHierarchy) {
						Debug.Log (currentGameObject + "  is activeInHierarchy");
						AudioManagerClip amClip = currentGameObject.GetComponent<AudioManagerClip>();

						// if it's BGM, check the BGM play and
						// if it's SFX, check the SFX play
						if ((amClip.audioType == AudioType.BGM && IsBgmPlay) ||
							(amClip.audioType == AudioType.SFX && IsSfxPlay)) {

							if (audioType != null) {
								if ((audioType == AudioType.BGM && amClip.audioType == AudioType.BGM) ||
									(audioType == AudioType.SFX && amClip.audioType == AudioType.SFX)) {
									// play fading
									amClip.Fade (volume, fadingTime);
								} else {
									// don't care, just change
									amClip.Fade (volume, fadingTime);
								}
							} else {
								// play fading
								amClip.Fade (volume, fadingTime);
							}
						} else {
							// just fade
							amClip.Fade (volume, fadingTime);
						}
					}
					i++;
				}
			} else {
				Debug.LogWarning("No prefabs called "+soundName +" in particle pool dictionary");
			}
		}else{
			Debug.LogWarning("No prefabs called " + soundName + " in dictionary");
		}	
	}

	void Update () {


		List<AudioSource> tmpAudioSourceList = new List<AudioSource>();
		//cek inactive sound to... inactive... you see..
		foreach (AudioSource aSource in m_ActiveSound ) {
			if (aSource != null) {
				//disable the one playing audio when end
				if (aSource.isPlaying == false && !aSource.loop) {
					tmpAudioSourceList.Add(aSource);
					aSource.gameObject.SetActive(false);

				}
			}
		}

		foreach(AudioSource aSource in tmpAudioSourceList) {
			//remove from active
			m_ActiveSound.Remove(aSource);
		}
		tmpAudioSourceList.Clear();


	}

	/**
		 * Init pool 
		 */
	private void initPool(AudioPool audioPool) {
		
		//check there was no same name for particle
		if (m_AudioDictionary.ContainsKey(audioPool.name)) {
			Debug.LogWarning("A sound with name '"+audioPool.name+"' has already been added");
		}else{
			//add to dictionary
			m_AudioDictionary.Add(audioPool.name, audioPool);
			
			//initialize the GameObjects for pooling later
			List<AudioSource> list = new List<AudioSource>();
			for (int i = 0; i < audioPool.poolNum; i++) {
				SpawnForPool(audioPool, list);
			}
			
			//add to particle pool
			m_AudioPool.Add(audioPool.name, list);
		}
	}


	private void DestroyAllObject() {
		foreach(KeyValuePair<string, List<AudioSource>> entry in m_AudioPool)
		{
			List<AudioSource> audioList = entry.Value;

			foreach(AudioSource audioSource in audioList) {
				Destroy(audioSource.gameObject);
			}

		}
	}

	public void ClearAll() {
		
		foreach(KeyValuePair<string, List<AudioSource>> entry in m_AudioPool) {
			List<AudioSource> audioList = entry.Value;
			
			foreach(AudioSource audioSource in audioList) {
				if (audioSource.gameObject != null) {
					DestroyImmediate(audioSource.gameObject);
				}
			}
		}

		m_AudioPool.Clear ();
		m_AudioDictionary.Clear ();
		m_ActiveSound.Clear ();
		m_PauseSound.Clear ();
	}
	
	void OnDestroy() {
		ClearAll();	
	}


	public void WaitAndPlay(float waitTime, string soundName, bool isLoop, float volume, float fadingTime) {
		StartCoroutine (IEWaitAndPlay (waitTime, soundName, isLoop, volume, fadingTime));
	}

	IEnumerator IEWaitAndPlay(float waitTime, string soundName, bool isLoop, float volume, float fadingTime) {
		yield return StartCoroutine (IndieTime.Instance.WaitForSeconds (waitTime));
		Play (soundName, isLoop, volume, fadingTime);
	}


	public void Mute(AudioType audioType) {

		foreach (AudioSource aSource in m_ActiveSound) {
			AudioManagerClip amc = aSource.GetComponent<AudioManagerClip> ();
			if (amc != null) {
				if (amc.audioType == audioType) {
					amc.Mute ();
				}
			}
		}

	}

	public void Unmute(AudioType audioType) {

		foreach (AudioSource aSource in m_ActiveSound) {
			AudioManagerClip amc = aSource.GetComponent<AudioManagerClip> ();
			if (amc != null) {
				if (amc.audioType == audioType) {
					amc.Unmute ();
				}
			}
		}

	}

	
}


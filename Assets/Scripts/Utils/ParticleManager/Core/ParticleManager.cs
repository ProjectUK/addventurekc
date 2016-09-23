/**
 * Class for handling load particles (or maybe any other prefabs?) using pool
 * last update: @igrir (29/April/2015)
 * 
 * How to use:
 * 1. Drag the prefabs of particle
 * 2. Call this manager and run Spawn() function
 * 
 * convention
 * s_ : static
 * m_ : member
 * 
 * TODO:
 * - Handle the Xffect and Unity's ParticleSystem difference
 * 
 * 
 */ 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Tinker {
	public class ParticleManager : MonoBehaviour {
		
		//Resource path
		private const string PARTICLE_PATH = "Particle/";
		
		private static ParticleManager s_Instance = null;
		
		[System.Serializable]
		public struct ParticlePool {
			public GameObject 	particlePrefab;
			public string 		name;
			public int 			poolNum;
			public bool 		isExpanding;
		}
		
		//list of particles added by user
		public List<ParticlePool> ParticlePrefabs = new List<ParticlePool>();
		
		//dictionary of particles
		private Dictionary<string, ParticlePool> m_ParticleDictionary = new Dictionary<string, ParticlePool>();
		
		//dictionary of particles' pool
		private Dictionary<string, List<ParticleClip>> m_ParticlePool = new Dictionary<string, List<ParticleClip>>();
		
		//list of running objects
		//TODO: Running objects placed here? [In consideration]
		private List<ParticleClip> m_ActiveParticle = new List<ParticleClip>();
		
		private bool m_IsPause = false;

		public delegate void _OnUnspawn(GameObject go);
		public _OnUnspawn OnUnspawn;
		
		// instance getter
		public static ParticleManager Instance {
			get{
				if (s_Instance == null) {
					s_Instance = GameObject.FindObjectOfType(typeof(ParticleManager)) as ParticleManager;
				}
				return s_Instance;
			}
		}
		
		void Awake () {

			Debug.Log ("Particle manager awake");
			//put all particles to dictionary
			foreach (ParticlePool particle in ParticlePrefabs) {
				initPool(particle);
			}

		}
		
		/**
		 * Create the prefabs for the pool
		 */
		GameObject SpawnForPool (GameObject prefab, List<ParticleClip> pool) {
			GameObject instance = Instantiate(prefab);
			//disable this at the moment
			instance.SetActive(false);
			
			//add the ParticleClip so it can accessed by this ParticleManager
			ParticleClip particleClip = instance.GetComponent<ParticleClip>();
			pool.Add(particleClip);
			
			//set the parent to this (ParticleManager's) game object
			//so our hierarchy looks clean
			instance.transform.parent = this.gameObject.transform;

			
			
			return instance;
		}
		
		public void OnLevelWasLoaded() {
			//removeInstanceDuplicate();
		}
		
		/**
		 * Prevent the instance duplicated
		 */
		private void removeInstanceDuplicate() {
			ParticleManager[] duplicates = GameObject.FindObjectsOfType<ParticleManager>();
			
			if (duplicates.Length > 0) {
				//destroy all gameobject except this
				foreach ( ParticleManager pm in duplicates ) {
					GameObject go = pm.gameObject;
					
					//prevent destroying this
					if (go != gameObject) {
						DestroyImmediate(go);
					}
				}
			}
		}
		
		/**
		 * Spawn the object from library
		 */
		public GameObject Spawn(string particleName) {
			GameObject spawnedGameObject = Spawn(particleName, new Vector3(0,0,0), 0);
			
			return spawnedGameObject;			
			
		}
		
		/**
		 * Spawn the object and set the parent
		 */
		public GameObject Spawn(string particleName, Vector3 position) {
			GameObject spawnedGameObject = Spawn(particleName, position, 0);
			return spawnedGameObject;
		}
		
		/**
		 * Spawn the object, set the parent, and let it die after some seconds
		 */
		public GameObject Spawn(string particleName, GameObject parent, float dieAfterSecond) {
			
			GameObject spawnedGameObject = Spawn(particleName, parent.transform.position, dieAfterSecond);
			if (spawnedGameObject != null) {
				//set the parent
				spawnedGameObject.GetComponent<ParticleClip>().Parent = parent;
			}
			
			return spawnedGameObject;
			
		}
		
		
		/**
		 * Spawn the object, set the position, and let it die after some seconds
		 */
		public GameObject Spawn(string particleName, Vector3 position, float dieAfterSecond) {
			
			if (m_IsPause) return null;
			
			//get the Particle info from dictionary
			ParticlePool particle;
			if (m_ParticleDictionary.TryGetValue(particleName, out particle)) {
				//get the list from the pool dictionary
				List<ParticleClip> pool;
				GameObject spawnObject = null;
				if (m_ParticlePool.TryGetValue(particleName, out pool)) {
					
					//search the inactive game object
					bool found = false;
					int i = 0;
					while (!found && i < pool.Count) {
						GameObject currentGameObject = pool[i].gameObject;
						if (!currentGameObject.activeInHierarchy) {
							spawnObject = currentGameObject;
							found = true;
						}
						i++;
					}
					
					//check the game object available
					if (spawnObject != null) {
						
						//set position if exist
						spawnObject.transform.position = position;
						
						ParticleClip pc = spawnObject.GetComponent<ParticleClip>();
						pc.Reset();
						
						//set the die time
						if (pc.DieTime <= 0) {
							pc.DieTime = dieAfterSecond;
						}

						//set the GameObject active
						spawnObject.SetActive(true);
						
						//add the spawned object to our active list
						m_ActiveParticle.Add(pc);
						
						//return the succeedly found object. YAY!
						return spawnObject;
					}else{
						
						//create new object for particle pool
						if (particle.isExpanding) {
							GameObject go = SpawnForPool(particle.particlePrefab, pool);
							return go;
						}else{
							Debug.LogError("No inactive gameobjects for " + particleName);
							
							return null;
						}
						
					}
					
				} else {
					Debug.LogError("No prefabs called "+particleName +" in particle pool dictionary");
					return null;
				}
			}else{
				Debug.LogError("No prefabs called " + particleName + " in dictionary");
				return null;
			}
			
			
		}
		
		void Update () {
			
		}
		
		/**
		 * Create pool using game object as the reference
		 */
		public void CreatePool(GameObject prefab, string name, int poolNum, bool isExpanding) {
			
			ParticlePool newParticle = new ParticlePool();
			newParticle.particlePrefab = prefab;
			newParticle.name = name;
			newParticle.poolNum = poolNum;
			newParticle.isExpanding = isExpanding;
			
			ParticlePrefabs.Add(newParticle);
			
			initPool(newParticle);
		}
		
		/**
		 * Create pool using resources load to find the reference and set the name
		 */
		public void CreatePool(string prefabName, string name, int poolNum, bool isExpanding) {
			GameObject go = Resources.Load(PARTICLE_PATH + prefabName) as GameObject;
			CreatePool(go, name, poolNum, isExpanding);
		}
		
		/**
		 * Create pool using resources load to find the reference
		 */
		public void CreatePool(string prefabName, int poolNum, bool isExpanding) {
			GameObject go = Resources.Load(PARTICLE_PATH + prefabName) as GameObject;
			CreatePool(go, go.name, poolNum, isExpanding);
		}
		
		public void DestroyPool(string poolName) {
			
			ParticlePool particle;
			
			//find the particles
			if( m_ParticleDictionary.TryGetValue(poolName, out particle) ){
				
				//find the particles' pool
				List<ParticleClip> particlePool = null;
				if (m_ParticlePool.TryGetValue(poolName, out particlePool)) {
					
					//remove the particle from list
					foreach(ParticlePool p in ParticlePrefabs){
						if (p.Equals(particle)) {
							ParticlePrefabs.Remove(p);
							break;
						}
					}
					
					//destroy all the objects in pool
					foreach (ParticleClip pc in particlePool) {
						DestroyImmediate(pc.gameObject);
					}
					
					//remove the particle in dictionary
					m_ParticleDictionary.Remove(poolName);
					//remove the particle in pool
					m_ParticlePool.Remove(poolName);
					
					
				}else{
					Debug.LogError("There was no particle pool called '"+ poolName + "'");
				}
				
			}else{
				Debug.LogError("There was no particle called '"+ poolName+"'");
			}
		}
		
		/**
		 * Init pool 
		 */
		private void initPool(ParticlePool particle) {
			
			//check there was no same name for particle
			if (m_ParticleDictionary.ContainsKey(particle.name)) {
				Debug.LogWarning("A particle with name '"+particle.name+"' has already been added");
			}else{
				//add to dictionary
				m_ParticleDictionary.Add(particle.name, particle);
				
				//initialize the GameObjects for pooling later
				List<ParticleClip> list = new List<ParticleClip>();
				for (int i = 0; i < particle.poolNum; i++) {
					SpawnForPool(particle.particlePrefab, list);
					
				}
				
				//add to particle pool
				m_ParticlePool.Add(particle.name, list);
			}
		}
		
		private void Recycle(GameObject gameObject) {
			gameObject.SetActive(false);
		}
		
		/**
		 * Check whether the key "name" exist
		 */
		public bool IsExist(string name) {
			bool isExist = false;
			
			//remove the particle from list
			foreach(ParticlePool p in ParticlePrefabs){
				if (p.name == name) {
					isExist = true;
					break;
				}
			}
			
			return isExist;
			
		}
		
		
		/**
		 * Get pause state
		 */
		public bool GetPause() {
			return m_IsPause;
		}
		
		/**
		 * Pause all particle
		 */
		public void SetPause(bool pause) {
			
			m_IsPause = pause;
			
			foreach(ParticleClip pc in m_ActiveParticle) {
				pc.SetPause(pause);
			}
			
		}
		
		/**
		 * Play all particle
		 */
		public void Play() {
			
			foreach(ParticleClip pc in m_ActiveParticle) {
				pc.Play();
			}
			
		}
		
		public void UnspawnParticle(GameObject gameObject) {
			//check has ParticleClip
			ParticleClip particleClip = gameObject.GetComponent<ParticleClip>();
			if (particleClip != null) {
				gameObject.SetActive(false);
				particleClip.Reset();
				
				//remove the particle from the active list
				m_ActiveParticle.Remove(particleClip);
			}else{
				Debug.LogError("The object to be unspawned doesn't have ParticleClip component");
			}
		}
		
		/**
		 * Unspawn specified particle
		 */
		public void UnspawnParticle(string particleName) {
			//get the list of the given name
			List<ParticleClip> particleList;
			if (m_ParticlePool.TryGetValue(particleName, out particleList)) {
				foreach(ParticleClip pc in particleList) {
					pc.gameObject.SetActive(false);
				}
			}
		}
		
		/**
		 * Unspawn all particle
		 */
		public void UnspawnAll() {
			foreach(KeyValuePair<string, List<ParticleClip>> entry in m_ParticlePool) {
				List<ParticleClip> particleClipList = entry.Value;
				
				foreach(ParticleClip pc in particleClipList) {
					if (pc != null & pc.gameObject.activeInHierarchy) {
						pc.gameObject.SetActive(false);
					}
				}
			}
		}

		/**
		 * Unspawn all particle
		 */
		public void UnspawnAll(_OnUnspawn onUnspawn) {
			foreach(KeyValuePair<string, List<ParticleClip>> entry in m_ParticlePool) {
				List<ParticleClip> particleClipList = entry.Value;
				
				foreach(ParticleClip pc in particleClipList) {
					if (pc != null & pc.gameObject.activeInHierarchy) {
						pc.gameObject.SetActive(false);
						onUnspawn(pc.gameObject);
					}
				}
			}
		}

		
	}
	
	public static class ParticleManagerExtensions {
		
		public static void UnspawnParticle(this GameObject gameObject){
			ParticleManager.Instance.UnspawnParticle(gameObject);
		}
		
	}
	
}
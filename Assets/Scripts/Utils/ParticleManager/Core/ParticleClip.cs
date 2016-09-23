//if XFFECT exist, write #define XFFECT
#define XFFECT

using UnityEngine;
using System.Collections;

namespace Tinker {
	public abstract class ParticleClip : MonoBehaviour{
		
		public float DieTime;
		protected float _Time;
		
		public GameObject Parent;
		
		public void Reset () {
			_Time = 0;
			Parent = null;
			Play ();
		}
		
		public void Update () {
			
			if (!Tinker.ParticleManager.Instance.GetPause()) {

				if (DieTime > 0) {
					if ( _Time <  DieTime) {
						_Time += Time.deltaTime;
					}else{
						Tinker.ParticleManager.Instance.UnspawnParticle(this.gameObject);
					}
				}

			}
			
			//follow parent if it has parent
			if (Parent != null) {
				transform.position = Parent.transform.position;
			}
			
		}
		
		abstract public void SetPause(bool isPause);
		abstract public void Play();
		
		/*
		public void Play () {
			ParticleSystem[] ps = GetComponents<ParticleSystem>();
			if (ps != null) {
				foreach(ParticleSystem currentPs in ps){
					currentPs.Play();
				}
			}

#if XFFECT
			//TODO: Check XFTS is exist
			Xft.XffectComponent[] xfts = GetComponents<Xft.XffectComponent>();
			if (xfts != null) {
				foreach(Xft.XffectComponent currentXfts in xfts) {
					currentXfts.Paused = false;
				}
			}

#endif
		}*/
		
		/*
		public void SetPause (bool isPause) {
			ParticleSystem[] ps = GetComponents<ParticleSystem>();
			if (ps != null) {
				foreach(ParticleSystem currentPs in ps){
					if (isPause) {
						if (currentPs.isPlaying) {
							currentPs.Pause();
						}
					}else{
						if (currentPs.isPaused) {
							currentPs.Play();
						}
					}
					
				}
			}

#if XFFECT
			//TODO: Check XFTS is exist
			Xft.XffectComponent[] xfts = GetComponents<Xft.XffectComponent>();
			if (xfts != null) {
				foreach(Xft.XffectComponent currentXfts in xfts) {
					if (isPause) {
						if (!currentXfts.Paused) {
							currentXfts.Paused = true;
						}
					}else{
						if (currentXfts.Paused) {
							currentXfts.Paused = false;
						}
					}
				}
			}

#endif
		}
		*/
		
	}
}
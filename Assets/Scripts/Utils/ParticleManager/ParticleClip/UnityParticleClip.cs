using UnityEngine;
using System.Collections;

namespace Tinker {
	public class UnityParticleClip : ParticleClip {

		ParticleSystem[] ps;

		public void Start() {
			ps = GetComponents<ParticleSystem>();
		}

		
		public override void Play () {
			if (ps != null) {
				foreach(ParticleSystem currentPs in ps){
					currentPs.Play();
				}
			}
			
		}

		public override void SetPause (bool isPause) {
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
		}

	}
}
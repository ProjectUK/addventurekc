using UnityEngine;
using System.Collections;

public class CameraShakeEvent : GameEvent {
	public float Duration;
	public float Strength;
	public int Vibrato;
	public float Randomness;

	public CameraShakeEvent(float duration, float strength, int vibrato, float randomness) {
		this.Duration = duration;
		this.Strength = strength;
		this.Vibrato = vibrato;
		this.Randomness = randomness;
	}

}

using UnityEngine;
using System.Collections;

public class EnemyDeadEvent : GameEvent {
	public EnemyController.AircraftType AircraftType;
	public EnemyDeadEvent(EnemyController.AircraftType aircraftType) {
		this.AircraftType = aircraftType;
	}
}

public class BossStartEvent : GameEvent {
	public BossStartEvent() {
		
	}
}

public class BossDeadEvent : GameEvent {
	public BossDeadEvent() {
	}
}

public class GameLoseEvent : GameEvent {
	public GameLoseEvent() {
	}
}

public class GameWinEvent : GameEvent {
	public GameWinEvent() {
	}
}

public class CafeActionStartedEvent : GameEvent {
	public CafeActionStartedEvent() {
	}
}
	
public class CafeActionEndedEvent : GameEvent {
	public CafeActionEndedEvent() {
	}
}

public class ReceiveWakeEvent : GameEvent {
	public float WakeValue;
	public ReceiveWakeEvent(float wakeValue) {
		this.WakeValue = wakeValue;
	}
}

public class BombPowerEvent : GameEvent {
	public Vector3 Position;
	public BombPowerEvent(Vector3 position) {
		this.Position = position;
	}
}

public class ShieldPowerEvent : GameEvent {
	public float ShieldTime;
	public ShieldPowerEvent(float shieldTime) {
		this.ShieldTime = shieldTime;
	}
}

public class CoffeePowerEvent : GameEvent {
	public CoffeePowerEvent() {
	}
}

public class BulletBoostEvent : GameEvent {
	public float BoostTime;
	public float BoostDelay;
	public BulletBoostEvent(float boostTime, float delayEffect) {
		this.BoostTime = boostTime;
		this.BoostDelay = delayEffect;
	}
}

public class HideBulletEvent : GameEvent {
	public HideBulletEvent() {
	}
}

public class MainMenuEvent :GameEvent { 
	public MainMenuEvent() {
	}
}

public class StartGameEvent :GameEvent { 
	public StartGameEvent() {
	}
}

public class PauseGameEvent :GameEvent { 
	public PauseGameEvent() {
	}
}

public class ResumeGameEvent :GameEvent { 
	public ResumeGameEvent() {
	}
}

public class PowerUpNotification: GameEvent{
	public string PowerUpId;
	public PowerUpNotification(string powerUpId) {
		this.PowerUpId = powerUpId;
	}
}

#region Aircraft Option

public class AircraftSetControlSwipeEvent:GameEvent{
	public bool IsSwipe;
	public AircraftSetControlSwipeEvent(bool isSwipe) {
		this.IsSwipe = isSwipe;
	}
}

#endregion
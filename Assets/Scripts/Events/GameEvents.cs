using UnityEngine;
using System.Collections;

public class EnemyDeadEvent : GameEvent {
	public EnemyController EnemyController;
	public EnemyDeadEvent(EnemyController enemyController) {
		this.EnemyController = enemyController;
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

#region Power Up Events

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

public class BulletBoostPowerEvent : GameEvent {
	public float BoostTime;
	public float BoostDelay;
	public BulletBoostPowerEvent(float boostTime, float delayEffect) {
		this.BoostTime = boostTime;
		this.BoostDelay = delayEffect;
	}
}

public class SpeedBoostPowerEvent : GameEvent { 
	public float BoostTime;
	public SpeedBoostPowerEvent(float boostTime) {
		this.BoostTime = boostTime;
	}
}

public class SpreadGunPowerEvent : GameEvent {
	public float BoostTime;
	public SpreadGunPowerEvent(float boostTime) {
		this.BoostTime = boostTime;
	}
}

public class ShadowAircraftPowerEvent : GameEvent {
	public float BoostTime;
	public ShadowAircraftPowerEvent(float boostTime) {
		this. BoostTime = boostTime;
	}
}

public class LaserPowerEvent: GameEvent{
	public float BoostTime;
	public LaserPowerEvent(float boostTime) {
		this. BoostTime = boostTime;
	}
}

#endregion

public class HideBulletEvent : GameEvent {
	public HideBulletEvent() {
	}
}

public class MainMenuEvent :GameEvent { 
	public MainMenuEvent() {
	}
}

public class StartGameEvent :GameEvent { 
	public string PlayerInitial;
	public StartGameEvent() {
	}
	public StartGameEvent(string playerInitial) {
		this.PlayerInitial = playerInitial;
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


#region Shop

public class BuyItemEvent:GameEvent{
	public string ItemName;
	public BuyItemEvent(string itemName) {
		this.ItemName = itemName;
	}
}

public class DetailItemEvent: GameEvent{ 
	public ShopItemModel ShopItem;
	public DetailItemEvent(ShopItemModel shopItem) {
		this.ShopItem = shopItem;
	}
}

#endregion
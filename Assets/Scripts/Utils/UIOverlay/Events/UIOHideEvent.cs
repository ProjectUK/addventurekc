public class UIOHideEvent : GameEvent {
	public string OverlayName;
	public string Message;
	public UIOHideEvent(string overlayName, string message) {
		this.OverlayName = overlayName;
		this.Message = message;
	}
}

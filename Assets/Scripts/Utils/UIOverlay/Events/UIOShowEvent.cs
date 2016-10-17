public class UIOShowEvent : GameEvent {
	public string OverlayName;
	public string Message;
	public UIOShowEvent(string overlayName, string message) {
		this.OverlayName = overlayName;
		this.Message = message;
	}
}

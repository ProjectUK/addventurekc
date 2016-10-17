using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;

public class UISliding : UIOverlay {

	[Header("Sliding Properties")]
	public RectTransform TargetRect;
	public RectTransform StartPos;
	public RectTransform EndPos;

	public override void Start ()
	{
		base.Start ();
	}

	public override void Show (string message)
	{
		TargetRect.DOMove(StartPos.position, 1, false).SetEase(Ease.OutBounce).SetUpdate(true);

		EventManager.Instance.TriggerEvent (new UIOShowEvent (UIOName, message));
	}

	public override void Hide (string message)
	{
		TargetRect.DOMove(EndPos.position, 0.5f, false).SetEase(Ease.InQuad).SetUpdate(true);

		EventManager.Instance.TriggerEvent (new UIOHideEvent (UIOName, message));
	}

}

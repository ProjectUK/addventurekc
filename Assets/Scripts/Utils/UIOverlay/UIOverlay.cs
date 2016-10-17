using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIOverlay : MonoBehaviour {

	// DON'T FORGET TO SET THIS BUDDY, FOLKS!
	protected string UIOName;

	protected CanvasGroup _CanvasGroup;

	private bool _IsHidden;
	public bool IsHidden{
		get{
			return _IsHidden;
		}
	}

	public virtual void Start() {
		_CanvasGroup = GetComponent<CanvasGroup> ();
		EventManager.Instance.AddListener<UIOShowEvent> (OnUIOShowEvent);
	}

	public virtual void Show(string message) {
		PopulateCanvasGroup ();
		_CanvasGroup.alpha = 1;
        _CanvasGroup.interactable = true;
        _CanvasGroup.blocksRaycasts = true;
		_IsHidden = false;

		EventManager.Instance.TriggerEvent (new UIOShowEvent (UIOName, message));
	}

	public void Show() {
		Show ("");
	}

	public virtual void Hide(string message) {
		PopulateCanvasGroup ();
		_CanvasGroup.alpha = 0;
        _CanvasGroup.interactable = false;
        _CanvasGroup.blocksRaycasts = false;	
		_IsHidden = true;

		EventManager.Instance.TriggerEvent (new UIOHideEvent (UIOName, message));
	}

	public void Hide() {
		Hide ("");
	}

	void PopulateCanvasGroup() {
		if (_CanvasGroup == null)
			_CanvasGroup = GetComponent<CanvasGroup> ();
	}

    public virtual void Activate(bool active, float alpha)
    {
        PopulateCanvasGroup();

        _CanvasGroup.alpha = alpha;
        _CanvasGroup.interactable = false;
        _CanvasGroup.blocksRaycasts = false;
        _CanvasGroup.interactable = active;
        _CanvasGroup.blocksRaycasts = active;
        _IsHidden = !active;
    }

	public virtual void OnUIOShowEvent(UIOShowEvent eve) {
	}
}

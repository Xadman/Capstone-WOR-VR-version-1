using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SpinButton : MonoBehaviour {

	public Sprite enabledSprite;
	public Sprite disabledSprite;
	public Sprite autoSprite;
	public Sprite stopSprite;


	Slot slot;

	private Button button;

	void Start() {
		button = GetComponent<Button>();
		slot = GameObject.Find ("BeachDays").GetComponent<Slot>();
	}

	void Update() {

		switch (slot.state)
		{
		case SlotState.playingwins:
			
			button.image.sprite = enabledSprite;
			if(slot.refs.credits.canPlaceBet())
				button.interactable = true;
			else
				button.interactable = false;
			break;
		case SlotState.ready:
			button.image.sprite = enabledSprite;
			if(slot.refs.credits.canPlaceBet())
				button.interactable = true;
			else
				button.interactable = false;
			break;
		case SlotState.snapping:
			button.image.sprite = disabledSprite;
			button.interactable = false;
			break;
		case SlotState.spinning:
			button.image.sprite = stopSprite;
			break;
		}
	}
}


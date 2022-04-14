using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MaxBetButton : MonoBehaviour {

	public Sprite enabledSprite;
	public Sprite disabledSprite;

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
			button.interactable = true;
			break;
		case SlotState.ready:
			button.image.sprite = enabledSprite;
			button.interactable = true;
			break;
		case SlotState.snapping:
			button.image.sprite = disabledSprite;
			button.interactable = false;
			break;
		case SlotState.spinning:
			button.image.sprite = disabledSprite;
			button.interactable = false;
			break;
		}
	}
}


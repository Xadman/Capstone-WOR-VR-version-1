using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Globalization;

public class BeachDaysGUI : MonoBehaviour {

	public Text bet;
	public Text lines;
	public Text won;
	public Text credits;
	public Text winReadout;

	public Text scatter5;
	public Text scatter6;
	public Text scatter7;

	public Leveling leveling;
	public Text level;

	public Scatters scatters;

	Slot slot;
	NumberFormatInfo nfi;

	void Start() {
		slot = GameObject.Find ("BeachDays").GetComponent<Slot>();

		nfi = new CultureInfo( "en-US", false ).NumberFormat;
		nfi.CurrencyDecimalDigits = 0;

		updateUI();
	}

	void Update() {
		switch (slot.state) {
		case SlotState.playingwins:
			if (slot.refs.wins.currentWin == null) return;

			if (slot.refs.wins.isBetweenWins())
				winReadout.text = "";
					else 
					winReadout.text = slot.refs.wins.currentWin.readout.ToString ();
			updateWon();
			updateCredits();
			updateScatters();
			break;
		case SlotState.spinning:
			won.text = "GOOD LUCK!";
			winReadout.text = "";
			updateScatters();
			break;

		default:
			won.text = "";
			winReadout.text = "";
			updateScatters();
			break;
		}
	}

	public void updateUI() {
		updateBet();
		updateWon ();
		updateCredits();
		updateLines();
		updateLevel();
		updateScatters();
	}
	void updateBet() {
		bet.text = slot.refs.credits.totalBet().ToString();
	}
	void updateWon() {
		won.text = slot.refs.credits.lastWin.ToString();
	}
	void updateCredits() {
		credits.text = slot.refs.credits.totalCreditsReadout().ToString();
	}
	void updateLines() {
		lines.text = slot.refs.credits.linesPlayed.ToString();
	}
	void updateLevel() {
		level.text = leveling.GetLevel().ToString();
	}
	void updateScatters() {
		scatter5.text = scatters.GetScatter(0).ToString("C", nfi);
		scatter6.text = scatters.GetScatter(1).ToString("C", nfi);
		scatter7.text = scatters.GetScatter(2).ToString("C", nfi);
	}
}

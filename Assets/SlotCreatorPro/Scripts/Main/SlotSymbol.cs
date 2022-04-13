// Brad Lima - 11/2019
//
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SlotSymbol : MonoBehaviour {

	[HideInInspector]
	public bool linked;
	[HideInInspector]
	public LinkPosition linkPosition;

	[HideInInspector]
	public bool isWild;
	[HideInInspector]
	public int clampPerReel;
	[HideInInspector]
	public int clampTotal;
	[HideInInspector]
	public int symbolIndex;
	[HideInInspector]
	public bool perReelFrequency;
	[HideInInspector]
	public int reelIndex;

}

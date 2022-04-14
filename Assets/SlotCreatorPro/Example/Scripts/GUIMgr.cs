//
// This class will cache the uGUI tree under the object it is added to and create a singleton for accessing them
//
// if you have:
//
// Canvas
//    Group1
//      Subgroup1  
//    Group2
//      Subgroup2
//
//  You can access them by:
//
//  GUIMgr.ins.getUI("Group1/Subgroup1");
//  GUIMgr.ins.getUI("Group2");
//
//  The / is what denotes going down another level
//
//  there are also convience methods like GUIMgr.ins.getText and GUIMgr.ins.getButton
//  these simply attempt to return the type of object directly.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class GUIMgr : MonoBehaviour {

	//NetworkManager networkMgr;

	public Dictionary<string, GameObject> elements = new Dictionary<string, GameObject>();

	private static GUIMgr s_Instance = null;
	public static GUIMgr ins { 
		get {
			if (s_Instance == null) {
				s_Instance =  FindObjectOfType(typeof (GUIMgr)) as GUIMgr;
			}
			
			return s_Instance;
		}
	}
	
	void OnApplicationQuit() {
		s_Instance = null;
	}

	#region _Cache Elements
	void Awake () {
		cacheGui();
	}

	public void cacheGui()
	{
		elements.Clear();
		foreach (Transform rootobj in gameObject.transform)
		{
			elements.Add (rootobj.name, rootobj.gameObject);
		}
		Debug.Log ("GUI indexed " + elements.Count + " items.");
	}

	void addChildren(GameObject obj, string path)
	{
		string oripath = path;
		foreach (Transform objm in obj.transform)
		{
			path = oripath + "/" + objm.name;
			if (elements.ContainsKey(path)) { Debug.Log ("key exists: " + path); }
			elements.Add (path, objm.gameObject);
		}
	}
	#endregion

	#region _Getters
	public GameObject getUI(string name)
	{
		if (elements.ContainsKey(name) == false) { Debug.Log("key not found = " + name); return null; }
		return elements[name];
	}
	public Text getText(string name)
	{
		if (elements.ContainsKey(name) == false) { Debug.Log("key not found = " + name); return null; }
		return elements[name].GetComponent<Text>();
	}
	public InputField getInputField(string name)
	{
		if (elements.ContainsKey(name) == false) { Debug.Log("key not found = " + name); return null; }
		return elements[name].GetComponent<InputField>();
	}
	public Slider getSlider(string name)
	{
		if (elements.ContainsKey(name) == false) { Debug.Log("key not found = " + name); return null; }
		return elements[name].GetComponent<Slider>();
	}
	public Image getImage(string name)
	{
		if (elements.ContainsKey(name) == false) { Debug.Log("key not found = " + name); return null; }
		return elements[name].GetComponent<Image>();
	}
	public Button getButton(string name)
	{
		if (elements.ContainsKey(name) == false) { Debug.Log("key not found = " + name); return null; }
		return elements[name].GetComponent<Button>();
	}

	#endregion

	void Update () {

	}

	#region Activate/Deactivate
	public void activatePanel(string panelName, float fadeInTime = 1f)
	{
		GameObject panel = getUI(panelName);
		if (panel == null) return;
		panel.SetActive(true);

		panel.transform.SetAsLastSibling();
	}
	public void deactivatePanel(string panelName)
	{
		GameObject panel = getUI(panelName);
		if (panel == null) return;
		panel.GetComponent<CanvasGroup>().interactable = false;
	}
	#endregion

}


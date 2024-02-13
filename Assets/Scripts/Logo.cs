using UnityEngine;
using System.Collections;

public class Logo : MonoBehaviour {
	public Texture logo;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI(){
		GUI.Label(new Rect((Screen.width/5)+185,(Screen.height/10)-30,logo.width,logo.height),logo);	
		GameObject.Find("Menu").renderer.material.color = Color.black;
	}
}

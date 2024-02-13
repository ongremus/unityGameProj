using UnityEngine;
using System.Collections;

public class Highlight : MonoBehaviour {
	private Color startcolor;
	public GameObject hl;
	public bool selects = false;
	
	// Use this for initialization
	public void notSelected() {
		hl.renderer.enabled = false;
		selects = false;
	}
	
	
	public void selected(){
		hl.renderer.enabled = true;
		selects = true;
		hl.renderer.material.color = Color.gray;
	}
	void OnMouseEnter(){
		hl.renderer.enabled = true;
		hl.renderer.material.color = Color.gray;
	}
	
	void OnMouseExit(){
		if(!selects)
			hl.renderer.enabled = false;	
	}
	
}

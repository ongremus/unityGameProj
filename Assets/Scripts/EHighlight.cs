using UnityEngine;
using System.Collections;

public class EHighlight : MonoBehaviour {
	public GameObject hl;
	
	void OnMouseEnter(){
		hl.renderer.enabled = true;
		hl.renderer.material.color = Color.red;
	}
	
	void OnMouseExit(){
		hl.renderer.enabled = false;	
	}
	
	void Update(){
		if(this.GetComponent<Unit>().GetSelected()){
			hl.renderer.enabled = true;
			hl.renderer.material.color = Color.red;
		}
	}
}

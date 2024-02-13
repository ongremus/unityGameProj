using UnityEngine;
using System.Collections;

public class Mousecursor : MonoBehaviour {
	public Texture cursorImage;
	// Use this for initialization
	void Start () {
		Screen.showCursor = false;
		if(WinLose.GetInstance().losing || WinLose.GetInstance().winning)
			WinLose.GetInstance().next = false;
	}
	
	void OnGUI(){
		Vector3 mos = Input.mousePosition;
		GUI.Label(new Rect(mos.x,Screen.height - mos.y,cursorImage.width,cursorImage.height),cursorImage);
	}
}

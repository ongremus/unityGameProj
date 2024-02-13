using UnityEngine;
using System.Collections;

public class StageScript : MonoBehaviour {
	private string currentToolTipText = "";
	private string stage = "";
	private GUIStyle guiStyleFore;
	private GUIStyle guiStyleBack;
	private GUIStyle guiStyleFore1;
	private GUIStyle guiStyleBack1;
	
	public Texture guiTexture;
	public Texture2D guiBanner;
	public Font fontFamily;
	
	public bool stage1 = false;
	public bool stage2 = false;
	
	void Start(){
		guiStyleFore1= new GUIStyle();
	    guiStyleFore1.normal.textColor = Color.black;  
	    guiStyleFore1.alignment = TextAnchor.UpperCenter ;
	    guiStyleFore1.wordWrap = true;
	    guiStyleBack1 = new GUIStyle();
	    guiStyleBack1.normal.textColor = Color.white;  
	    guiStyleBack1.alignment = TextAnchor.UpperCenter ;
	    guiStyleBack1.wordWrap = true;
		
		guiStyleFore = new GUIStyle();
		guiStyleFore.fontSize = 45;
		guiStyleFore.font = fontFamily;
	    guiStyleFore.normal.textColor = Color.black;  
	    guiStyleFore.alignment = TextAnchor.UpperCenter ;
	    guiStyleFore.wordWrap = true;
	    guiStyleBack = new GUIStyle();
		guiStyleBack.fontSize = 45;
		guiStyleBack.font = fontFamily;
	    guiStyleBack.normal.textColor = Color.white;  
	    guiStyleBack.alignment = TextAnchor.UpperCenter ;
	    guiStyleBack.wordWrap = true;
	
		stage = "Rise of the Shinobi";
		
		GameObject.Find("Stage2").renderer.enabled = false;
		
		if(stage2 && WinLose.GetInstance().winning){
			WinLose.GetInstance().next = false;
			GameObject.Find("/WinLose/win").audio.Stop();
		}
	}
	
	void OnMouseOver()
	{
		renderer.material.color = Color.red;	
		if(stage1)
			currentToolTipText = "\n\nChapter 1:\nKankuro Village";
		if(stage2 && WinLose.GetInstance().winning){
			//GameObject.Find("Stage2").renderer.enabled = true;
			currentToolTipText = "\n\nChapter 2:\nRivalry Land";
		}
	}
	
	void OnMouseExit()
	{
		renderer.material.color = Color.white;
		currentToolTipText = "";
	}
	
	void OnMouseDown()
	{
		if(stage1)
			AutoFade.LoadLevel(2,2,2,Color.black);
		if(stage2){
			if(WinLose.GetInstance().winning)
			{
				AutoFade.LoadLevel(0,2,2,Color.black);
			}
		}
	}
	
	void OnGUI(){
		float x = Event.current.mousePosition.x;
	    float y = Event.current.mousePosition.y;
		
		if(stage2 && WinLose.GetInstance().winning){
			GameObject.Find("Stage2").renderer.enabled = true;
		}
		
		GUI.DrawTexture (new Rect(268,565,500,100),guiBanner,ScaleMode.StretchToFill, true, 100f);
		GUI.Label (new Rect(268,600,500,100), stage, guiStyleBack);
	    GUI.Label (new Rect(268,600,500,100), stage, guiStyleFore);
		if (currentToolTipText != "")
    	{
			GUI.Label (new Rect(x-125,y,250,300),guiTexture);
	        GUI.Label (new Rect(x-149,y+21,300,60), currentToolTipText, guiStyleBack1);
	        GUI.Label (new Rect (x-150,y+20,300,60), currentToolTipText, guiStyleFore1);
    	}	
	}
}

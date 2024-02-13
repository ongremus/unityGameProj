/*
 * Script to handle all UI in game.
 * Should be attached to a manager game object.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIScript : MonoBehaviour {
	
	private float sW = 0f;	// Screen Width Variable
	private float sH = 0f;	// Screen Height Variable
	private float uiHeight = 0f;
	private float uiWidth = 0f;
	private float uiPosX = 0f;
	private float uiPosY = 0f;
	private float timeRemainingThisStage = 30f;
	
	private int TOTAL_COUNT = 20;
	
	private GUIStyle guiStyleFore1;
	private GUIStyle guiStyleBack1;
	public Texture2D guiBanner;
	public Font fontFamily;
	
	private float uiPosX2 = 0f;
	
	private float uiArea = 0f;
	public GameObject[] selectedUnits;
	private string selectedString;
	
	public GameObject newUnit;
	
	public Texture cursorImage;
	public Texture gui_minimap;
	public Texture gui_char;
	public Texture gui_view;
	
	public Texture ninja_icon;
	
	public Texture health;
	public Texture health_back;
	
	private Texture selTexture;
	private GUIStyle stats;
	
	// Use this for initialization
	void Start () {
		sW = Screen.width;
		sH = Screen.height;
		uiHeight = sH / 4;
		uiWidth = sW;
		
		uiPosY = uiHeight * 3;
		uiPosX2 = uiHeight * 2;
			
		uiArea = uiWidth / 4;
		
		selTexture = null;
		selectedUnits = null;
		
		selectedUnits = UnitManager.GetInstance().GetSelectedUnitsArray();
		Screen.showCursor = false;
		
		guiStyleFore1= new GUIStyle();
		guiStyleFore1.fontSize = 15;
	    guiStyleFore1.normal.textColor = Color.black;  
	    guiStyleFore1.alignment = TextAnchor.UpperCenter ;
	    guiStyleFore1.wordWrap = true;
	    guiStyleBack1 = new GUIStyle();
		guiStyleBack1.fontSize = 15;
	    guiStyleBack1.normal.textColor = Color.black;  
	    guiStyleBack1.alignment = TextAnchor.UpperLeft ;
	    guiStyleBack1.wordWrap = true;
		if(WinLose.GetInstance().losing)
			WinLose.GetInstance().next = false;
	}
	
	// check selected units array and set the texture
	public void getinfo() {
		
		if (selectedUnits.Length > 0){
			foreach(GameObject units in selectedUnits){
				selectedString += units.name + " \n" ;
			}
			
			selTexture = selectedUnits[0].GetComponent<Unit>().icon;
		}
		else
			selTexture = null;

	}
	//check LMB click
	void Update(){
		selectedUnits = UnitManager.GetInstance().GetSelectedUnitsArray();
		if (Input.GetButtonUp("Fire1"))
        {
			this.selectedString = "";
            getinfo();
        }
		
	}
	
	void OnGUI () {
		
		//map box
		int offsetx = 0;
		int offsety = 5;
		Vector3 mos = Input.mousePosition;

		GUI.BeginGroup(new Rect(0,uiPosY,uiHeight,uiHeight));
		GUI.DrawTexture(new Rect(0,0,uiHeight,uiHeight), gui_minimap, ScaleMode.StretchToFill, true, 0);
		GUI.EndGroup();
		
		//char box
		GUI.BeginGroup(new Rect(uiHeight, uiPosY, uiHeight, uiHeight));
		GUI.DrawTexture(new Rect(0,0,uiHeight,uiHeight), gui_char, ScaleMode.StretchToFill, true, 0);
		
		if(selTexture != null){
			GUI.DrawTexture(new Rect(5,58,uiHeight-20,uiHeight), selTexture, ScaleMode.StretchToFill, true, 0);
		}
		
		GUI.EndGroup();
		
		//view box
		GUI.BeginGroup(new Rect(uiPosX2, uiPosY, sW-uiPosX2, uiHeight));
		GUI.DrawTexture(new Rect(0,0,sW-uiPosX2,uiHeight), gui_view, ScaleMode.StretchToFill, true, 0);
		GUI.EndGroup();
		
		//stats
		GUI.BeginGroup(new Rect(uiPosX2+10, uiPosY+60, sW-uiPosX2, uiHeight));
		
		if (selectedUnits.Length > 1){
			foreach(GameObject unit in selectedUnits){
				if(GUI.Button(new Rect(offsetx, offsety, 50, 50), unit.GetComponent<Unit>().icon)){
					Debug.Log("pressed on: " + unit.name);
					UnitManager.GetInstance().ClearSelectedUnitsList();
					UnitManager.GetInstance().AddSelectedUnit(unit);
				}
				
				float cur = unit.GetComponent<Unit>().GetHP();
				float max = unit.GetComponent<Unit>().GetMaxHP();
				float percent_hp = cur / max * 50f;
				percent_hp = Mathf.RoundToInt(percent_hp);
				
				GUI.DrawTexture(new Rect(offsetx, 55, 50, 5), health_back ,ScaleMode.StretchToFill);
				GUI.DrawTexture(new Rect(offsetx, 55, percent_hp, 5), health ,ScaleMode.StretchToFill);
				
				offsetx += 60;
			}
		}
		else if(selectedUnits.Length == 1){
			foreach(GameObject unit in selectedUnits){
				if(unit.name == "shadow_dan"){
					string uhp = unit.GetComponent<Unit>().GetHP().ToString("R");
					string umaxhp = unit.GetComponent<Unit>().GetMaxHP().ToString("R");
					
					GUI.Label(new Rect(0, 0, sW-uiPosX2, uiHeight), "Shadow Dan  " + "   HP: " + uhp + "/" + umaxhp,guiStyleBack1);
					print (UnitManager.GetInstance().GetAllUnitsCount());
					if(UnitManager.GetInstance().GetAllUnitsCount() < TOTAL_COUNT){
						if(GUI.Button(new Rect(offsetx, offsety+20, 50, 50), ninja_icon)){
							//spawn unit
							float r = 15f; //range
							float x = Random.value * r;
							float z = Random.value * r;
							
							Vector3 pos = new Vector3(x+5f,1f,z+5f) + unit.transform.position;
							Transform clone = (Transform) Object.Instantiate(newUnit.transform, pos, Quaternion.identity);
							clone.name = "ninja_range";
							UnitManager.GetInstance().AddUnit(clone.gameObject);
						 	GameObject hl = GameObject.Find("/" + clone.name+ "/ready");
							hl.audio.Play();
						}
					}
				}
				else{
					string hp = unit.GetComponent<Unit>().GetHP().ToString("R");
					string maxhp = unit.GetComponent<Unit>().GetMaxHP().ToString("R");
					string atk = unit.GetComponent<Unit>().GetAttack().ToString("R");
					string names = unit.name;
					
					if(unit.name == "ninja_hero"){
						names = "Zanosuke";
					}
					if(unit.name == "ninja_range"){
						names = "Shuriken Ninja";
					}
					GUI.Label(new Rect(0, 0, sW-uiPosX2, uiHeight), names + "\n\nHP: " + hp + "/" + maxhp + "\n\nATK: " + atk,guiStyleBack1);
				}
			}
		}

		GUI.EndGroup();
		
		GUI.Label (new Rect(Screen.width - 220,Screen.height-780,250,260),guiBanner);
	    //GUI.Label (new Rect(Screen.width - 220,Screen.height-725,250,300), "POP: "+UnitManager.GetInstance().GetAllUnitsCount()+"/"+ TOTAL_COUNT, guiStyleBack1);
	    GUI.Label (new Rect(Screen.width - 220,Screen.height-725,250,300), "POP: "+UnitManager.GetInstance().GetAllUnitsCount()+"/"+ TOTAL_COUNT, guiStyleFore1);
		
		GUI.Label(new Rect(mos.x,Screen.height - mos.y,cursorImage.width,cursorImage.height),cursorImage);
	}
	
}

using UnityEngine;
using System.Collections;

public class WinLose: MonoBehaviour {
	
	private static WinLose instance;
	public Texture victoryPic;
	public bool winning = false;
	private bool won = false;
	
	public bool losing = false;
	private bool lost = false;
	
	public bool next = true;
	private float timeRemainingThisStage = 25f;
	public Texture cursorImage;
	
	public static WinLose GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(WinLose)) as WinLose;
        }
        return instance;
    }
	
	void Awake(){
		DontDestroyOnLoad(this);	
	}
	
	void Start(){	
	}
	void OnGUI(){
		Vector3 mos = Input.mousePosition;
		
		timeRemainingThisStage -= Time.deltaTime;
		if(winning && timeRemainingThisStage <=0){
			if(won){
				GameObject wl = GameObject.Find("/WinLose");
				wl.audio.Play ();
				GameObject l = GameObject.Find("/WinLose/win");
				l.audio.Play ();
				won = false;
			}
			if(next){
				GUI.DrawTexture(new Rect(50,50,928,668), victoryPic, ScaleMode.StretchToFill, true, 0);	
				if(GUI.Button(new Rect(430, 520, 150, 50),"Continue Mission")){
					AutoFade.LoadLevel(1,2,2,Color.black);
				}
				if(GUI.Button(new Rect(430, 600, 150, 50),"Back To Main Menu")){
					AutoFade.LoadLevel(0,2,2,Color.black);
				}
				
			}
			//Time.timeScale = 0;
		}
		
		if(losing && timeRemainingThisStage <=0){
			if(lost){
				GameObject wl = GameObject.Find("/WinLose/Defeat");
				wl.audio.Play ();
				GameObject l = GameObject.Find("/WinLose/lost");
				l.audio.Play ();
				lost = false;
			}
			if(next){
				print ("here");
				GUI.DrawTexture(new Rect(50,50,928,668), victoryPic, ScaleMode.StretchToFill, true, 0);	
				if(GUI.Button(new Rect(430, 520, 150, 50),"Restart Mission")){
					AutoFade.LoadLevel(2,2,2,Color.black);
				}
				if(GUI.Button(new Rect(430, 600, 150, 50),"Back To Main Menu")){
					AutoFade.LoadLevel(0,2,2,Color.black);
				}
				//next = false;
			}
			//Time.timeScale = 0;
		}
		GUI.Label(new Rect(mos.x,Screen.height - mos.y,cursorImage.width,cursorImage.height),cursorImage);
	}
	
	public void win(){
		this.winning = true;
		this.won = true;
		GameObject.Find("/Camera").audio.Stop();
		//yield return new WaitForSeconds(2f);	
	}
	
	public void lose(){
		this.losing = true;
		this.lost = true;
		GameObject.Find("/Camera").audio.Stop();
	}
	
}

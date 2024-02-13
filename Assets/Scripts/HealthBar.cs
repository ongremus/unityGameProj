using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour
{
    // constant variable
    //public static int NUMTEXTURE = 21;
	//public Texture2D[] texture = new Texture2D[NUMTEXTURE];
	
    private float maxHealth;
    private float curHealth;
    public Texture2D maxtexture;
	public Texture2D zerotexture;
    private Rect healthBar;
	private Rect healthBar_back;
    private bool selected;
	
	private float healthScale = 40f;
	
	// health percentage
    public int GetPercent(float cur, float max)
    {
		float percent = cur / max * 40f;
		
		return Mathf.RoundToInt(percent);
		/*
        float percent = cur / max * (NUMTEXTURE - 1);
        if (percent < 1 && percent > 0)
        {
            percent = 1;
        }

        int textureNum = Mathf.RoundToInt(percent);
        return textureNum;
        */
    }
	
	// get the unit's attribute
    void Update()
    {
        Unit unit = gameObject.GetComponent<Unit>();
        maxHealth = unit.GetMaxHP();
		//curHealth = maxHealth*0.75f;
        curHealth = unit.GetHP();
        selected = unit.GetSelected();
    }
	
	// display the health bar
    void OnGUI()
    {
        //int textureNum = GetPercent(curHealth, maxHealth);
		
		this.healthScale = GetPercent(curHealth, maxHealth);
		
		/*
        if (textureNum > NUMTEXTURE)
        {
            textureNum = NUMTEXTURE;
        }

        if (textureNum < 0)
        {
            textureNum = 0;
        }
        */

        if (selected)
        {
			healthBar_back = new Rect(Camera.main.WorldToScreenPoint(transform.position).x-10, Screen.height - Camera.main.WorldToScreenPoint(transform.position).y-110 + ((Camera.main.orthographicSize/50) * 50), 40, 5);
            healthBar = new Rect(Camera.main.WorldToScreenPoint(transform.position).x-10, Screen.height - Camera.main.WorldToScreenPoint(transform.position).y-110 + ((Camera.main.orthographicSize/50) * 50), healthScale, 5);
            GUI.DrawTexture(healthBar_back, zerotexture, ScaleMode.StretchToFill);
			GUI.DrawTexture(healthBar, maxtexture, ScaleMode.StretchToFill);
        }
    }
}
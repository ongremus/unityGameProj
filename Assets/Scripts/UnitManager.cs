using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UnitManager : MonoBehaviour
{
    // the one and only instance for the unit manager
    private static UnitManager instance;
	
    private List<GameObject> allUnitsList = new List<GameObject>();
    private List<GameObject> selectedUnitsList = new List<GameObject>();

    // accessor that delivers always the one and only instance of the UnitManager
    // Use it like this: UnitManager.GetInstance().<function name>
    public static UnitManager GetInstance()
    {
        if (instance == null)
        {
            instance = FindObjectOfType(typeof(UnitManager)) as UnitManager;
        }
        return instance;
    }
	
	// return the number of selected units
    public int GetSelectedUnitsCount()
    {
        return selectedUnitsList.Count;
    }
	
	// return the number of selected units
    public int GetAllUnitsCount()
    {
        return allUnitsList.Count;
    }
	
	public GameObject[] GetSelectedUnitsArray(){
		
		return selectedUnitsList.OrderBy(GameObject=>GameObject.name).ToArray();
		//return selectedUnitsList.ToArray();
	}
	
	// add units into the units list
    public void AddUnit(GameObject go)
    {
        allUnitsList.Add(go);
    }
	
	// add selected unit into the selected list
    public void AddSelectedUnit(GameObject go)
	{		
		// only if the unit is your team
		if(go.tag == "Unit")
		{
        	selectedUnitsList.Add(go);
			go.GetComponent<Highlight>().selected();
        	go.SendMessage("SetUnitSelected", true);
			
			if(go.name == "ninja_hero")
			{
				print ("boss selected");
				go.SendMessage("specialAttack",true);
			}
		}
    }
	
	// Clear all selected units
    public void ClearSelectedUnitsList()
    {
        foreach (GameObject go in selectedUnitsList)
        {
			go.GetComponent<Highlight>().notSelected();
            go.SendMessage("SetUnitSelected", false);
			go.SendMessage("specialAttack",false);
        }
        selectedUnitsList.Clear();
    }
	
	public void StopMove(){
		foreach (GameObject go in selectedUnitsList)
		{
			go.SendMessage("EndMove", false); // call Attack to end attacking
		}
	}
	
	// Move selected units to the destination throught AstarAI
    public void MoveSelectedUnitsToPoint(Vector3 destinationPoint)
    {
		int i = 0;
		int j = 0;
		int even = 1;
		bool first = true;
		
		switch(selectedUnitsList.Count){
		case 1:
			foreach (GameObject go in selectedUnitsList)
			{
				GameObject hl = GameObject.Find("/" + go.name+ "/highlight");
				hl.audio.Play();
				go.SendMessage("End", false); // stop attacking
				go.SendMessage("Move", destinationPoint);
			}
			break;
		case 2:
	        foreach (GameObject go in selectedUnitsList)
	        {
				if(first)
				{
					GameObject hl = GameObject.Find("/" + go.name+ "/highlight");
					hl.audio.Play();
					first = false;
				}
				go.SendMessage("End", false); // stop attacking
				go.SendMessage("Move", destinationPoint + (Vector3.right * i));
				i+= 10;	
	        }
			break;
		case 3:
	        foreach (GameObject go in selectedUnitsList)
	        {
				if(first)
				{
					GameObject hl = GameObject.Find("/" + go.name+ "/highlight");
					hl.audio.Play();
					first = false;
				}
				if(even % 2 != 0)
				{
					go.SendMessage("End", false); // stop attacking
					go.SendMessage("Move", destinationPoint + (Vector3.right * i));
					i+= 10;
				}
				else{
					go.SendMessage("End", false); // stop attacking
					go.SendMessage("Move", destinationPoint + (Vector3.forward * 10) + (Vector3.right * 5));
				}
				even++;	
	        }
			break;
		case 4:
			foreach (GameObject go in selectedUnitsList)
	        {
				if(first)
				{
					GameObject hl = GameObject.Find("/" + go.name+ "/highlight");
					hl.audio.Play();
					first = false;
				}
				if(even % 2 != 0)
				{
					go.SendMessage("End", false); // stop attacking
					go.SendMessage("Move", destinationPoint + (Vector3.right * i) + (Vector3.forward * j));
					i+= 10;
				}
				else{
					go.SendMessage("End", false); // stop attacking
					go.SendMessage("Move", destinationPoint + (Vector3.forward * j) + (Vector3.right * i));
					j += 10;
					i = 0;
				}
				even++;	
	        }
			break;
		case 5:
			foreach (GameObject go in selectedUnitsList)
	        {
				if(first)
				{
					GameObject hl = GameObject.Find("/" + go.name+ "/highlight");
					hl.audio.Play();
					first = false;
				}
				if(even % 2 != 0)
				{
					go.SendMessage("End", false); // stop attacking
					go.SendMessage("Move", destinationPoint + (Vector3.right * i));
					i+= 10;
				}
				else{
					j = 10;
					i -= 5;
					go.SendMessage("End", false); // stop attacking
					go.SendMessage("Move", destinationPoint + (Vector3.forward * j) + (Vector3.right * i));
					i += 5;
				}
				even++;	
	        }
			break;
		case 6:
			foreach (GameObject go in selectedUnitsList)
	        {
				if(first)
				{
					GameObject hl = GameObject.Find("/" + go.name+ "/highlight");
					hl.audio.Play();
					first = false;
				}
				if(even % 2 != 0)
				{
					go.SendMessage("End", false); // stop attacking
					go.SendMessage("Move", destinationPoint + (Vector3.right * i));
					i+= 10;
				}
				else{
					j = 10;
					i -= 5;
					go.SendMessage("End", false); // stop attacking
					go.SendMessage("Move", destinationPoint + (Vector3.forward * j) + (Vector3.right * i));
					i += 5;
				}
				even++;	
	        }
			break;
		}
    }
	
	// Multiple units selection
    public void SelectUnitsInArea(Vector2 point1, Vector2 point2)
    {
        if (point2.x < point1.x)
        {
            // swap x positions. Selection rectangle is beeing drawn from right to left
            float x1 = point1.x;
            float x2 = point2.x;
            point1.x = x2;
            point2.x = x1;
        }

        if (point2.y > point1.y)
        {
            // swap z positions. Selection rectangle is beeing drawn from bottom to top
            float z1 = point1.y;
            float z2 = point2.y;
            point1.y = z2;
            point2.y = z1;
        }
		bool hero = false;
		int i = 0;
        foreach (GameObject go in allUnitsList)
        {
			if(go.tag == "Unit" && go.name != "shadow_dan")
			{
				Vector3 goPos = Camera.main.WorldToScreenPoint(go.transform.position);
	            if (goPos.x > point1.x && goPos.x < point2.x && goPos.y < point1.y && goPos.y > point2.y)
	            {
					
					if(go.transform.name == "ninja_hero")
					{
						go.transform.audio.Play();
						hero = true;
							
						go.SendMessage("specialAttack",true);
					}
	                selectedUnitsList.Add(go);
					go.GetComponent<Highlight>().selected();
	                go.SendMessage("SetUnitSelected", true);
					i++;
					if(i == 6)
						break;
	            
				}
			}
        }
		
		
    }
	
	// attack the target Object
	public void Attack(GameObject target)
	{
		foreach (GameObject go in selectedUnitsList)
        {
			if(target.tag == "Enemy")
			{
				go.SendMessage("EndMove", false); // call AstarAI to stop moving
				go.SendMessage("Begin", target); // call Attack to attack
			}
        }
	}
	
	// stop attacking
	public void StopAttack()
	{
		foreach (GameObject go in selectedUnitsList)
        {
			go.SendMessage("End", false); // call Attack to end attacking
        }
	}
	
	// remove unit from the list
	public void Remove(GameObject obj)
	{
		allUnitsList.Remove(obj);
		selectedUnitsList.Remove(obj);
	}

    public void OnApplicationQuit()
    {
        instance = null;
    }
}
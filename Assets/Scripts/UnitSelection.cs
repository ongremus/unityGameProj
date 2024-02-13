using UnityEngine;
using System.Collections;

public class UnitSelection : MonoBehaviour
{
    private Vector2 mouseButton1DownPoint;
    private Vector2 mouseButton1UpPoint;
    private bool mouseLeftDrag = false;
    private int terrainLayerMask = 1 << 9; // The terrain layer in unity
    private int nonTerrainLayerMask = ~(1 << 9); // The non-terrain layers in unity
    private float raycastLength = 2000.0f;
	private RaycastHit hit;
	public GameObject Target;
	// selection box texture
    public Texture selectionTexture;
	
    // range in which a mouse down and mouse up event will be treated as "the same location" on the map.
    private int mouseButtonReleaseBlurRange = 20;

    // Constant variables
    private string LMB = "Fire1"; // Left Mouse Button
    private string RMB = "Fire2"; // Right Mouse Button
    private string SHIFT = "Shift"; // Shift Button
    private string CTRL = "Ctrl"; // Ctrl Button

    // may need to change to player1, player2 and etc instead
    private string PLAYER_TAG = "Unit"; // Tag for player's units
    private string ENEMY_TAG = "Enemy"; // Tag for enemy's units
    private string NEUTRAL_TAG = "Neutral"; // Tag for neutral/ non-controllable objects
	
	// GameObject to be instantiate
	public GameObject prefab;
	
	private bool selected = false;
	
    void Start()
    {
		// Get all the units in the map
        foreach (GameObject go in GameObject.FindGameObjectsWithTag(PLAYER_TAG))
        {
			if(go.name != "shadow_dan"){
            	UnitManager.GetInstance().AddUnit(go);
			}
        }
    }
	
    void OnGUI()
    {
		// draw the selection box
        if (mouseLeftDrag)
        {
            float width = mouseButton1UpPoint.x - mouseButton1DownPoint.x;
			float height = -(mouseButton1UpPoint.y - mouseButton1DownPoint.y);
            Rect rect = new Rect(mouseButton1DownPoint.x, Screen.height - mouseButton1DownPoint.y, width, height);
            GUI.DrawTexture(rect, selectionTexture, ScaleMode.StretchToFill, true);
        }
    }

    void Update()
    {
        // Left mouse button
        if (Input.GetButtonDown(LMB))
        {
            Mouse1Down(Input.mousePosition);
        }

        if (Input.GetButtonUp(LMB))
        {
            Mouse1Up(Input.mousePosition);
        }

        if (Input.GetButton(LMB))
        {
            // Used to determine if there is some mouse drag operation going on. 
            Mouse1DownDrag(Input.mousePosition);
        }

        // Right mouse button for units moving
        if (Input.GetButtonUp(RMB))
        {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			
			// target enemy unit
			//if(Physics.Raycast(ray, out hit, raycastLength, nonTerrainLayerMask))
			if(Physics.Raycast(ray, out hit, raycastLength))
			{
				//Debug.Log("NONTERRAIN - " + hit.transform.name);
				if(hit.transform.name == "Terrian") {
					mouseIndicator();
					UnitManager.GetInstance().StopAttack(); // call UnitManager to stop the attack
					UnitManager.GetInstance().MoveSelectedUnitsToPoint(hit.point); // call UnitManager to go to the point
				}
				else
					UnitManager.GetInstance().Attack(hit.collider.gameObject);
			}
			// if not enemy unit
			/*else if(Physics.Raycast(ray, out hit, raycastLength, terrainLayerMask))
			{
				Debug.Log("TERRAIN");
				UnitManager.GetInstance().StopAttack(); // call UnitManager to stop the attack
				UnitManager.GetInstance().MoveSelectedUnitsToPoint(hit.point); // call UnitManager to go to the point
			}*/
        }
		
		// testing for instantiating prefab
		/*if(Input.GetButtonUp("Space"))
		{
			Instantiate(prefab);
		}*/
		if(Input.GetKey(KeyCode.S)){
			UnitManager.GetInstance().StopMove();
}
    }
	
	void mouseIndicator(){
			GameObject TargetObj = Instantiate(Target,hit.point,Quaternion.identity) as GameObject;
			TargetObj.name = "TargetMouse";
	}
	
	// multiple units selection
    void Mouse1DownDrag(Vector2 screenPosition)
    {
		// for selecting while camera is moving
		CameraRTS cam = gameObject.GetComponent<CameraRTS>();
		
		// World to the Screen Conversion
		Ray ray1 = Camera.main.ScreenPointToRay(new Vector3(0,0,0));
		Ray ray2 = Camera.main.ScreenPointToRay(new Vector3(Screen.width,Screen.height,0));
		RaycastHit hit1;
		RaycastHit hit2;
		Physics.Raycast(ray1, out hit1, Mathf.Infinity, terrainLayerMask);
		Physics.Raycast(ray2, out hit2, Mathf.Infinity, terrainLayerMask);
		float xUnitper3DUnit = Screen.width/Mathf.Abs(hit1.point.x-hit2.point.x);
		float yUnitper3DUnit = Screen.height/Mathf.Abs(hit1.point.z-hit2.point.z);
	
		// Determine which direction the camera is moving
		if(cam.CamMove() != 0)
		{
			switch(cam.CamMove())
			{
			case 1: mouseButton1DownPoint.x += cam.GetScrollSpeed() * Time.deltaTime * xUnitper3DUnit;
				break;
			case 2: mouseButton1DownPoint.x -= cam.GetScrollSpeed() * Time.deltaTime * xUnitper3DUnit;
				break;
			case 3: mouseButton1DownPoint.y += cam.GetScrollSpeed() * Time.deltaTime * yUnitper3DUnit * 1.75f;
				break;
			case 4: mouseButton1DownPoint.y -= cam.GetScrollSpeed() * Time.deltaTime * yUnitper3DUnit * 1.75f;
				break;
			}
		}
		
        // Only show the drag selection texture if the mouse has been moved and not if the user made only a single left mouse click
        if (screenPosition != mouseButton1DownPoint && mouseButton1DownPoint.y > (Screen.height*0.25f))
        {
            mouseLeftDrag = true;
            // while dragging, update the current mouse pos for the selection rectangle.
            mouseButton1UpPoint = screenPosition;

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out hit, raycastLength, terrainLayerMask))
            {
				selected = true;
                UnitManager.GetInstance().ClearSelectedUnitsList(); // call UnitManager to clear the selected list
				UnitManager.GetInstance().SelectUnitsInArea(mouseButton1DownPoint, mouseButton1UpPoint); // call UnitManager to determine which units are in the area
            }
        }
    }
	
	// single unit selection
    void Mouse1Down(Vector2 screenPosition)
    {
		
        mouseButton1DownPoint = screenPosition;
    }

    void Mouse1Up(Vector2 screenPosition)
    {
		
        mouseButton1UpPoint = screenPosition;

        mouseLeftDrag = false;
		
		// Add selected unit into the selected list
        if (IsInRange(mouseButton1DownPoint, mouseButton1UpPoint) && Input.mousePosition.y > (Screen.height*0.25f))
        {
            if (UnitManager.GetInstance().GetSelectedUnitsCount() == 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(mouseButton1DownPoint);
                if (Physics.Raycast(ray, out hit, raycastLength, nonTerrainLayerMask))
                {
					selected = true;
					hit.collider.gameObject.audio.Play();
                    UnitManager.GetInstance().AddSelectedUnit(hit.collider.gameObject);
                }
            }
            else
            {
				selected = false;
				UnitManager.GetInstance().ClearSelectedUnitsList();
            }
        }
    }

	// Check if the point are far away to be deem as drag
    bool IsInRange(Vector2 v1, Vector2 v2)
    {
        float dist = Vector2.Distance(v1, v2);
        if (Vector2.Distance(v1, v2) < mouseButtonReleaseBlurRange)
        {
            return true;
        }
        return false;
    }
}
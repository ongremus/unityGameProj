using UnityEngine;
using System.Collections;

public class CameraRTS : MonoBehaviour {
	
	public GameObject cameraObject;
	public int speed = 80;
	public int zoomSpeed = 1;	// Zoom speed using mouse scroll
	public int maxZoom = 50;		// Maximum zoom out distance
	public int minZoom = 20;		// Maximum zoom in distance
	public int pixelBoundary = 10;
	
	private float mouseX = 0f;
	private float mouseY = 0f;
	
	private int screenWidth = 0;
	private int screenHeight = 0;
	
	private int camMove = 0;
	public int LevelArea = 135;
	// constant variable
	private string MOUSESCROLL = "Mouse ScrollWheel";
	
	// Use this for initialization
	void Start () {
		screenWidth  = Screen.width;
		screenHeight = Screen.height;
        camera.orthographic = true;
        camera.orthographicSize = 30;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 translation = Vector3.zero;

		if((Input.GetAxis(MOUSESCROLL) > 0) && (camera.orthographicSize > minZoom))	// Zoom in
			camera.orthographicSize -= zoomSpeed;
		if((Input.GetAxis(MOUSESCROLL) < 0) && (camera.orthographicSize < maxZoom))	// Zoom out
			camera.orthographicSize += zoomSpeed;
		
		mouseX = Input.mousePosition.x;
		mouseY = Input.mousePosition.y; 
		
		camMove = 0;
		
		// Do camera movement by mouse position5.
		if (mouseX < pixelBoundary) 
		{
			translation += Vector3.right * speed * Time.deltaTime;
			camMove = 1;
		}
		if (mouseX >= Screen.width-pixelBoundary) 
		{
			translation += Vector3.right * -speed * Time.deltaTime;
			camMove = 2;
		}
		if (mouseY < pixelBoundary) 
		{
			translation += Vector3.up * -speed * Time.deltaTime;
			camMove = 3;
		}
		if (mouseY >= Screen.height-pixelBoundary) 
		{
			translation += Vector3.up * speed * Time.deltaTime;
			camMove = 4;
		} 
	
		// Keep camera within level and zoom area
        Vector3 desiredPosition = camera.transform.position + translation;
        if (desiredPosition.x < -(LevelArea+48) || (LevelArea+30) < desiredPosition.x)
        {
			camMove = 0;
            translation.x = 0;
        }
		if (desiredPosition.y < LevelArea || ((LevelArea*2)+80) < desiredPosition.y)
        {
			camMove = 0;
            translation.y = 0;
        }
        if (desiredPosition.z < -(LevelArea/2) || (LevelArea/2) < desiredPosition.z)
        {
            translation.z = 0;
        }
 		
        // Finally move camera parallel to world axis
        camera.transform.position += translation;
	}

    public Vector3 GetPosition()
    {
        return transform.position;
    }
	
	public int CamMove()
	{
		return camMove;
	}
	
	public float GetScrollSpeed()
	{
		return speed;
	}
}

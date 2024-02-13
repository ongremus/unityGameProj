using UnityEngine;
using System.Collections;

public class MousePoint : MonoBehaviour {

	RaycastHit hit;
	public GameObject Target;
	
	private float raycastlength = 500;
	
	// Update is called once per frame
	void Update () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		
		if(Physics.Raycast(ray,out hit,raycastlength)){
			if(hit.transform.name == "Terrian"){
				if(Input.GetMouseButtonDown(1)){
					print ("mouse");
					GameObject TargetObj = Instantiate(Target,hit.point,Quaternion.identity) as GameObject;
					TargetObj.name = "TargetMouse";
				}
			}
		}
		
		Debug.DrawRay(ray.origin,ray.direction*raycastlength,Color.yellow);
	}
}

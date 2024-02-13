using UnityEngine;
using System.Collections;

public class DestroyParentObject : MonoBehaviour {

	void DestroyObj(){
		Destroy(this.gameObject.transform.parent.gameObject);	
	}
}

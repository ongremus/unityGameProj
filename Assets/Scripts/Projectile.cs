using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	private bool shooting = false; 
	public GameObject shuriken;
	public GameObject arrow;
	private Vector3 aim;
	GameObject projectile;
	float time;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(shooting){
			//projectile.transform.position = Vector3.MoveTowards(new Vector3(transform.position.x, 4, transform.position.z), aim, Time.deltaTime * 400);
			time += Time.deltaTime/1;
			projectile.transform.position = Vector3.Lerp(new Vector3(transform.position.x, 4, transform.position.z), aim, time);
			if(projectile.transform.position == aim){
				Destroy(projectile);
				shooting = false;
			}
		}
	}
	
	public void Shoot()
	{
		this.aim = new Vector3(GetComponent<Unit>().GetTarget().transform.position.x, 4, GetComponent<Unit>().GetTarget().transform.position.z);
		if(this.tag == "Unit")
		{
			projectile = shuriken;
		}
		else{
			projectile = arrow;
		}
		projectile = Instantiate(projectile, new Vector3(this.transform.position.x, 4, this.transform.position.z), this.transform.rotation) as GameObject;
		shooting = true;
		time = 0;
	}
}
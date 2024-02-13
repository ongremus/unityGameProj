using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {
	
	private float hp;			// Health
    public float maxHP;         // Maximum Health
	public float moveSpd;		// Move speed in meters / sec
	private float rotSpd;		// Rotation speed in (??) / sec

	public float attack;		// Attack in damage amount
	public float attackSpd;		// Speed in attacks / sec
	public float attackRange;	// Range of attack in m
	public float sightRange;	// Range of sight
	
	private bool dead = false;
    private bool selected;		// determine if selected
	
	public GameObject target;	// its attacking target
	public Texture icon;
	
	public AnimationState[] clips;	
	
	// Use this for initialization
	void Start () {
		
		// For using animations without having to use names
		//clips = new AnimationState[this.animation.GetClipCount()];
		int a = 0;
		/*foreach(AnimationState state in animation)
			clips[a++] = state;	*/	
		
		// pre-determine first
		hp = maxHP;
		
		if(this.name == "shadow_dan"){
			hp = 5000;
		}
		else if(this.name == "ninja_range" || this.name == "samurai_range"){
			hp = 100;
		}
		else if(this.name == "samurai_boss"){
			hp = 1000;
		}
		else if(this.name == "ninja_hero"){
			hp = 800;
		}

		rotSpd = 25f;
		//attackSpd = 1f;

        selected = false;	
	}
	
	//public AnimationState[] GetAnimations() { return clips;	}	
	
	// set if selected
    void SetUnitSelected(bool selected)
    {
        this.selected = selected;
    }
	
	// being attacked
	public void KanaAttack(float damage)
	{
		this.hp -= damage;
		if(this.hp <= 0 && !dead)
		{
			if(this.name == "shadow_dan"){
				WinLose.GetInstance().next = true;
				GameObject wl = GameObject.Find("/WinLose");
				wl.GetComponent<WinLose>().lose();
				//Time.timeScale = 0;
			}else{
				target.SendMessage("SetUnitSelected", false);
				this.hp = 0;
				dead = true;
				this.SendMessage("EndMove", false);
				UnitManager.GetInstance().Remove(this.gameObject);
				animation.Play(this.name + "_die");
				//animation.Play(clips[3].name);
				GameObject hl = GameObject.Find("/" + this.name+ "/dead");
				hl.audio.Play();
				Destroy(this.gameObject, 3);
				
				if(this.name == "samurai_boss"){
					WinLose.GetInstance().next = true;
					GameObject wl = GameObject.Find("/WinLose");
					wl.GetComponent<WinLose>().win();
				}
			}
			
		}
	}
	
	public void RegenHealth()
	{
		if(hp < maxHP){
			this.hp++;
		}
	}
	
	public float GetMovementSpd()
	{
		return moveSpd;
	}
	
	// set its target
	public void SetTarget(GameObject target)
	{
		this.target = target;
	}
	
	public void ClearTarget()
	{
		this.target = null;
	}
	
	public GameObject GetTarget()
	{
		return this.target;
	}
	
	// return hp
	public float GetHP()
	{
		return this.hp;
	}
	
	// return max hp
	public float GetMaxHP()
	{
		return this.maxHP;
	}
	
	// return true if selected
	public bool GetSelected()
	{
		return this.selected;
	}
	
	public float GetAttack()
	{
		return this.attack;
	}
	
	public float GetAttackSpd()
	{
		return this.attackSpd;
	}
	
	public float GetAttackRange()
	{
		return this.attackRange;
	}
	
	public float GetSightRange()
	{
		return this.sightRange;
	}
	
	// For fog of war testing
	void Update() {
		
	}
	
	void OnColliderEnter(Collision col){
		Vector3 v = new Vector3(5f,0f,5f);
		if(col.collider.tag == "Unit")
			this.SendMessage("Move", this.transform.position + v);	
	}
}

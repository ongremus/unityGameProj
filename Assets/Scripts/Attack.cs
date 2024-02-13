using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
	
	private float damage;		// Attack in damage amount
	private float attackSpd;	// Speed in attacks / sec
	private float attackRange;	// Range of attack in m 
	private float sightRange;	// Range of Sight
	
	// Timing
	private float attackDuration;
	private float attackBuff;
	private float healthRegen;
	private float healthBuff;
	
	private bool stopAttack = true;
	private bool goAttack = false;
	private bool interuptable = true;
	private bool kanaDamage = true;
	
	private bool activated = false;
	private bool startCount = false;
	private float delay = 5;
	private float timing;

	public Transform specialPower1;
	public Transform specialPower2;
	
	Unit attacker;
	
	private GameObject empty;	// empty object
	
	// Use this for initialization
	void Start () {
		// get the attacker's attributes
		attacker = GetComponent<Unit>();
		
		damage = attacker.GetAttack();
		attackSpd = attacker.GetAttackSpd();
		attackRange = attacker.GetAttackRange();
		sightRange = attacker.GetSightRange();
		
		attackDuration = 2f; // attack every 2 sec
		attackBuff = 1f;
		
		healthRegen = 1f; // regen every 1 sec
		healthBuff = 0f;
		
		timing = delay;
	}
	
	// Begin attacking target
	void Begin(GameObject target){
		// display that the enemy is selected
		target.SendMessage("SetUnitSelected", true);
		attacker.SendMessage("SetTarget", target); // call Unit to set its target
		
		if(Vector3.Distance(attacker.transform.position, target.transform.position) > sightRange){
			attacker.SetTarget(empty);
		}
		
		// if too far away from attack, move to the target
		if(Vector3.Distance(attacker.transform.position, target.transform.position) > attackRange){
			if(goAttack && !animation.IsPlaying(name + "_attack"))
			{
				attacker.SendMessage("Move", target.transform.position);
				goAttack = false;
			}
		}
		// if within attack range, attack!
		else{
			if(!goAttack)
			{
				attacker.SendMessage("EndMove", false);
			}
			goAttack = true;
			attacker.transform.LookAt(target.transform);
			
			if(attackBuff >= attackDuration - 1f && kanaDamage){
				kanaDamage = false;
				attacker.SendMessage("StartAttack");
				GameObject hl = GameObject.Find("/" + attacker.name+ "/attack");
				hl.audio.Play();
			}
			
			// attack only if there is target
			if(target.GetComponent<Unit>().GetHP() > 0 && attackBuff >= attackDuration){
				kanaDamage = true;
				target.GetComponent<Unit>().KanaAttack(damage);
				attackBuff = 0;
				if(target.GetComponent<Unit>().GetHP() <= 0)
				{
					attacker.SetTarget(empty);
					attacker.SendMessage("EndMove", true);
				}
			}
			else
			{
				// stop attacking
				End (true);
				if(target.GetComponent<Unit>().GetHP() <= 0)
				{
					attacker.SetTarget(empty);
					attacker.SendMessage("EndMove", true);
				}
			}
			
			// timing
			if(attackBuff < attackDuration)
			{
				attackBuff += Time.deltaTime * attacker.GetAttackSpd();
			}
		}
	}
	
	// stop moving, and idle for next attack
	public void Begin(){
		stopAttack = true;	
		goAttack = true;
		interuptable = true;
	}
	
	// end attack
	public void End(bool interuptable){
		stopAttack = true;
		this.interuptable = interuptable;
	}
	public void specialAttack(bool activated)
	{
		print ("called");
		this.activated = activated;
	}
	
	private void startTimer()
	{
		startCount = true;
	}
	void Update () {
		
		if(startCount)
		{
			timing-=Time.deltaTime;
			if(timing<=0)
			{
				activated = true;
				timing = delay;
				startCount = false;
			}
		}
		
		if(attacker.GetHP() > 0){
			// health regen
			healthBuff += Time.deltaTime;
			if(healthBuff >= healthRegen){
				this.SendMessage("RegenHealth");
				healthBuff = 0f;
			}
			
			// attack!
			if(!stopAttack){
				if(attacker.GetTarget() != null){
					Begin(attacker.GetTarget());
				}
			}
			// check if any enemy is on sight
			else{
				stopAttack = true;
				
				if(Input.GetKeyUp("c")&& interuptable && activated)
				{
					this.animation.Play(name + "_skill1");
					GameObject.Find("/ninja_hero/skill1").audio.Play ();
					Instantiate(specialPower1,attacker.transform.position,Quaternion.identity);
					Collider[] nearbyUnits = Physics.OverlapSphere(transform.position, sightRange, ~(1 << 9));
					foreach(Collider hit in nearbyUnits){
						if(hit.tag == "Enemy" && hit.GetComponent<Unit>().GetHP() > 0)
						{
							hit.GetComponent<Unit>().KanaAttack(damage*10);
						}
					}
					startCount = true;
					activated = false;
				
				}
				if(Input.GetKeyUp("v")&& interuptable && activated)
				{
					animation.wrapMode = WrapMode.Once;
					this.animation.Play(name + "_skill2");
					GameObject.Find("/ninja_hero/skill2").audio.Play ();
					Instantiate(specialPower2,attacker.GetTarget().transform.position,Quaternion.identity);
				
					Collider[] nearbyUnits = Physics.OverlapSphere(attacker.GetTarget().transform.position, sightRange, ~(1 << 9));
					foreach(Collider hit in nearbyUnits){
						if(hit.tag == "Enemy" && hit.GetComponent<Unit>().GetHP() > 0)
						{
						hit.GetComponent<Unit>().KanaAttack(damage*10);
						}
					}
				
				}
				
				if(interuptable)
				{
					Collider[] nearbyUnits = Physics.OverlapSphere(transform.position, sightRange, ~(1 << 9));
					foreach(Collider hit in nearbyUnits){
						if(hit.tag == "Enemy" && hit.GetComponent<Unit>().GetHP() > 0
							&& (attacker.GetTarget() == hit.gameObject || attacker.GetTarget() == null))
						{
							if(attacker.GetTarget() == null){
								Begin(hit.gameObject);
							}
							else{
								Begin(attacker.GetTarget());
							}
						}
					}
					if(attacker.GetTarget() != null && attacker.GetTarget().GetComponent<Unit>().GetHP() <= 0){
						attacker.SetTarget(empty);
					}
				}
				else{
					if(attacker.GetTarget() != null)
					{
						attacker.GetTarget().SendMessage("SetUnitSelected", false);
						attacker.SetTarget(empty);
					}
				}
			}
		}
	}
}

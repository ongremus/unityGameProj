using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {
	
	private float damage;		// Attack in damage amount
	private float attackSpd;	// Speed in attacks / sec
	private float attackRange;	// Range of attack in m 
	private float sightRange;	// Range of Sight
	
	// Timing
	private float attackDuration;
	private float attackBuff;
	private float healthRegen;
	private float healthBuff;
	
	private bool stopAttack = false;
	private bool goAttack = false;
	private bool interuptable = true;
	private bool moveout = false;
	private bool afterloop = false;
	private bool kanaDamage = true;
	
	Unit attacker;
	
	private GameObject empty;
	
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
		
		Begin ();
	}
	
	// Begin attacking target
	void Begin(GameObject target){
		// display that the enemy is selected
		target.SendMessage("SetUnitSelected", true);
		attacker.SendMessage("SetTarget", target); // call Unit to set its target
		
		moveout = false;
		afterloop = false;
		
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
					moveout = true;
				}
			}
			else
			{
				// stop attacking
				End (true);
				if(target.GetComponent<Unit>().GetHP() <= 0)
				{
					attacker.SetTarget(empty);
					moveout = true;
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
	
	void Update () {
		
		if(attacker.GetHP() > 0){
			// health regen
			healthBuff += Time.deltaTime;
			if(healthBuff >= healthRegen){
				this.SendMessage("RegenHealth");
				healthBuff = 0f;
			}
			
			// attack!
			/*if(!stopAttack){
				if(attacker.GetTarget() != null){
					Begin(attacker.GetTarget());
				}
			}
			// check if any enemy is on sight
			else{
				stopAttack = true;*/
				
				if(interuptable)
				{
					afterloop = true;
					Collider[] nearbyUnits = Physics.OverlapSphere(transform.position, sightRange, ~(1 << 9));
					foreach(Collider hit in nearbyUnits){
						if(hit.tag == "Unit" && hit.GetComponent<Unit>().GetHP() > 0
							&& (attacker.GetTarget() == hit.gameObject || attacker.GetTarget() == null))
						{
							moveout = false;
							if(attacker.GetTarget() == null){
								Begin(hit.gameObject);
							}
							else{
								Begin(attacker.GetTarget());
							}
						}
					}
					// no more enemy around, go back to movement
					if(moveout && afterloop){
						attacker.SendMessage("EndMove", true);
						moveout = false;
						if(this.name == "samurai_boss"){
							attacker.SendMessage("BossGoBack");
						}
					}
					// target is dead, go back to movement
					else if(!moveout && afterloop)
					{
						if(attacker.GetTarget() != null && attacker.GetTarget().GetComponent<Unit>().GetHP() <= 0)
						{
							attacker.SetTarget(empty);
							attacker.SendMessage("EndMove", true);
							moveout = false;
							if(this.name == "samurai_boss"){
								attacker.SendMessage("BossGoBack");
							}
						}
					}
				}
				else{
					if(attacker.GetTarget() != null)
					{
						attacker.SendMessage("EndMove", true);
						attacker.GetTarget().SendMessage("SetUnitSelected", false);
						attacker.GetTarget().SendMessage("ClearTarget");
						if(this.name == "samurai_boss"){
							attacker.SendMessage("BossGoBack");
						}
					}
				}
			//}
		}
	}
}

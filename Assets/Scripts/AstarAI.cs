using UnityEngine;
using System.Collections;
//Note this line, if it is left out, the script won't know that the class 'Path' exists and it will throw compiler errors
//This line should always be present at the top of scripts which use pathfinding
using Pathfinding;

public class AstarAI : MonoBehaviour {
    //The point to move to
    private Vector3 targetPosition;
    private Vector3 start;			// Start point
	private Vector3 end;				// end point
	
    private Seeker seeker;
    private CharacterController controller;
	
	// for AI movement
	public GameObject[] waypoints;
	private int state;
	private int prevState;
 	
    //The calculated path
    public Path path;
    
    //The AI's speed per second
    public float speed = 1000;
    
    //The max distance from the AI to a waypoint for it to continue to the next waypoint
    public float nextWaypointDistance = 3;
 	
    //The waypoint we are currently moving towards
    private int currentWaypoint = 0;
	
	public float dist;				// Start-end
	public Vector3 dir;				// Movement direction vector
	public Quaternion lookRotation;	// Rotation dir
	public float i;		
	
	public float moveSpd;		// Move speed in meters / sec
	public float rotSpd;		// Rotation speed in (??) / sec
	
	private string name;
	
 	private bool moving;
	private bool reached = true;
	
	private AnimationState[] clips;
	
    public void Start () {
		
		//clips = GetComponent<Unit>().GetAnimations();
		
		seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
		
		speed = GetComponent<Unit>().GetMovementSpd();
		
		i = 0f;
		moveSpd = 25f;
		rotSpd = 75f;
		moving = false;
       
  		name = this.gameObject.name;
    }
	
    void Update()
	{	
		// if it is AI
		if(reached && this.gameObject.tag == "Enemy" && this.name != "samurai_boss")
		{
			reached = false;
			RandomMovement();
		}
		
		// move to the destination
		if(moving == true)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotSpd);
			seeker.StartPath (transform.position,end, OnPathComplete);

			moving = false;	
		}
		
		if (path == null) {
            //We have no path to move after yet
            return;
        }
		
		if(currentWaypoint > path.vectorPath.Count)
		{
			EndMove(true);
			return;
		}
		
		if (currentWaypoint == path.vectorPath.Count) {
			EndMove(true);
            return;
        }
        
        //Direction to the next waypoint
        Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
        dir *= speed * Time.fixedDeltaTime;
        controller.SimpleMove (dir);
        
        //Check if we are close enough to the next waypoint
        //If we are, proceed to follow the next waypoint
        if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
            if(currentWaypoint + 1 < path.vectorPath.Count){
				this.transform.LookAt(path.vectorPath[++currentWaypoint]);
			}
			else{
				currentWaypoint++;
			}
            return;
        }
	}

    public void OnPathComplete (Path p) {
        if (!p.error) {
            path = p;
            //Reset the waypoint counter
            currentWaypoint = 0;
        }
    }
	
	void Move (Vector3 dest) {
		animation.Play(name + "_run");
		
		//animation.Play (clips[0].name);
		start = transform.position;
		end = new Vector3(dest.x, transform.position.y, dest.z);
		i= 0f;
		dist = Vector3.Distance(start, end);
		dir = (end - start).normalized;
		lookRotation = Quaternion.LookRotation(dir);
		moving = true;
		reached = false;
	}
	
	void EndMove(bool reached)
	{
		this.reached = reached;
		path = null;
		if(this.tag == "Unit"){
			GetComponent<Attack>().Begin();
		}
		else if(this.tag == "Enemy"){
			GetComponent<EnemyAttack>().Begin();
		}
		animation.Play();
	}
	
	void StartAttack()
	{
		reached = false;
		animation[name + "_attack"].wrapMode = WrapMode.Once;
		animation.Play(name + "_attack");
		//animation[clips[2].name].wrapMode = WrapMode.Once;
		//animation.Play(clips[2].name);
		
		// range units shoot projectile
		if(name == "ninja_range" || name == "samurai_range"){
			this.SendMessage("Shoot");
		}
	}
	
	// for AI to move randomly
	void RandomMovement()
	{
		state = Random.Range(0, waypoints.Length);
		
		while(state == prevState)
		{
			state = Random.Range(0, waypoints.Length);
		}
		
		prevState = state;

		
		Move(waypoints[state].transform.position);
	}
	
	// samurai boss going back to his original position
	public void BossGoBack()
	{
		Move(waypoints[0].transform.position);
	}
}
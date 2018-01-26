using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaypointMove : MonoBehaviour, IGameActor, IEnemyActor {

    public EnemyStatus myStatus = EnemyStatus.Idle;

    public GameObject[] myWaypoints;
    private GameObject currentWaypoint = null;
    private GameObject nextWaypoint = null;
    private int nextWayptIndex;
    public float distToHitWaypoint = 1.0f;
    public SpriteRenderer mySpriteRenderer;

    //
    [SerializeField]
    private AnimationCurve speedGraph;
    private Keyframe[] speedReductionKeys;
    private float speedReductionFactor;
    private float distBetweenWaypoint;
    private float distToNextWaypoint;
    private float percentToNextWaypoint;

    private float areaKillbox;
    private float areaMySprite;
    public float arbitraryTestSize = 1.0f;

    public Rigidbody2D enemyRb;
    private GameObject thisGobj;

    private float enemyMaxSpeed = 7.0f;
    private Vector2 moveVelocity;

    //Time to wait in seconds before switching to next waypoint
    private float waitTime = 1.0f;
    private bool waitHere = false;
    private bool canMove = true;

	// Use this for initialization
	void Start () {
        currentWaypoint = myWaypoints[0];
        if (myWaypoints.Length > 1) {
            nextWaypoint = myWaypoints[1];
        }else {
            nextWaypoint = myWaypoints[0];
        }
        thisGobj = this.gameObject;
        enemyRb = this.GetComponent<Rigidbody2D>();
        mySpriteRenderer = this.GetComponent<SpriteRenderer>();
        moveVelocity = Vector2.zero;
        nextWayptIndex = 1;

        speedReductionKeys = new Keyframe[3];
        speedReductionKeys[0] = new Keyframe(0, 0.1f, 0.0f, 5.6f);
        speedReductionKeys[1] = new Keyframe(0.5f, 1.0f, 0.0f, 0.0f);
        speedReductionKeys[2] = new Keyframe(1, 0.1f, -5.6f, 0.0f);
        speedGraph = new AnimationCurve(speedReductionKeys);
        
    }

    void Update() {
        if(waitHere && waitTime > 0) {
            waitTime -= Time.deltaTime;
        }else if(waitHere && waitTime <= 0) {
            waitHere = false;
            waitTime = 1.0f;
        }
    }
	
	void FixedUpdate () {
        if (canMove) {
            if (!waitHere && Vector2.Distance(thisGobj.transform.position, nextWaypoint.transform.position) < distToHitWaypoint) {
                waitHere = true;

                nextWayptIndex++;
                if (nextWayptIndex >= myWaypoints.Length) {
                    nextWayptIndex = 0;
                }
                currentWaypoint = nextWaypoint;
                nextWaypoint = myWaypoints[nextWayptIndex];
            }

            if (!waitHere) {
                moveVelocity = nextWaypoint.transform.position - thisGobj.transform.position;
                moveVelocity.Normalize();
                moveVelocity *= enemyMaxSpeed;

                distBetweenWaypoint = Vector2.Distance(currentWaypoint.transform.position, nextWaypoint.transform.position);
                distToNextWaypoint = Vector2.Distance(transform.position, nextWaypoint.transform.position);
                percentToNextWaypoint = (distBetweenWaypoint - distToNextWaypoint) / distBetweenWaypoint;

                speedReductionFactor = speedGraph.Evaluate(percentToNextWaypoint);
                moveVelocity *= speedReductionFactor;

            } else {
                moveVelocity = Vector2.zero;
            }

            if(nextWaypoint == currentWaypoint) {
                moveVelocity = Vector2.zero;
            }

            enemyRb.velocity = moveVelocity;
        }
	}

    public void TakeMessage(string message) {
        if(message == "PlayerDied") {
            canMove = false;
        }
    }

    public void DestroySelf(string message) {
        //Debug.Log("DESTROYING SELF");
        this.myStatus = EnemyStatus.Dead;
        this.enabled = false;
        this.gameObject.SetActive(false);
    }

    public Bounds GetBounds() {
        return mySpriteRenderer.bounds;
    }

    public bool BoxFits(Bounds killBounds) {
        areaKillbox = killBounds.size.x * killBounds.size.y;
        //Debug.Log(areaKillbox);
        areaMySprite = mySpriteRenderer.bounds.size.x * mySpriteRenderer.bounds.size.y;

        if (areaKillbox <= areaMySprite + arbitraryTestSize) {
            //Debug.Log("FITS");
            return true;
        }
        return false;
    }

    public EnemyStatus getStatus() {
        return myStatus;
    }

    public void ActivateEnemy() {
        //mySpriteRenderer.enabled = true;
        this.enabled = true;
        this.gameObject.SetActive(true);
        myStatus = EnemyStatus.Alive;
    }
}

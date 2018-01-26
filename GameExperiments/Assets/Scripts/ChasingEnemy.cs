using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingEnemy : MonoBehaviour, IGameActor, IEnemyActor {

    public EnemyStatus myStatus = EnemyStatus.Idle;

    public SpriteRenderer mySpriteRenderer;

    private float areaKillbox;
    private float areaMySprite;
    public float arbitraryTestSize = 1.0f;

    public GameObject chaseTarget;

    public Rigidbody2D enemyRb;
    private Vector2 nextVelocity;
    //private GameObject thisGobj;

    private float enemyMaxSpeed = 3.0f;
    //private Vector2 moveVelocity;
    private Vector2 desired;

    private bool canMove = true;

    // Use this for initialization
    void Start() {
        enemyRb = this.GetComponent<Rigidbody2D>();
        mySpriteRenderer = this.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
    }

    void FixedUpdate() {
        if (canMove) {
            //Debug.Log(enemyRb.velocity);
            if (enemyRb.velocity.magnitude > enemyMaxSpeed) {
                enemyRb.velocity.Normalize();
                enemyRb.velocity *= enemyMaxSpeed;
            }
            //Debug.DrawRay(new Vector3(0,0,0), enemyRb.velocity, Color.red);
            //float angle = Mathf.Atan2(enemyRb.velocity.y, enemyRb.velocity.x) * Mathf.Rad2Deg;
            //transform.Rotate(0, 0, angle);
            float angle = Mathf.Atan2(enemyRb.velocity.y, enemyRb.velocity.x) * Mathf.Rad2Deg + -90;
            //Debug.Log("ANGLE: " + angle);
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

            desired = chaseTarget.transform.position - transform.position;
            desired.Normalize();
            desired *= enemyMaxSpeed;

            nextVelocity = desired - enemyRb.velocity;
            enemyRb.AddForce(nextVelocity, ForceMode2D.Force);
        }else {
            enemyRb.velocity = Vector2.zero;
        }
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

    public void DestroySelf(string message) {
        //Debug.Log("DESTROYING SELF");
        this.myStatus = EnemyStatus.Dead;
        this.enabled = false;
        this.gameObject.SetActive(false);
    }

    public Bounds GetBounds() {
        return mySpriteRenderer.bounds;
    }

    public EnemyStatus getStatus() {
        return myStatus;
    }

    public void TakeMessage(string message) {
        //Do nothing;
        if (message == "PlayerDied") {
            canMove = false;
        }
    }

    public void ActivateEnemy() {
        //mySpriteRenderer.enabled = true;
        this.myStatus = EnemyStatus.Alive;
        this.enabled = true;
        this.gameObject.SetActive(true);
    }
}

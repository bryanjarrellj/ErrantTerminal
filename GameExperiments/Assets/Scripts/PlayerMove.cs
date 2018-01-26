using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour, IGameActor {

    public MyGameManager myGManager;

    public Vector2 velocity;
    public Rigidbody2D rb2D;
    public float normalSpeed = 7.0f;

    public LayerMask layersIgnore;
    public BoxCollider2D characterBounds;
    public Vector3 rightChar;
    public Vector3 upChar;

    public float fudgeBoundValue = 0.1f;

    private Vector3 roomStartPosition;
    private Color oldColor;
    private Color damageColor = Color.red;
    private SpriteRenderer myCharSpriteRend;

    //private Vector3 wallNormal;
    private Vector3 wallRightVector;
    private Vector3 zCrossVector = new Vector3(0, 0, 1);
    //private Vector3 adjustedVector;

    private RaycastHit2D wallHit;

    //private Vector2 testForce = new Vector2(15, 0);
    private bool canMove = true;

    void Start() {
        rb2D = GetComponent<Rigidbody2D>();
        velocity = new Vector2(0, 0);

        roomStartPosition = transform.position;
        myCharSpriteRend = GetComponent<SpriteRenderer>();
        oldColor = myCharSpriteRend.color;

        characterBounds = GetComponent<BoxCollider2D>();
        rightChar = characterBounds.bounds.extents;
        rightChar.y = 0.0f;
        rightChar.x = rightChar.x + fudgeBoundValue;
        upChar = characterBounds.bounds.extents;
        upChar.x = 0.0f;
        upChar.y = upChar.y + fudgeBoundValue;
        Debug.Log("Extents : " + rightChar.x);
        Debug.Log("Extents y: " + upChar.y);
    }

    void Update() {
        velocity.x = 0.0f;
        velocity.y = 0.0f;

        //Debug.DrawRay(transform.position, upChar, Color.green);
        //Debug.DrawRay(transform.position, -upChar, Color.red);
        //Debug.DrawRay(transform.position, rightChar, Color.blue);
        //Debug.DrawRay(transform.position, -rightChar, Color.magenta);

        if (Input.GetKey(KeyCode.A)) {
            velocity.x = -1.0f;
        } else if (Input.GetKey(KeyCode.D)) {
            velocity.x = 1.0f;
        }

        if (Input.GetKey(KeyCode.W)) {
            velocity.y = 1.0f;
        } else if (Input.GetKey(KeyCode.S)) {
            velocity.y = -1.0f;
        }
    }

    //public static RaycastHit2D Raycast(Vector2 origin, Vector2 direction, float distance = Mathf.Infinity,
    //int layerMask = DefaultRaycastLayers, float minDepth = -Mathf.Infinity, float maxDepth = Mathf.Infinity);
    void FixedUpdate() {
        //rb2D.velocity += (velocity * Time.fixedDeltaTime);
        if (canMove) {
            //Adjust for walls and such..
            //Debug.Log(rightChar.magnitude);

            velocity.Normalize();
            velocity *= normalSpeed;

            AdjustVelocityWallHit(-rightChar);
            AdjustVelocityWallHit(rightChar);
            AdjustVelocityWallHit(upChar);
            AdjustVelocityWallHit(-upChar);

            rb2D.velocity = (velocity);

            //Debug.Log(velocity);
        }
    }

    //Adjust velocity vector when hitting wall
    //This is to not lose any velocity.
    void AdjustVelocityWallHit(Vector3 rayDirection) {
        wallHit = Physics2D.Raycast(transform.position, rayDirection, rayDirection.magnitude, layersIgnore);
        if (wallHit.collider != null) {
            //Debug.DrawLine(wallHit.point, wallHit.point + wallHit.normal, Color.black);
            wallRightVector = Vector3.Cross(wallHit.normal, zCrossVector);
            //Debug.DrawLine(wallHit.point, wallHit.point + (Vector2)wallRightVector, Color.red);
            //Debug.Log("DOT PRODUCT");
            //Debug.Log(Vector3.Dot(wallRightVector, velocity));
            if (Vector3.Dot(wallRightVector, velocity) > 0.1 && Vector3.Dot(wallHit.normal, velocity) < -0.1) {
                velocity = wallRightVector.normalized * normalSpeed;
            } else if (Vector3.Dot(wallRightVector, velocity) < -0.1 && Vector3.Dot(wallHit.normal, velocity) < -0.1) {
                velocity = -wallRightVector.normalized * normalSpeed;
            }
        }
    }

    void ResetCharacterPosition() {
        velocity = Vector2.zero;
        rb2D.velocity = Vector2.zero;
        rb2D.MovePosition(roomStartPosition);
    }

    void OnCollisionEnter2D(Collision2D collision) {
        //Debug.Log("Collided");
    }

    void OnTriggerEnter2D(Collider2D other) {
        //Debug.Log("Triggered");
        //Debug.Log(other.tag);
        if(other.CompareTag("Enemy")) {
            myGManager.NotifyManager("HitEnemy");
        }
    }

    public void TakeMessage(string message) {
        if(message == "PlayerStartDamage") {
            //Debug.Log("DO DAMAGE ANIMATION");
            myCharSpriteRend.color = damageColor;
            ResetCharacterPosition();
        }

        if(message == "PlayerEndDamage") {
            //Debug.Log("END DAMAGE ANIMATION");
            myCharSpriteRend.color = oldColor;
        }

        if (message == "PlayerDied") {
            //Debug.Log("PLAYER DIED");
            canMove = false;
        }
    }
}

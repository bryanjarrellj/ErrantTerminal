using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public BulletStatus myBulletStatus = BulletStatus.Waiting;
    //public Vector2 bulletVelocityDirection = Vector2.up;
    public float bulletSpeed = 5.0f;

    public float timeAlive = 5.0f;
    public float originalTimeAlive;

	// Use this for initialization
	void Start () {
        originalTimeAlive = timeAlive;
	}

    public void ActivateBullet() {
        this.enabled = true;
        this.gameObject.SetActive(true);
        this.myBulletStatus = BulletStatus.Active;
    }

    public void DeactivateBullet() {
        this.enabled = false;
        this.gameObject.SetActive(false);
        this.myBulletStatus = BulletStatus.Waiting;
        this.transform.position = Vector3.zero;
    }
	
	// Update is called once per frame
	void Update () {
		if(myBulletStatus == BulletStatus.Active) {
            transform.Translate(Vector3.up * bulletSpeed * Time.deltaTime, Space.Self);
            timeAlive -= Time.deltaTime;

            if (timeAlive <= 0.0f) {
                DeactivateBullet();
                timeAlive = originalTimeAlive;
            }
        }
	}
}

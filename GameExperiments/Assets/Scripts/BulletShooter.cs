using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletShooter : MonoBehaviour {

    public GameObject childRotater;
    public BulletPool gameBullets;

    public Bullet bulletToShoot;

    public GameObject[] spawnBulletPositions = new GameObject[4];

    public float shootTime = 1.0f;
    public float originalShootTime;

	// Use this for initialization
	void Start () {
        originalShootTime = shootTime;
	}
	
	// Update is called once per frame
	void Update () {
        childRotater.transform.Rotate(Vector3.forward * Time.deltaTime * 45.0f);
        if(shootTime <= 0.0f) {
            for(int i = 0; i < spawnBulletPositions.Length; i++) {
                bulletToShoot = gameBullets.GetBullet();
                bulletToShoot.transform.position = spawnBulletPositions[i].transform.position;
                bulletToShoot.transform.rotation = spawnBulletPositions[i].transform.rotation;
                bulletToShoot.ActivateBullet();
            }
            shootTime = originalShootTime;
        }else {
            shootTime -= Time.deltaTime;
        }
    }
}

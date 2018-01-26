using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour {

    List<Bullet> bulletList = new List<Bullet>(200);
    public Transform bulletParent;

    public Bullet bulletToAdd;
    public Bullet bulletReturn;
    public bool bulletFound = false;

    public Bullet exampleBullet;
    public int startBullets = 200;

	// Use this for initialization
	void Start () {
        bulletParent = transform;
        for (int i = 0; i < startBullets; i++) {
            bulletToAdd = Instantiate(exampleBullet, Vector3.zero, Quaternion.identity, bulletParent);
            bulletToAdd.DeactivateBullet();
            bulletList.Add(bulletToAdd);
        }
        exampleBullet.myBulletStatus = BulletStatus.Example;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public Bullet GetBullet() {
        bulletFound = false;
        //bulletReturn = Instantiate(exampleBullet, Vector3.zero, Quaternion.identity, bulletParent);
        //bulletReturn.myBulletStatus = BulletStatus.Active;
        for(int i = 0; i < bulletList.Count; i++) {
            if(bulletList[i].myBulletStatus == BulletStatus.Waiting) {
                bulletReturn = bulletList[i];
                bulletFound = true;
            }
        }

        if (!bulletFound) {
            bulletToAdd = Instantiate(exampleBullet, Vector3.zero, Quaternion.identity, bulletParent);
            bulletToAdd.DeactivateBullet();
            bulletList.Add(bulletToAdd);
            bulletReturn = bulletToAdd;
        }

        return bulletReturn;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour {

    //public AudioSource myAudio;

    void Start() {
        AudioSource audio = GetComponent<AudioSource>();
        audio.loop = true;
        audio.Play();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SceneManager.LoadScene("MovementTest", LoadSceneMode.Single);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Application.Quit();
        }
	}
}

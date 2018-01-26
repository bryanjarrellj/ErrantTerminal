using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogic : MonoBehaviour {

    public Image healthImg1;
    public Image healthImg2;
    public Image healthImg3;

    public Text tomeText;
    public Text subTomeText;

    public Text playerDiedText;
    public Text restartPlayerDiedText;

    public Text playerWinsText;
    public Text restartPlayerWinsText;

    public void uiLoseHealth() {
        if (healthImg3.enabled) {
            healthImg3.enabled = false;
            return;
        }

        if (healthImg2.enabled) {
            healthImg2.enabled = false;
            return;
        }

        if (healthImg1.IsActive()) {
            healthImg1.enabled = false;
            return;
        }
    }

    public void playerDied() {
        playerDiedText.enabled = true;
        restartPlayerDiedText.enabled = true;
    }

    public void playerWins() {
        playerWinsText.enabled = true;
        restartPlayerWinsText.enabled = true;
    }
}

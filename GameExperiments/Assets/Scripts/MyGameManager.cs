using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MyGameManager : MonoBehaviour {

    public List<MonoBehaviour> gameEntities = new List<MonoBehaviour>(10);

    public MonoBehaviour tmpMono;

    public PlayerCursor myPlayerCursor;

    public List<MonoBehaviour> currentEnemyListToAdd;
    public List<MonoBehaviour> Tome1Level1Enemies;
    public List<MonoBehaviour> Tome1Level2Enemies;
    public List<MonoBehaviour> Tome1Level3Enemies;
    public List<MonoBehaviour> Tome1Level4Enemies;
    public List<MonoBehaviour> Tome1Level5Enemies;
    public List<MonoBehaviour> Tome1Level6Enemies;
    public List<MonoBehaviour> Tome2Level7Enemies;

    public float addEnemiesTimer = 3.0f;
    public float originalAddEnemiesTimer;

    public int currentLevel = 1;
    public int totalLevels = 5;

    public UILogic myUiLogic;

    public bool canTakeDamage = true;
    public bool isInvincible = false;
    public float invincibleTimer = 1.0f;
    public float originalTimer;

    public bool isGameOver = false;
    public int playerLives = 3;
    public bool levelEnded = true;

    //Private variables
    private int loopVar1 = 0;
    private IGameActor tmpActor;
    private IEnemyActor tmpEnemy;

	// Use this for initialization
	void Start () {
        //gameEntities = tome1Level1Entities;;
        originalAddEnemiesTimer = addEnemiesTimer;
        originalTimer = invincibleTimer;
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            //Debug.Log("RESTARTING GAME");
            SceneManager.LoadScene("MovementTest", LoadSceneMode.Single);
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene("LoadGame", LoadSceneMode.Single);
        }

        //Update player Damage IFrame and
        //player take damage logic.
        UpdatePlayerDamage();
        if (levelEnded) {
            addEnemiesTimer -= Time.deltaTime;
        }
        if(addEnemiesTimer <= 0) {
            addEnemiesTimer = originalAddEnemiesTimer;
            ClearEnemies(gameEntities);
            AddEnemiesToEntities(currentLevel);
            levelEnded = false;
        }
    }

    public void ClearEnemies(List<MonoBehaviour> enemies) {
        for(int i = 0; i < enemies.Count; i++) {
            if(enemies[i] is IEnemyActor &&
                (enemies[i] as IEnemyActor).getStatus() == EnemyStatus.Dead) {
                //Debug.Log("REMOVING ENEMY");
                enemies.Remove(enemies[i]);
            }
        }
    }

    public void AddEnemiesToEntities(int levelNumber) {
        currentEnemyListToAdd = null;
        if(levelNumber == 1) {
            currentEnemyListToAdd = Tome1Level1Enemies;
        }else if(levelNumber == 2) {
            currentEnemyListToAdd = Tome1Level2Enemies;
        }else if(levelNumber == 3) {
            currentEnemyListToAdd = Tome1Level3Enemies;
        }else if(levelNumber == 4) {
            currentEnemyListToAdd = Tome1Level4Enemies;
        }else if(levelNumber == 5) {
            currentEnemyListToAdd = Tome1Level5Enemies;
        }else if(levelNumber == 6) {
            currentEnemyListToAdd = Tome1Level6Enemies;
        }else if(levelNumber == 7) {
            currentEnemyListToAdd = Tome2Level7Enemies;
        }
        for(int i = 0; i < currentEnemyListToAdd.Count; i++) {
            if(currentEnemyListToAdd[i] is IEnemyActor) {
                gameEntities.Add(currentEnemyListToAdd[i]);
                (currentEnemyListToAdd[i] as IEnemyActor).ActivateEnemy();
            }
        }
    }

    public void CheckEndLevel() {
        bool tmpLevelEnd = true;
        for (int i = 0; i < gameEntities.Count; i++) {
            if (gameEntities[i] is IEnemyActor) {
                EnemyStatus tmpStatus = (gameEntities[i] as IEnemyActor).getStatus();
                if(tmpStatus != EnemyStatus.Dead) {
                    tmpLevelEnd = false;
                }
            }
        }
        if (tmpLevelEnd) {
            currentLevel++;
            if(currentLevel >= 7 && totalLevels > 6) {
                myPlayerCursor.useColor = true;
            }
        }

        if(currentLevel > totalLevels) {
            //Debug.Log("SHOW WIN SCREEN");
            for (loopVar1 = 0; loopVar1 < gameEntities.Count; loopVar1++) {
                if (gameEntities[loopVar1] is IGameActor) {
                    tmpActor = gameEntities[loopVar1] as IGameActor;
                    tmpActor.TakeMessage("PlayerDied");
                }
            }
            myUiLogic.playerWins();
        } else {
            levelEnded = tmpLevelEnd;
            if (tmpLevelEnd && currentLevel == 7 && totalLevels > 6) {
                //Show The Second Tome Screen Here
            }
        }
    }

    void UpdatePlayerDamage() {
        if (!isGameOver) {
            if (isInvincible) {
                invincibleTimer -= Time.deltaTime;
            }

            if (invincibleTimer <= 0) {
                canTakeDamage = true;
                isInvincible = false;
                invincibleTimer = originalTimer;
                for (loopVar1 = 0; loopVar1 < gameEntities.Count; loopVar1++) {
                    if (gameEntities[loopVar1] is IGameActor) {
                        tmpActor = gameEntities[loopVar1] as IGameActor;
                        tmpActor.TakeMessage("PlayerEndDamage");
                    }
                }
            }
        }
    }

    //Take Damage With Invincibility
    void takeDamageIFrame() {
        if (canTakeDamage) {
            canTakeDamage = false;
            isInvincible = true;
            loseALife();
        }
    }

    void takeDamageNoIFrame() {
        //Not Implemented
    }

    void loseALife() {
        for (int i = 0; i < gameEntities.Count; i++) {
            if (gameEntities[i] is IGameActor) {
                tmpActor = gameEntities[i] as IGameActor;
                tmpActor.TakeMessage("PlayerStartDamage");
            }
        }

        if (playerLives > 0) {
            playerLives -= 1;
            myUiLogic.uiLoseHealth();
        }

        if(playerLives <= 0) {
            playerDie();
        }
    }

    void playerDie() {
        for(loopVar1 = 0; loopVar1 < gameEntities.Count; loopVar1++) {
            if(gameEntities[loopVar1] is IGameActor) {
                tmpActor = gameEntities[loopVar1] as IGameActor;
                tmpActor.TakeMessage("PlayerDied");
            }
        }
        myUiLogic.playerDied();
    }

    public void NotifyManager(string notifyID) {
        //Debug.Log("NOTIFIED");
        if(notifyID == "HitEnemy") {
            takeDamageIFrame();
        }
    }

    public void NotifyManager(string notifyID, Bounds spriteBounds, SpriteRenderer boxFirstPoint, SpriteRenderer boxSecondPoint) {
        if(notifyID == "PlayerDestroyEnemy") {
            for (loopVar1 = 0; loopVar1 < gameEntities.Count; loopVar1++) {
                if (gameEntities[loopVar1] is IEnemyActor) {
                    tmpEnemy = gameEntities[loopVar1] as IEnemyActor;
                    if (InsideBounds(spriteBounds, tmpEnemy.GetBounds()) &&
                        tmpEnemy.BoxFits(spriteBounds)) {
                        //Debug.Log("PLEASE DESTROY SELF");
                        if (gameEntities[loopVar1] is IColorActor) {
                            if((gameEntities[loopVar1] as IColorActor).CorrectColor(boxFirstPoint, boxSecondPoint)) {
                                tmpEnemy.DestroySelf("PlayerKill");
                            }
                        } else {
                            tmpEnemy.DestroySelf("PlayerKill");
                        }
                    }
                }
            }
            CheckEndLevel();
        }
    }

    public bool InsideBounds(Bounds largerBounds, Bounds smallerBounds) {
        if(largerBounds.Contains((Vector2)smallerBounds.min) && largerBounds.Contains((Vector2)smallerBounds.max)) {
            //Debug.Log("Inside");
            return true;
        }
        //Debug.Log("Not Inside");
        return false;
    }


}

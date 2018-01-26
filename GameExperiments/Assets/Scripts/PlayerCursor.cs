using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCursor : MonoBehaviour {

    public MyGameManager myGManager;

    public Camera mainGameCam;

    public Texture2D mouseTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot;

    public SpriteRenderer boxFirstPoint;
    public SpriteRenderer boxSecondPoint;
    public SpriteRenderer killBox;
    public SpriteRenderer ghostPoint;
    public SpriteRenderer reloadBar;
    public Bounds destroyBounds;

    public float keyCounter = 1.0f;
    public float originalKeyCounter;

    private bool firstPointSet = false;
    private bool allowPointPlacing = true;
    public bool useColor = false;

    private Vector3 desiredLocalScale;
    private float lerpXValue = 0.0f;
    public float lerpXDuration = 0.3f;
    private float lerpYValue = 0.0f;
    public float lerpYDuration = 0.2f;
    public float flashTimer = 0.4f;
    public float totalFlashTimer;
    private bool finishedBoxAnimation = false;

    private float originalBoxXScale;
    private float originalBoxYScale;

    private Vector3 originalReloadScale;

    //Killbox color variables
    public Color originalColor;
    public Color flashColor = Color.white;


    //public Vector2 cursorVelocity;
    //public float cursorSpeed = 5.0f;
    //private Collider2D cursorCollider;

	// Use this for initialization
	void Start () {
        //
        hotSpot = new Vector2(mouseTexture.width / 2.0f, mouseTexture.height / 2.0f);
        Cursor.SetCursor(mouseTexture, hotSpot, cursorMode);
        originalKeyCounter = keyCounter;
        originalColor = killBox.color;
        flashColor.a = 0.5f;
        totalFlashTimer = flashTimer;
        originalReloadScale = reloadBar.transform.localScale;
    }
	
	// Update is called once per frame
	void Update () {
        //cursorVelocity.x = 0.0f;
        //cursorVelocity.y = 0.0f;

        //if (Input.GetKey(KeyCode.LeftArrow)) {
        //    cursorVelocity.x = -1.0f;
        //} else if (Input.GetKey(KeyCode.RightArrow)) {
        //    cursorVelocity.x = 1.0f;
        //}

        //if (Input.GetKey(KeyCode.UpArrow)) {
        //    cursorVelocity.y = 1.0f;
        //} else if (Input.GetKey(KeyCode.DownArrow)) {
        //    cursorVelocity.y = -1.0f;
        //}

        //cursorVelocity.Normalize();
        //var nextCursorPos = transform.position + (Vector3)(cursorVelocity * cursorSpeed * Time.deltaTime);
        var nextCursorPos = Input.mousePosition;
        nextCursorPos = mainGameCam.ScreenToWorldPoint(nextCursorPos);
        nextCursorPos.z = 0;
        //Debug.Log(nextCursorPos);
        if (IsPointInScreenView(nextCursorPos)) {
            //transform.Translate(cursorVelocity * cursorSpeed * Time.deltaTime);
            transform.position = nextCursorPos;
        }
        //Debug.Log("Screen Space Position:");
        //Debug.Log(mainGameCam.WorldToScreenPoint(transform.position));
        //Debug.Log("IS POINT IN SCREEN VIEW??");
        //Debug.Log(IsPointInScreenView(transform.position));

        if (Input.GetMouseButton(0)) {
            ghostPoint.transform.position = transform.position;
            ghostPoint.enabled = true;
        }else {
            ghostPoint.enabled = false;
        }

        if (Input.GetMouseButtonUp(0)) {
            if (!firstPointSet && allowPointPlacing) {
                boxFirstPoint.transform.position = transform.position;
                boxFirstPoint.enabled = true;
                firstPointSet = true;
            } else if (firstPointSet && allowPointPlacing) {
                boxSecondPoint.transform.position = transform.position;
                boxSecondPoint.enabled = true;
                allowPointPlacing = false;
                killBox.transform.position = GetDestroyBoxPosition();
                //killBox.transform.localScale = GetDestroyBoxScale();
                desiredLocalScale = GetDestroyBoxScale();
                killBox.transform.localScale = new Vector3(0, desiredLocalScale.y * 0.1f, 0);
                originalBoxXScale = killBox.transform.localScale.x;
                originalBoxYScale = killBox.transform.localScale.y;
                killBox.enabled = true;
                reloadBar.transform.localScale = new Vector3(0, reloadBar.transform.localScale.y, reloadBar.transform.localScale.z);

                if (useColor) {
                    if (boxFirstPoint.transform.position.x > boxSecondPoint.transform.position.x &&
                        boxFirstPoint.transform.position.y > boxSecondPoint.transform.position.y) {
                        killBox.color = new Color(0, 0, 1, 0.5f);
                        originalColor = killBox.color;
                    }else if (boxFirstPoint.transform.position.x > boxSecondPoint.transform.position.x &&
                        boxFirstPoint.transform.position.y <= boxSecondPoint.transform.position.y) {
                        killBox.color = new Color(0, 1, 0, 0.5f);
                        originalColor = killBox.color;
                    } else if (boxFirstPoint.transform.position.x <= boxSecondPoint.transform.position.x &&
                        boxFirstPoint.transform.position.y > boxSecondPoint.transform.position.y) {
                        killBox.color = new Color(1, 1, 0.5f, 0.5f);
                        originalColor = killBox.color;
                    } else if (boxFirstPoint.transform.position.x <= boxSecondPoint.transform.position.x &&
                        boxFirstPoint.transform.position.y <= boxSecondPoint.transform.position.y) {
                        killBox.color = new Color(1, 0, 0, 0.5f);
                        originalColor = killBox.color;
                    }
                }
            }
        }

        if (!allowPointPlacing) {
            keyCounter -= Time.deltaTime;
            var percentageDone = (originalKeyCounter - keyCounter) / originalKeyCounter;
            reloadBar.transform.localScale = new Vector3(Mathf.Lerp(0, originalReloadScale.x, percentageDone),
                                                         reloadBar.transform.localScale.y, 
                                                         reloadBar.transform.localScale.z);
            killBox.transform.localScale = new Vector3(Mathf.Lerp(originalBoxXScale, desiredLocalScale.x, lerpXValue),
                                                       Mathf.Lerp(originalBoxYScale, desiredLocalScale.y, lerpYValue),
                                                       0);

            if (lerpXValue < 1.0f) {
                lerpXValue += Time.deltaTime / lerpXDuration;
            } else if (lerpXValue >= 1.0f && lerpYValue < 1.0f) {
                lerpYValue += Time.deltaTime / lerpYDuration;
            } else if (lerpYValue >= 1.0f && flashTimer > 0.0f) {
                if (flashTimer/totalFlashTimer >= 0.75f || (flashTimer/totalFlashTimer >= 0.25 && flashTimer/totalFlashTimer < 0.5)) {
                    killBox.color = flashColor;
                }else {
                    killBox.color = originalColor;
                }
                flashTimer -= Time.deltaTime;
                //Debug.Log("FLASH TIMER: " + flashTimer + " " + finishedBoxAnimation);
            } else if( flashTimer <= 0.0f && !finishedBoxAnimation) {
                killBox.color = originalColor;
                killBox.enabled = false;
                finishedBoxAnimation = true;
                Debug.Log("Destroy");
                destroyBounds = killBox.bounds;
                myGManager.NotifyManager("PlayerDestroyEnemy", destroyBounds, boxFirstPoint, boxSecondPoint);
            }

        }

        if(keyCounter <= 0.0f) {
            //For box size
            lerpXValue = 0;
            lerpYValue = 0;
            killBox.color = originalColor;
            flashTimer = totalFlashTimer;
            finishedBoxAnimation = false;
            killBox.enabled = false;

            //For reload bar size
            reloadBar.transform.localScale = originalReloadScale;

            //For point placement
            allowPointPlacing = true;
            firstPointSet = false;
            boxFirstPoint.enabled = false;
            boxSecondPoint.enabled = false;
            keyCounter = originalKeyCounter;
        }
    }

    Vector2 GetDestroyBoxPosition() {
        float p1 = (boxFirstPoint.transform.position.x + boxSecondPoint.transform.position.x) / 2.0f;
        float p2 = (boxFirstPoint.transform.position.y + boxSecondPoint.transform.position.y) / 2.0f;
        return new Vector2(p1, p2);
    }

    Vector3 GetDestroyBoxScale() {
        float scaleX = Mathf.Abs(boxFirstPoint.transform.position.x - boxSecondPoint.transform.position.x);
        float scaleY = Mathf.Abs(boxFirstPoint.transform.position.y - boxSecondPoint.transform.position.y);
        return new Vector3(scaleX, scaleY, 1);
    }

    bool IsPointInScreenView(Vector2 position) {
        position = mainGameCam.WorldToScreenPoint(position);
        return position.x > 0 && position.y > 0 && position.x < Screen.width && position.y < Screen.height; 
    }
}

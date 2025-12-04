using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum BallIdentity
{
    Bouncyball,
    Lootball
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    Camera cam;

    public BallIdentity ballIdentity = BallIdentity.Bouncyball;

    public Ball ball;
    public Trajectory trajectory;
    [SerializeField] float pushForce = 4f;
    [SerializeField] float lootBallPushForce = 10f;

    [SerializeField] float maxDragDistance = 2.5f; // Set your preferred limit

    private Coroutine resetBallCoroutine;
    public float resetPositionDelay = 5f;
    [SerializeField] private float lootResetPositionDelay = 2.5f;

    public GameObject successPanel;
    public GameObject[] successStars;
    public GameObject failurePanel;

    bool isDragging = false;

    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 direction;
    Vector2 force;
    float distance;

    // For tracking FallenBoxes UI
    [SerializeField] private int targetBoxes = 5;
    [SerializeField] private float updateDelay = 2f;
    private int fallenBoxes = 0;
    [SerializeField] private TextMeshProUGUI fallenBoxesText;
    [SerializeField] private TextMeshProUGUI BallsCount;
    private HashSet<GameObject> countedBoxes = new HashSet<GameObject>();

    // for showing available balls ui
    public int currentBalls = 14;
    public GameObject[] AvailableBalls;
    private int maxBalls;

    private bool successPanelOpened;

    // Loot ui
    [SerializeField] private TextMeshProUGUI LootBallsCountL;
    [SerializeField] private TextMeshProUGUI LootBallsCountR;
    public GameObject SlingshotBase;
    public int currentLootBalls;
    private int maxLootballs = 3;

    // Coin
    public int coins = 10;
    [SerializeField] private TextMeshProUGUI coins_text;
    public int bombBallCost = 50;
    public int freezingBallCost = 70;
    public int explosionBallCost = 60;
    public int BoxDestoyerBallCost = 70;

    private Vector3 startPosition;
    private float localPushForce;

    private bool gameEnded;
    private bool canEnablePower = true;
    private bool isCancelledDrag = false;
    private bool wasLastLootball = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        startPosition = transform.position;
        localPushForce = pushForce;

        LootBallsCountL.gameObject.SetActive(false);
        LootBallsCountR.gameObject.SetActive(false);

        maxBalls = currentBalls;
        currentLootBalls = maxLootballs;
        fallenBoxesText.text = "Boxes Fallen: " + fallenBoxes + "/" + targetBoxes;

        cam = Camera.main;
        ball.DeactivateRb();

        UpdateBallCount();
        UpdateCoinUi();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
             CancelDrag();
        }

        if (isDragging)
        {
            OnDrag();
        }
    }

    private void CancelDrag()
    {
        if (!isDragging) return;

        isCancelledDrag = true;

        isDragging = false;
        trajectory.Hide();
        ResetBall();
        Slingshot.Instance.OnMouseUpEvent();
        SoundManager.instance.PlayButtonClick();
    }

    private void OnMouseDown()
    {
        isDragging = true;
        OnDragStart();
    }

    private void OnMouseUp()
    {
        isDragging = false;
        OnDragEnd();
    }

    void OnDragStart()
    {
        if (gameEnded) { return; }

        ball.DeactivateRb();
        ball.transform.position = Slingshot.Instance.idlePosition.position;
        ball.transform.rotation = Quaternion.identity;

        startPoint = cam.ScreenToWorldPoint(Input.mousePosition);

        trajectory.Show();
        Slingshot.Instance.OnMouseDownEvent();

        SoundManager.instance.PlayCameraShakeSound();
    }

    void OnDrag()
    {
        if (gameEnded) { return; }

        endPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);

        // Limit the drag distance
        distance = Mathf.Min(distance, maxDragDistance);

        direction = (startPoint - endPoint).normalized;
        force = direction * distance * pushForce;

        if (ballIdentity == BallIdentity.Bouncyball)
        {
            trajectory.UpdateDots(ball.pos, force, false);
        }
        else if (ballIdentity == BallIdentity.Lootball)
        {
            trajectory.UpdateDots(ball.pos, force, true);
        }
    }

    void OnDragEnd()
    {
        if (gameEnded) { return; }

        if (isCancelledDrag == true)
        {
            isCancelledDrag = false;
            return;
        }

        canEnablePower = false;

        if (ballIdentity == BallIdentity.Bouncyball)
        {
            currentBalls--;
            UpdateBallCount();

            if (currentBalls == 0)
            {
                gameEnded = true;
            }
        }
        else if(ballIdentity == BallIdentity.Lootball)
        {
            currentLootBalls--;
            UpdateLootBallCount();

            if (currentLootBalls == 0)
            {
                wasLastLootball = true;

                ballIdentity = BallIdentity.Bouncyball;
                currentLootBalls = maxLootballs;
                UpdateLootBallCount();
                LootBallsCountL.text = "x" + currentLootBalls;
                LootBallsCountR.text = currentLootBalls + "x";

                LootBallsCountL.gameObject.SetActive(false);
                LootBallsCountR.gameObject.SetActive(false);
                SlingshotBase.gameObject.SetActive(true);

                StartCoroutine(ResetBallOriginalScale());

                transform.position = startPosition;
                Slingshot.Instance.UpdateStripPosition();

                gameEnded = true;
            }
        }

        ball.ActivateRb();
        ball.Push(force);
        Slingshot.Instance.OnMouseUpEvent();
        ball.GetComponent<Ball>().Release();

        // Restart the auto-reset coroutine
        if (resetBallCoroutine != null)
        {
            StopCoroutine(resetBallCoroutine);
        }

        if (successPanelOpened == false)
        {
            resetBallCoroutine = StartCoroutine(AutoSetDragPosition());
        }
    }

    IEnumerator AutoSetDragPosition()
    {
        if (ballIdentity == BallIdentity.Bouncyball)
        {
            yield return new WaitForSeconds(resetPositionDelay);
        }
        else if (ballIdentity == BallIdentity.Lootball)
        {
            yield return new WaitForSeconds(lootResetPositionDelay);
        }

        if (ballIdentity == BallIdentity.Bouncyball)
        {
            if (currentBalls == 0)
            {
                OpenFailurePanel();
            }
        }

        ResetBall();
    }

    IEnumerator ResetBallOriginalScale()
    {
        yield return new WaitForSeconds(resetPositionDelay);

        if (wasLastLootball)
        {
            wasLastLootball = false;
            ResetScale();
        }
    }

    public void ResetScale()
    {
        ball.transform.localScale *= 2.0f;
        pushForce = localPushForce;
        gameEnded = false;
    }

    public void ResetBall()
    {
        if (wasLastLootball)
        {
            wasLastLootball = false;
            ResetScale();
        }

        canEnablePower = true;

        // Move ball to drag start position and reset velocity
        ball.DeactivateRb();
        ball.transform.position = Slingshot.Instance.idlePosition.position;
        ball.transform.rotation = Quaternion.identity;
        ball.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

        ball.collisionCount = 5;
    }

    public void UpdateFallenBoxes(GameObject box)
    {
        if (!countedBoxes.Contains(box))
        {
            countedBoxes.Add(box);
            fallenBoxes++;
            fallenBoxesText.text = "Boxes Fallen: " + fallenBoxes + "/" + targetBoxes;

            StartCoroutine(DestroyBoxAfterDelay(box));
        }
    }

    public void UpdateFallenBoxesInstant(GameObject box)
    {
        if (!countedBoxes.Contains(box))
        {
            countedBoxes.Add(box);
            fallenBoxes++;
            fallenBoxesText.text = "Boxes Fallen: " + fallenBoxes + "/" + targetBoxes;

            Destroy(box);

            if (fallenBoxes >= targetBoxes)
            {
                OpenSuccessPanel();
            }
        }
    }

    private IEnumerator DestroyBoxAfterDelay(GameObject box)
    {
        yield return new WaitForSeconds(updateDelay);
        Destroy(box);

        if (fallenBoxes >= targetBoxes)
        {
            OpenSuccessPanel();
        }
    }

    public void OpenSuccessPanel()
    {
        Timer.Instance.StopTimer();
        successPanel.SetActive(true);

        successPanelOpened = true;

        CalculateStars();

        DestroyBalloons();

        gameEnded = true;
    }

    public void DestroyBalloons()
    {
        GameObject balloonParent = GameObject.Find("Balloon");

        if (balloonParent != null)
        {
            foreach (Transform child in balloonParent.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }

    public void CalculateStars()
    {
        // Calculate starCount based on the percentage of balls used
        float percentageUsed = 1.0f - ((float)currentBalls / maxBalls);
        int starCount = Mathf.CeilToInt(percentageUsed * 5);

        // Invert the star count logic
        starCount = 6 - starCount;

        // Ensure starCount is between 1 and 3
        starCount = Mathf.Clamp(starCount, 1, 5);

        for (int i = 0; i < successStars.Length; i++)
        {
            if (i < starCount)
            {
                successStars[i].gameObject.SetActive(true);
            }
            else
            {
                successStars[i].gameObject.SetActive(false);
            }
        }
    }

    public void OpenFailurePanel()
    {
        if (successPanelOpened) return;
        Timer.Instance.StopTimer();
        failurePanel.SetActive(true);

        gameEnded = true;
    }

    public void LeavePressed_ToggleDrag(bool yes)
    {
        gameEnded = yes;
    }

    public void UpdateBallCount()
    {
        for (int i = 0; i < AvailableBalls.Length; i++)
        {
            if (i < currentBalls)
            {
                AvailableBalls[i].gameObject.SetActive(true);
            }
            else
            {
                AvailableBalls[i].gameObject.SetActive(false);
            }
        }

        BallsCount.text = "x" + currentBalls;
    }

    public void IncrementBallCount()
    {
        if (currentBalls < 13)
        {
            currentBalls = currentBalls + 2;
            UpdateBallCount();
        }
    }

    public void IncrementCoinCount(int amount)
    {
        coins = coins + amount;
        UpdateCoinUi();
    }

    private void UpdateCoinUi()
    {
        coins_text.text = "Coins: " + coins;
    }

    public void ActivateLootballs(bool Left)
    {
        if (Left)
        {
            LootBallsCountL.gameObject.SetActive(true);
            LootBallsCountL.text = "x" + currentLootBalls;
        }
        else
        {
            LootBallsCountR.gameObject.SetActive(true);
            LootBallsCountR.text = currentLootBalls + "x";
        }

        // scale the ball size to half the current size
        ball.transform.localScale *= 0.5f;
        pushForce = lootBallPushForce;

        ballIdentity = BallIdentity.Lootball;
        SlingshotBase.gameObject.SetActive(false);

        ResetBall();
    }

    public void UpdateLootBallCount()
    {
        LootBallsCountL.text = "x" + currentLootBalls;
        LootBallsCountR.text = currentLootBalls + "x";
    }

    public void SetBallTypeFromUI(int type)
    {
        CancelDrag();

        if (canEnablePower == false) return;

        if (type == 1)
        {
            if (coins > bombBallCost)
            {
                ball.SetBallType((BallType)type);
                coins = coins - bombBallCost;
                UpdateCoinUi();
            }
        }

        if (type == 2)
        {
            if (coins > freezingBallCost)
            {
                ball.SetBallType((BallType)type);
                coins = coins - freezingBallCost;
                UpdateCoinUi();
            }
        }

        if (type == 3)
        {
            if (coins > explosionBallCost)
            {
                ball.SetBallType((BallType)type);
                coins = coins - explosionBallCost;
                UpdateCoinUi();
            }
        }

        if (type == 4)
        {
            if (coins > BoxDestoyerBallCost)
            {
                ball.SetBallType((BallType)type);
                coins = coins - BoxDestoyerBallCost;
                UpdateCoinUi();
            }
        }
    }
}
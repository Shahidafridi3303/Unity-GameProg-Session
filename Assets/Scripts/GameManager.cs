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
    //public Trajectory trajectory;
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

        UpdateBallCount();
        UpdateCoinUi();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
             //CancelDrag();
        }

        if (isDragging)
        {
            //OnDrag();
        }
    }

    private void OnMouseDown()
    {
        isDragging = true;
        OnDragStart();
    }

    private void OnMouseUp()
    {
        isDragging = false;
        //OnDragEnd();
    }

    void OnDragStart()
    {
        //if (gameEnded) { return; }

        //ball.DeactivateRb();
        //ball.transform.position = Slingshot.Instance.idlePosition.position;
        //ball.transform.rotation = Quaternion.identity;

        //startPoint = cam.ScreenToWorldPoint(Input.mousePosition);

        //trajectory.Show();
        //Slingshot.Instance.OnMouseDownEvent();

        //SoundManager.instance.PlayCameraShakeSound();
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
    }

    public void ResetBall()
    {
        if (wasLastLootball)
        {
            wasLastLootball = false;
            ResetScale();
        }

        // Move ball to drag start position and reset velocity
        //ball.transform.position = Slingshot.Instance.idlePosition.position;
        ball.transform.rotation = Quaternion.identity;
        ball.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
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

    }

    public void LeavePressed_ToggleDrag(bool yes)
    {
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
}
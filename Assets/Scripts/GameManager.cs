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

    public GameObject successPanel;
    public GameObject failurePanel;

    // For tracking FallenBoxes UI
    [SerializeField] private int targetBoxes = 5;
    [SerializeField] private float updateDelay = 2f;
    private int fallenBoxes = 0;
    [SerializeField] private TextMeshProUGUI fallenBoxesText;
    private HashSet<GameObject> countedBoxes = new HashSet<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        fallenBoxesText.text = "Boxes Fallen: " + fallenBoxes + "/" + targetBoxes;

        cam = Camera.main;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
             //CancelDrag();
        }
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
        Time.timeScale = 0.0f;
    }

    public void OpenFailurePanel()
    {
        Timer.Instance.StopTimer();
        failurePanel.SetActive(true);
        Time.timeScale = 0.0f;
    }

    public void OpenFailureWithDelay()
    {
        Invoke("OpenFailurePanel", 0.8f);
    }
}
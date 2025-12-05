using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    public int minAmount = 7, maxAmount = 19;
    private int amount;

    private Rigidbody2D rb;
    private Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Start()
    {
        // DeactivateRb();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Ball.Instance.BallCameraShake_Small(true, transform.position);
            UpdateGameManager(); 
        }
    }

    public void Activate()
    {
        Ball.Instance.BallCameraShake_Small(false, transform.position);
        UpdateGameManager();
    }

    public void UpdateGameManager()
    {
        amount = Random.Range(minAmount, maxAmount);
        Destroy(gameObject);
    }
}
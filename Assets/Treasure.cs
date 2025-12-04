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
        DeactivateRb();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            Ball.Instance.BallCameraShake_Small(true, transform.position);
            UpdateGameManager(); 
            SoundManager.instance.PlayTreasureActivateSound();
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
        GameManager.Instance.IncrementCoinCount(amount);
        Destroy(gameObject);
    }

    public void ActivateRb()
    {
        rb.isKinematic = false;
        col.isTrigger = false;
        Invoke("DeactivateRb", 1.0f);
    }

    public void DeactivateRb()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
        col.isTrigger = true;
        transform.rotation = Quaternion.identity;
    }
}
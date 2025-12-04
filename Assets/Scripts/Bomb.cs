using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BombState
{
    Normal,
    Hanging
}

public class Bomb : MonoBehaviour
{
    public float fieldOfImpact = 3.22f;
    public float treasureFieldOfImpact = 1.88f;
    public float force;
    public LayerMask LayerToHit;

    public GameObject explosionEffect;
    public GameObject bomb;
    public float effectDestroyDelay = 5;

    private bool alreadyActivated;
    public BombState bombState;
    public GameObject chains;

    private Rigidbody2D rb;
    private bool canExplode;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start()
    {
        if (bombState == BombState.Hanging)
        {
            rb.isKinematic = true;
        }
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    explode();
        //}
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Box") || other.gameObject.CompareTag("Wall") || other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Treasure"))
        {
            if (bombState == BombState.Hanging)
            {
                if (canExplode)
                {
                    explode();
                }
            }
            else
            {
                explode();
            }
        }
    }
    
    void explode()
    {
        if (alreadyActivated) return;

        alreadyActivated = true;

        // Apply force to objects in fieldOfImpact
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, fieldOfImpact, LayerToHit);
        foreach (Collider2D obj in objects)
        {
            Vector2 direction = obj.transform.position - transform.position;
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(direction * force);
            }
        }

        // Detect and activate nearby treasures
        Collider2D[] treasures = Physics2D.OverlapCircleAll(transform.position, treasureFieldOfImpact);
        foreach (Collider2D treasure in treasures)
        {
            if (treasure.CompareTag("Treasure"))
            {
                Treasure treasureScript = treasure.GetComponent<Treasure>();
                if (treasureScript != null)
                {
                    treasureScript.Activate();
                }
            }
        }

        bomb.gameObject.SetActive(false);
        CameraShake.Instance.Shake();
        SoundManager.instance.PlayBombExplodeSound();
        GameObject explosionEffectIns = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        explosionEffectIns.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        Destroy(gameObject, effectDestroyDelay);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fieldOfImpact);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, treasureFieldOfImpact);
    }

    public void Trigger()
    {
        canExplode = true;
        Destroy(chains);
        rb.isKinematic = false;
    }
}

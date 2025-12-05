using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private float fieldOfImpact = 3.22f;
    public float force;

    public GameObject explosionEffect;
    public GameObject bomb;
    private float effectDestroyDelay = 5;

    private bool alreadyActivated;

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    explode();
        //}
    }

    public void explode()
    {
        //if (alreadyActivated) return;

        //alreadyActivated = true;

        //// Apply force to objects in fieldOfImpact
        //Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, fieldOfImpact, LayerToHit);
        //foreach (Collider2D obj in objects)
        //{
        //    Vector2 direction = obj.transform.position - transform.position;
        //    Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        //    if (rb != null)
        //    {
        //        rb.AddForce(direction * force);
        //    }
        //}

        if (alreadyActivated) return;
        alreadyActivated = true;

        // Get EVERYTHING inside radius (no layer mask needed)
        Collider2D[] objects = Physics2D.OverlapCircleAll(transform.position, fieldOfImpact);

        foreach (Collider2D obj in objects)
        {
            // Check if the object has the Box script
            Box box = obj.GetComponent<Box>();
            if (box == null)
                continue;  // Skip objects without Box script

            // Apply force only to objects with Box script
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 direction = obj.transform.position - transform.position;
                rb.AddForce(direction.normalized * force);
            }
        }

        bomb.gameObject.SetActive(false);
        CameraShake.Instance.Shake();
        GameObject explosionEffectIns = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        explosionEffectIns.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        Destroy(gameObject, effectDestroyDelay);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fieldOfImpact);
    }
}

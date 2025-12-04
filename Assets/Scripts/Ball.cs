using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameObject smallExplosionEffect;

    private Rigidbody2D rb;
    private CircleCollider2D col;

    [HideInInspector] public Vector3 pos { get { return transform.position; } }

    // ExplodeType property
    public float BombfieldOfImpact;
    public float FrozefieldOfImpact;
    public float ExplosionfieldOfImpact;

    public float force;

    public static Ball Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

    public void Push(Vector2 force)
    {
        rb.linearVelocity = force;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //if (other.gameObject.CompareTag("Chains"))
        //{
        //    //Bomb bombScript = GetComponent<Bomb>();
        //    Bomb bombScript = other.gameObject.GetComponentInParent<Bomb>();

        //    if (bombScript != null)
        //    {
        //        bombScript.Trigger();
        //    }
        //}
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, BombfieldOfImpact);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, FrozefieldOfImpact);

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, ExplosionfieldOfImpact);
    }

    public void BallCameraShake()
    {
        CameraShake.Instance.Shake();
        GameObject explosionEffectIns = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        explosionEffectIns.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }

    public void BallCameraShake_Small(bool Shake, Vector3 location)
    {
        if (!Shake)
        {
            CameraShake.Instance.Shake(true);

            GameObject smallExplosionEffectIns = Instantiate(smallExplosionEffect, location, Quaternion.identity);
            smallExplosionEffectIns.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        }
        else
        {
            GameObject explosionEffectIns = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            explosionEffectIns.transform.localScale = new Vector3(0.15f, 0.15f, 0.15f);
        }
    }
}

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

    public static Ball Instance;

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

        PlatformColorChanger Platf_ChngColr = other.gameObject.GetComponentInParent<PlatformColorChanger>();
        Bomb bomb = other.gameObject.GetComponentInParent<Bomb>();
        Spike spike = other.gameObject.GetComponent<Spike>();

        if (Platf_ChngColr)
        {
            Platf_ChngColr.ApplyColor();
        }
        if (bomb)
        {
            bomb.explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Spike spike = other.gameObject.GetComponent<Spike>();

        if (spike)
        {
            BallCameraShake();
            GameManager.Instance.OpenFailureWithDelay();
        }
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

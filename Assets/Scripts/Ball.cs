using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallType
{
    Normal,
    Bomb,
    Freeze,
    Explosion,
    BoxDestoyer
}

public class Ball : MonoBehaviour
{
    public GameObject explosionEffect;
    public GameObject smallExplosionEffect;

    private Rigidbody2D rb;
    private CircleCollider2D col;
    public BallType ballType = BallType.Normal;

    [HideInInspector] public int collisionCount = 0;
    [HideInInspector] public Vector3 pos { get { return transform.position; } }

    // ExplodeType property
    public float BombfieldOfImpact;
    public float FrozefieldOfImpact;
    public float ExplosionfieldOfImpact;

    public float force;

    public static Ball Instance;

    public WindBlower windBlower;
    public WindBlower windBlower2;

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
        rb.velocity = force;
    }

    public void ActivateRb()
    {
        rb.isKinematic = false;
    }

    public void DeactivateRb()
    {
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
        transform.rotation = Quaternion.identity;
    }

    public void Release()
    {
        collisionCount = 0; // Reset collision count on release
        PostTrajectory.instance.Clear();
        StartCoroutine(CreatePathPoints());
    }

    IEnumerator CreatePathPoints()
    {
        while (collisionCount < 2) 
        {
            PostTrajectory.instance.CreateCurrentPathPoint(transform.position);
            yield return new WaitForSeconds(PostTrajectory.instance.timeInterval);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Chains"))
        {
            //Bomb bombScript = GetComponent<Bomb>();
            Bomb bombScript = other.gameObject.GetComponentInParent<Bomb>();

            if (bombScript != null)
            {
                bombScript.Trigger();
            }
        }

        switch (ballType)
        {
            case BallType.Bomb:
                ExplodeEffect();
                ballType = BallType.Normal;
                break;

            case BallType.Freeze:
                FreezeEffect();
                ballType = BallType.Normal;
                break;

            case BallType.Explosion:
                ExplosionEffect();
                Invoke("DefaultBallType", 2.0f);
                break;

            case BallType.BoxDestoyer:
                BoxDestoyerEffect(other);
                Invoke("DefaultBallType", GameManager.Instance.resetPositionDelay);
                break;
        }

        StartCoroutine(IncrementCollisionCount());
        SoundManager.instance.PlayBallBoxCollideSound();
    }

    private void BoxDestoyerEffect(Collision2D other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
            GameManager.Instance.UpdateFallenBoxesInstant(other.gameObject);
        }
    }

    private void DefaultBallType()
    {
        ballType = BallType.Normal;
    }

    private void ExplosionEffect()
    {
        Collider2D[] hitBoxes = Physics2D.OverlapCircleAll(transform.position, ExplosionfieldOfImpact);

        foreach (Collider2D col in hitBoxes)
        {
            if (col.CompareTag("Platform"))
            {
                Destroy(col.gameObject);
            }

            if (col.CompareTag("Button"))
            {
                col.GetComponent<Button>().DestroySelf_Laser();
            }

            if (col.CompareTag("Treasure"))
            {
                col.GetComponent<Treasure>().ActivateRb();
            }
        }
    }

    private void FreezeEffect()
    {
        Collider2D[] hitBoxes = Physics2D.OverlapCircleAll(transform.position, FrozefieldOfImpact);

        foreach (Collider2D col in hitBoxes)
        {
            if (col.CompareTag("Box"))
            {
                col.gameObject.layer = LayerMask.NameToLayer("FrozenLayer");

                Box boxScript = col.GetComponent<Box>();
                if (boxScript != null)
                {
                    boxScript.ApplyFreezeEffect();
                }
            }
        }

        windBlower.ActivateWind();
        windBlower2.ActivateWind();
    }

    private void ExplodeEffect()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, BombfieldOfImpact); 
        foreach (Collider2D nearbyObject in colliders)
        {
            Rigidbody2D rb = nearbyObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                Vector2 forceDir = rb.transform.position - transform.position;
                rb.AddForce(forceDir.normalized * force); 
            }
        }

        BallCameraShake();
        GameManager.Instance.ResetBall();
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

    public void SetBallType(BallType type)
    {
        ballType = type;
    }

    private IEnumerator IncrementCollisionCount()
    {
        yield return new WaitForSeconds(0.08f); 
        collisionCount++;
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

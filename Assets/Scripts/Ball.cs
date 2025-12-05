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

    public static Ball Instance;

    [Header("Player Control")]
    public float moveSpeed = 5f;
    public bool useRawInput = true;   
    private float defaultGravity; 

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
        col = GetComponent<CircleCollider2D>();

        if (rb == null)
        {
            Debug.LogError("Ball: Rigidbody2D missing!");
        }
        else
        {
            defaultGravity = rb.gravityScale;
        }

        if (Instance == null) Instance = this;
    }

    private void Update()
    {
        HandlePlayerControl();
    }

    private void HandlePlayerControl()
    {
        // read input (WASD or arrow keys)
        float h = useRawInput ? Input.GetAxisRaw("Horizontal") : Input.GetAxis("Horizontal");
        float v = useRawInput ? Input.GetAxisRaw("Vertical") : Input.GetAxis("Vertical");

        Vector2 input = new Vector2(h, v);

        if (input.sqrMagnitude > 0f)
        {
            rb.gravityScale = 0f;

            Vector2 targetVel = input.normalized * moveSpeed;
            rb.linearVelocity = targetVel;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
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

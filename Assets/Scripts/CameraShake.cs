using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }
    private Vector3 originalPosition;
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;

    public float shakeDuration_small = 0.25f;
    public float shakeMagnitude_small = 0.05f;

    private bool right = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        originalPosition = transform.position;
    }

    public void Shake(bool small = false)
    {
        PlatformColorChanger.Instance.ChangePlatformColors();
        //SoundManager.instance.PlayCameraShakeSound();

        if (!small)
        {
            StartCoroutine(ShakeCoroutine(shakeDuration, shakeMagnitude));
        }
        else
        {
            StartCoroutine(ShakeCoroutine(shakeDuration_small, shakeMagnitude_small));
        }

        if (Balloon.Instance.AlreadyTravelling())
        {
            return;
        }

        if (right)
        {
            right = false;
            Balloon.Instance.EnableMove(true);
        }
        else
        {
            right = true;
            Balloon.Instance.EnableMove(false);
        }
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
    }
}
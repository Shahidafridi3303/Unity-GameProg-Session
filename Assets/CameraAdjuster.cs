using UnityEngine;

public class CameraAdjuster : MonoBehaviour
{
    public float targetAspect = 16f / 9f;

    void Start()
    {
        Camera cam = Camera.main;
        float currentAspect = (float)Screen.width / Screen.height;
        cam.orthographicSize *= targetAspect / currentAspect;
    }
}
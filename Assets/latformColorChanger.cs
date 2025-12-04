using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformColorChanger : MonoBehaviour
{
    private List<SpriteRenderer> platformRenderers = new List<SpriteRenderer>();
    public static PlatformColorChanger Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ChangePlatformColors()
    {
        Invoke("ApplyColor", 0.2f);
        //SoundManager.instance.PlayPlatformChangeSound();
    }

    private void ApplyColor()
    {
        platformRenderers.Clear();
        SpriteRenderer[] renderers = GetComponentsInChildren<SpriteRenderer>();

        foreach (SpriteRenderer renderer in renderers)
        {
            if (renderer != null)
            {
                platformRenderers.Add(renderer);
            }
        }

        Color newColor = GetRandomColor();

        foreach (SpriteRenderer renderer in platformRenderers)
        {
            if (renderer != null)
            {
                renderer.color = newColor;
            }
        }
    }

    private Color GetRandomColor()
    {
        return new Color(Random.value, Random.value, Random.value, 1f);
    }
}
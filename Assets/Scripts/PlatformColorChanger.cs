using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformColorChanger : MonoBehaviour
{
    private List<SpriteRenderer> platformRenderers = new List<SpriteRenderer>();

    [Header("Custom Color Options")]
    public bool useCustomColors = false;      // Toggle ON/OFF  
    public List<Color> allowedColors;         // Colors user can choose

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
    }

    public void ApplyColor()
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
        if (useCustomColors && allowedColors != null && allowedColors.Count > 0)
        {
            int index = Random.Range(0, allowedColors.Count);
            Color c = allowedColors[index];
            c.a = 1f; // ALWAYS force full alpha
            return c;
        }

        // Default random color
        return new Color(Random.value, Random.value, Random.value, 1f);
    }
}

using UnityEngine;

public class Box : MonoBehaviour
{
    public Color freezeColor = Color.cyan;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ApplyFreezeEffect()
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = freezeColor;
        }
    }
}
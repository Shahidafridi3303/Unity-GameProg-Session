using UnityEngine;

public class BoxCollisionChecker : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Box box = other.gameObject.GetComponent<Box>();

        if (box)
        {
            GameManager.Instance.UpdateFallenBoxes(other.gameObject);
        }
    }
}
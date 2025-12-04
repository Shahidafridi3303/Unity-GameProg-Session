using UnityEngine;

public class BoxCollisionChecker : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Box"))
        {
            GameManager.Instance.UpdateFallenBoxes(other.gameObject);
        }
    }
}
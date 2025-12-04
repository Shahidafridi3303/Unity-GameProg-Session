using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootAreaTrigger : MonoBehaviour
{
    public GameObject Slingshot;
    public GameObject SlingshotTeleportLocation;
    public float delayInSeconds = 2f;
    public bool isLeft = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Ball"))
        {
            StartCoroutine(MoveSlingshotAfterDelay());
        }
    }

    private IEnumerator MoveSlingshotAfterDelay()
    {
        yield return new WaitForSeconds(delayInSeconds);

        Slingshot.transform.position = SlingshotTeleportLocation.transform.position;
        Slingshot.GetComponent<Slingshot>().UpdateStripPosition();
        GameManager.Instance.ActivateLootballs(isLeft);
        SoundManager.instance.PlayMoveSound();
        Destroy(gameObject);
    }
}

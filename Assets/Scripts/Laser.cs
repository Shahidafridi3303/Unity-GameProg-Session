using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float defDistanceRay = 100;
    public Transform laserFirePoint;
    public LineRenderer m_lineRenderer;
    private bool isActive = true;
    public bool autoReactivate = false;
    public float reactivateDelay = 3f;
    public Button laserDeactivatorButton;

    private void Update()
    {
        if (isActive)
        {
            ShootLaser();
        }
    }

    void ShootLaser()
    {
        RaycastHit2D _hit = Physics2D.Raycast(laserFirePoint.position, transform.up);

        if (_hit.collider != null && _hit.collider.CompareTag("Ball"))
        {
            Ball.Instance.BallCameraShake();
            GameManager.Instance.ResetBall();
            SoundManager.instance.PlayLaserSound();
        }

        Draw2DRay(laserFirePoint.position, _hit.collider != null ? _hit.point : laserFirePoint.transform.right * defDistanceRay);
    }

    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }

    public void DeactivateLaser()
    {
        isActive = false;
        m_lineRenderer.enabled = false;
        SoundManager.instance.PlayNotYourMoveSound();

        if (autoReactivate)
        {
            StartCoroutine(ReactivateLaserAfterDelay());
        }

        if (laserDeactivatorButton)
        {
            laserDeactivatorButton.UpdateLeverState(false);
        }
    }

    public void ActivateLaser()
    {
        isActive = true;
        m_lineRenderer.enabled = true;
        SoundManager.instance.PlayMoveSound();

        if (laserDeactivatorButton)
        {
            laserDeactivatorButton.UpdateLeverState(true);
        }
    }

    IEnumerator ReactivateLaserAfterDelay()
    {
        yield return new WaitForSeconds(reactivateDelay);
        ActivateLaser();
    }
}
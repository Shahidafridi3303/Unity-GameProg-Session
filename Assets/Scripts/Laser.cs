using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private float defDistanceRay = 100;
    public Transform laserFirePoint;
    public LineRenderer m_lineRenderer;
    private bool isActive = true;
    private bool PlayerDead = false;

    private void Start()
    {
        ActivateLaser();
    }

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

        if (_hit.collider != null)
        {
            // Look for Ball script instead of tag
            Ball ball = _hit.collider.GetComponent<Ball>();

            if (ball != null)
            {
                if (PlayerDead) return;
                PlayerDead = true;

                ball.BallCameraShake();
                GameManager.Instance.OpenFailureWithDelay();
            }
        }

        Draw2DRay(
            laserFirePoint.position,
            _hit.collider != null ? _hit.point : laserFirePoint.transform.right * defDistanceRay
        );
    }


    void Draw2DRay(Vector2 startPos, Vector2 endPos)
    {
        m_lineRenderer.SetPosition(0, startPos);
        m_lineRenderer.SetPosition(1, endPos);
    }

    public void ActivateLaser()
    {
        isActive = true;
        m_lineRenderer.enabled = true;
    }
}
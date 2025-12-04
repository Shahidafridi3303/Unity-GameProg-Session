using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBlower : MonoBehaviour
{
    private AreaEffector2D areaEffector;
    private MovingPlatform movingPlatform;

    public float magnitude = 22f;
    public float deactivateTime = 5f;

    void Start()
    {
        areaEffector = GetComponent<AreaEffector2D>();
        movingPlatform = GetComponent<MovingPlatform>();

        gameObject.SetActive(false);
        
        areaEffector.forceMagnitude = 0f;
        movingPlatform.canPerform = false;
    }

    public void ActivateWind()
    {
        gameObject.SetActive(true);

        areaEffector.forceMagnitude = magnitude;
        movingPlatform.canPerform = true;

        Invoke("DeactivateWind", deactivateTime);
    }

    public void DeactivateWind()
    {
        gameObject.SetActive(false);

        areaEffector.forceMagnitude = 0f;
        movingPlatform.canPerform = false;
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PostTrajectory : MonoBehaviour
{
    public GameObject[] pathTemplates;

    public static PostTrajectory instance;

    public List<GameObject> lastPoints;

    public float timeInterval;

    int lastIndex = 0;

    void Start()
    {
        instance = this;
        lastPoints = new List<GameObject>();
    }

    public void CreateCurrentPathPoint(Vector3 position)
    {
        GameObject point = Instantiate(pathTemplates[lastIndex], position, Quaternion.identity, transform);
        point.SetActive(true);
        lastPoints.Add(point);

        lastIndex++;

        if (lastIndex == pathTemplates.Length)
            lastIndex = 0;
    }

    public void Clear()
    {
        lastPoints.ForEach((obj) => Destroy(obj));
        lastPoints.Clear();
        lastIndex = 0;
    }
}
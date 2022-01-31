using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circlePositionRandomizer : MonoBehaviour
{
    public float radius = 2.0f;
    public Vector2 center = new Vector2(0f, 0f);

    private float spawnX, spawnZ;
    private const float TWOPI = 2 * Mathf.PI;
    private float randomAngle;

    public void RandomizePosition()
    {
        randomAngle = Random.Range(0f, TWOPI);
        spawnX = radius * Mathf.Cos(randomAngle) + center.x;
        spawnZ = radius * Mathf.Sin(randomAngle) + center.y;
        transform.localPosition = new Vector3(spawnX, 0, spawnZ);
    }
     
    private void Awake()
    {
        RandomizePosition();
    }
}
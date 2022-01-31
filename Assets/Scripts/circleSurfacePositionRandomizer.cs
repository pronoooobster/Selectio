using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class circleSurfacePositionRandomizer : MonoBehaviour
{
    public float radiusMax = 2.0f;
    public Vector2 center = new Vector2(0f, 0f);

    private float spawnX, spawnZ;
    private const float TWOPI = 2 * Mathf.PI;
    private float randomAngle;
    private float radiusCurrent;

    private float randomNormalized()
    {
        return Random.Range(0.0f, 1.0f);        // float number in the range of 0 to 1
    }

    public void RandomizePosition()
    {
                                                                                                // radius randomization normalized
        radiusCurrent = radiusMax * Mathf.Sqrt(randomNormalized());
                                                                                                // angle randomization from 0 to 360 deg
        randomAngle = Random.Range(0f, TWOPI);
                                                                                                // spawn position calculation & change position
        spawnX = radiusCurrent * Mathf.Cos(randomAngle) + center.x;
        spawnZ = radiusCurrent * Mathf.Sin(randomAngle) + center.y;
        transform.localPosition = new Vector3(spawnX, 0, spawnZ);
        this.transform.localEulerAngles = new Vector3(this.transform.localEulerAngles.x, Random.Range(0f, 360f), this.transform.localEulerAngles.z);                              // randomize rotation around Y axis
    }

    
}

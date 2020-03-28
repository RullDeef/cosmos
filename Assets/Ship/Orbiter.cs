using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbiter : MonoBehaviour
{
    public Transform targetTransform;

    // circles per second
    public float orbitSpeed = 0.3f;

    public float orbitRadius = 3.0f;

    // offset in circles
    [Range(0.0f, 1.0f)]
    public float angleOffset = 0.0f;

    public Vector3 orbitAxis = Vector3.up;
    public Vector3 startingOffset = Vector3.right;

    public void SetOrbitAxis(Vector3 axis)
    {
        orbitAxis = axis.normalized;
    }

    public void SetStartingOffset(Vector3 offset)
    {
        startingOffset = offset - orbitAxis * Vector3.Dot(orbitAxis, offset);
        startingOffset = startingOffset.normalized;
    }

    public void InitRandomInThor(Vector3 axis, float r1, float r2, float minSpeed = 0.1f, float maxSpeed = 0.3f)
    {
        SetOrbitAxis(axis);
        SetStartingOffset(Vector3.right);

        orbitSpeed = Random.Range(minSpeed, maxSpeed);
        orbitRadius = Random.Range(r1 - r2, r1 + r2);
        angleOffset = Random.Range(0.0f, 1.0f);
    }

    private void Update()
    {
        float angle = angleOffset + orbitSpeed * Time.time;
        angle *= 180.0f;

        Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.AngleAxis(angle, orbitAxis));

        Vector3 offset = rotation.MultiplyVector(startingOffset);
        offset = orbitRadius * offset.normalized;

        Vector3 forward = Vector3.Cross(orbitAxis, offset);
        forward *= Mathf.Sign(orbitSpeed);

        transform.position = targetTransform.position + offset;
        transform.rotation = Quaternion.LookRotation(forward, orbitAxis);
    }
}

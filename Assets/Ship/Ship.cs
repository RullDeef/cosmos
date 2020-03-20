using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public enum ShipState
    {
        Idle = 0,
        Flow = 1
    }

    public ShipState state = ShipState.Idle;
    public Transform target;

    /**
     * Every frame updates position of ship based
     * on current ship state and target transform.
     */
    void Update()
    {
        if (state == ShipState.Idle)
        {
            Vector3 newPosition = transform.localPosition;
            Quaternion rotation = Quaternion.Euler(0, 0, 90 * Time.deltaTime);
            Matrix4x4 matrix = Matrix4x4.Rotate(rotation);
            newPosition = matrix.MultiplyPoint(newPosition);
            transform.localPosition = newPosition;
        }
    }
}

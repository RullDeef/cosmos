using UnityEngine;

[System.Serializable, ExecuteAlways]
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
        if (state == ShipState.Idle && target != null)
        {
            Vector3 delta = transform.position - target.position;
            Quaternion rotation = Quaternion.Euler(0, 0, 90 * Time.deltaTime);
            Matrix4x4 matrix = Matrix4x4.Rotate(rotation);

            Vector3 newPosition = target.position + matrix.MultiplyPoint(delta);
            transform.position = newPosition;
        }
    }
}

using UnityEngine;

/**
 * \brief   Перечисление всех возможных состояний корабля.
 *
 * - idle - патрулирование системы
 * - flow - перелёт между системами
 */
public enum ShipState
{
    idle = 0,
    flow = 1
}

[System.Serializable, ExecuteAlways]
public class Ship : MonoBehaviour
{
    public ShipState state = ShipState.idle;

    // TODO: think about what this variable represents actually
    // because the same thing exists also in fleet structure...
    // Need to create something that loads target transform
    // from containee fleet..

    // SOLUTION: this varible must be assigned only from fleet
    public Vector3 targetPosition;
    public float orbitRadius;

    /**
     * \brief   Используется при перелёте, чтобы
     *          указать относительную позицию.
     */
    private Vector3 offset;

    public void ChangeState()
    {
        if (state == ShipState.idle)
        {
            state = ShipState.flow;
            offset = targetPosition - transform.position;
            offset /= orbitRadius;
        }
        else
        {
            state = ShipState.idle;
        }
    }

    /**
     * \brief   Каждый кадр обновляет позицию корабля основываясь
     *          на текущем состоянии корабля и его целевом объекте.
     * 
     * Поворот реализуется так, что корабль всегда вращается
     * в плоскости перпендикулярной направлению взгяда камеры.
     */
    void Update()
    {
        // TODO: add camera-depend motion
        if (state == ShipState.idle && targetPosition != null)
        {
            Vector3 delta = transform.position - targetPosition;
            Quaternion rotation = Quaternion.Euler(0, 0, 90 * Time.deltaTime);
            Matrix4x4 matrix = Matrix4x4.Rotate(rotation);

            delta = matrix.MultiplyPoint(delta);
            delta = orbitRadius * delta.normalized;
            transform.position = targetPosition + delta;
        }
        else if (state == ShipState.flow)
        {
            Vector3 delta = orbitRadius * offset;
            transform.position = targetPosition + delta;
        }
    }
}

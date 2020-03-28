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

    void Update()
    {
        if (state == ShipState.flow)
        {
            Vector3 delta = orbitRadius * offset;
            transform.position = targetPosition + delta;
        }
    }
}

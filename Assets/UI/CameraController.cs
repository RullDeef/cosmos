/**
 * \file    CameraController.cs
 * \brief   File with CameraController definition.
 */
using System.Collections.Generic;
using UnityEngine;

/**
 * Monobehaviour. Controlls camera movement and zooming.
 */
public class CameraController : MonoBehaviour
{
    /**
     * Главный обозреваемый объект
     */
    private GameObject mainObservable;
    /**
     * Список обозреваемых объектов
     */
    private List<GameObject> observables = new List<GameObject>();

    private Vector3 targetPosition;
    private float targetDistance;
    private float speed = 5.0f;

    public Vector3 defaultObservablePosition = Vector3.zero;

    [Range(0, 50)]
    public float defaultObservableDistance = 5.0f;

    [Range(1, 10)]
    public float mouseRotationSpeed = 3.0f;
    
    /**
     * Field of view (FOV) for camera controller.
     */
    private const float observableAngle = 60.0f * Mathf.PI / 180.0f;

    private InputController inputController;

    private void Awake()
    {
        inputController = GameObject.FindObjectOfType<InputController>();
    }

    public void SetMainObservable(GameObject obj)
    {
        if (observables.Contains(obj))
            observables.Remove(obj);
        mainObservable = obj;

        RecalculateObservableParams();
    }

    public void AddObservable(GameObject obj)
    {
        if (obj != mainObservable && !observables.Contains(obj))
            observables.Add(obj);

        RecalculateObservableParams();
    }

    public void ClearObservables()
    {
        mainObservable = null;
        observables.Clear();

        RecalculateObservableParams();
    }

    /**
     * Пересчитывает позицию и дистанцию наблюдения исходя
     * из текущих обозреваемых объектов.
     */
    private void RecalculateObservableParams()
    {
        if (mainObservable != null)
        {
            // calculate size of current group based on the max distance from first observable.
            targetPosition = mainObservable.transform.position;
        }
        else if (observables.Count == 0)
        {
            targetPosition = defaultObservablePosition;
        }
        else
        {
            // dont stick to particular observable
            targetPosition = Vector3.zero;
            foreach (GameObject obj in observables)
                targetPosition += obj.transform.position;
            targetPosition /= observables.Count;
        }

        float maxDistance = observables.Count == 0 ? defaultObservableDistance : 0.0f;

        foreach (GameObject obj in observables)
        {
            float currDistance = (obj.transform.position - targetPosition).magnitude;
            maxDistance = Mathf.Max(maxDistance, currDistance);
        }

        targetDistance = 1.25f * maxDistance / Mathf.Sin(observableAngle / 2);
    }

    /**
     * Applies velocity and rotation to current object (attached to Camera) transform.
     * 
     * Used only in Update method.
     */
    private void UpdateTransform()
    {
        if (!Mathf.Approximately((transform.position - targetPosition).magnitude, targetDistance))
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            float velocity = ((transform.position - targetPosition).magnitude - targetDistance) * speed;
            transform.position += velocity * Time.deltaTime * direction;
        }

        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(targetPosition - transform.position), 0.05f);
    }

    private void Update()
    {
        if (inputController.IsDragging())
        {
            // look around
            float dx = mouseRotationSpeed * Input.GetAxis("Mouse X");
            float dy = mouseRotationSpeed * Input.GetAxis("Mouse Y");

            Matrix4x4 translateMatrix = Matrix4x4.Translate(targetPosition);
            Matrix4x4 rotateMatrix = Matrix4x4.Rotate(Quaternion.Euler(-dy * 0, dx, 0));
            Matrix4x4 result = translateMatrix * rotateMatrix * translateMatrix.inverse;

            transform.position = result.MultiplyPoint(transform.position);
            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
        }

        UpdateTransform();
    }
}

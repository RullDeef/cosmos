/**
 * \file    CameraController.cs
 * \brief   File with CameraController definition.
 */
using UnityEngine;

/**
 * Monobehaviour. Controlls camera movement and zooming.
 */
public class CameraController : MonoBehaviour
{
    private Graph graph;

    private PlanetSystem observablePlanetSystem;

    private Vector3 targetPosition;
    private float targetDistance;
    private float speed = 5.0f;

    [Range(1, 10)]
    public float mouseRotationSpeed = 3.0f;

    /**
     * Mouse dragging state tracker.
     */
    private bool mouseIsDragging;
    
    /**
     * Field of view (FOV) for camera controller.
     */
    private const float observableAngle = 60.0f * Mathf.PI / 180.0f;

    private SystemSelector systemSelector;

    /**
     * Prepares speed to target position and orientation
     * to fully see sphere with given center and radius.
     */
    private void UpdateTargetObservable(Vector3 center, float radius)
    {

        targetPosition = center;
        targetDistance = 0.5f * radius / Mathf.Sin(observableAngle / 2.0f);
    }

    /**
     * Makes target currently selected planet and
     * updates speed and orientation to that target.
     * 
     * Used only in Update method.
     */
    private void UpdateTargetObservablePlanet()
    {
        // calculate size of current group based on the max distance.
        float maxDistance = 0.0f;
        float currDistance;

        foreach (PlanetSystem system in graph.GetNeighbors(observablePlanetSystem))
        {
            currDistance = (system.transform.position - observablePlanetSystem.transform.position).magnitude;
            maxDistance = Mathf.Max(maxDistance, currDistance);
        }

        UpdateTargetObservable(observablePlanetSystem.transform.position, 2.5f * maxDistance);
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
        if (graph == null)
        {
            graph = GameObject.FindObjectOfType<GraphGenerator>().graph;
            if (graph == null) 
            {
                Debug.Log("Graph is null!");
                return;
            }

            // look to entire graph at start
            Vector3 size = graph.GetSize();

            // update target observable
            UpdateTargetObservable(Vector3.zero, size.magnitude);
        }

        if (Input.GetMouseButtonUp(0) && !mouseIsDragging)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                PlanetSystem planetSystem = hit.transform.parent.GetComponent<PlanetSystem>();
                if (planetSystem != null)
                {
                    observablePlanetSystem = planetSystem;
                    UpdateTargetObservablePlanet();

                    if (systemSelector == null)
                        systemSelector = GameObject.FindObjectOfType<SystemSelector>();
                    systemSelector.HandleSelection(planetSystem);
                }
            }
            else
            {
                // look to entire graph on click in sky
                Vector3 size = graph.GetSize();
                UpdateTargetObservable(Vector3.zero, size.magnitude);

                if (systemSelector == null)
                    systemSelector = GameObject.FindObjectOfType<SystemSelector>();
                systemSelector.HandleCancelation();
            }
        }
        if (Input.GetMouseButton(0))
        {
            // look around
            float dx = mouseRotationSpeed * Input.GetAxis("Mouse X");
            float dy = mouseRotationSpeed * Input.GetAxis("Mouse Y");

            if (dx * dx + dy * dy > 0.0)
            {
                mouseIsDragging = true;
            }

            Matrix4x4 translateMatrix = Matrix4x4.Translate(targetPosition);
            Matrix4x4 rotateMatrix = Matrix4x4.Rotate(Quaternion.Euler(-dy * 0, dx, 0));
            Matrix4x4 result = translateMatrix * rotateMatrix * translateMatrix.inverse;

            transform.position = result.MultiplyPoint(transform.position);
            transform.rotation = Quaternion.LookRotation(targetPosition - transform.position);
        }
        if (Input.GetMouseButtonDown(0))
        {
            mouseIsDragging = false;
        }

        UpdateTransform();
    }
}

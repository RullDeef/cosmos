using UnityEngine;

/**
 * Класс, осуществляющий доступ к текущему состоянию ввода.
 * Содержит методы доступа к методу ввода.
 */
public class InputController : MonoBehaviour
{
    private bool mouseWasPressed;
    private bool mousePressed;
    private bool mouseDragged;

    private Vector3 mouseLastPos = Vector3.zero;
    private Vector3 mouseDelta = Vector3.zero;

    /**
     * Возвращает true, если было в текущем кадре было нажатие.
     */
    public bool IsClicked()
    {
        return mouseWasPressed;
    }

    /**
     * Возвращает true, если с момента последнего нажатия
     * было движение мышки.
     */
    public bool IsDragging()
    {
        return mouseDragged;
    }

    public Vector3 GetTouchPosition()
    {
        return mouseLastPos;
    }

    public Vector3 GetDraggingDelta()
    {
        return mouseDelta;
    }

    private void Update()
    {
        mousePressed = Input.GetMouseButton(0);

        if (mousePressed)
        {
            float dx = Input.GetAxis("Mouse X");
            float dy = Input.GetAxis("Mouse Y");

            if (dx * dx + dy * dy > float.Epsilon * float.Epsilon)
                mouseDragged = true;
        }

        mouseWasPressed = false;
        if (Input.GetMouseButtonUp(0))
        {
            mouseWasPressed = !mouseDragged;
            mouseDragged = false;
        }

        // update mouse position and delta
        mouseDelta = Input.mousePosition - mouseLastPos;
        mouseLastPos = Input.mousePosition;
    }
}

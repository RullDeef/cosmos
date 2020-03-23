using System.Collections.Generic;
using UnityEngine;
using TMPro;

/**
 * Синглтон-класс для управления системами.
 * 
 * Мониторит, какие системы выделены в данный момент времени,
 * подсвечивая их. (не реализовано)
 */
public class SystemSelector : MonoBehaviour
{
    public enum SelectionState
    {
        current,
        target
    }

    /**
     * Переменная текущего состояния выделения.
     * Может принимать значения:
     *   Выбор текущей системы (current)
     *   Выбор целевых систем (target)
     */
    public SelectionState state = SelectionState.current;

    /**
     * Текущая выбранная система.
     */
    public PlanetSystem currentSystem;

    /**
     * Список выбранных целевых систем.
     */
    public List<PlanetSystem> targetSystems;

    /**
     * Ссылка на текст для вывода отладочной информации
     * о количестве выбранных систем.
     */
    public TMP_Text text;

    private InputController inputController;
    private CameraController cameraController;

    private void Awake()
    {
        inputController = GameObject.FindObjectOfType<InputController>();
        cameraController = GameObject.FindObjectOfType<CameraController>();

        UpdateCameraObservables();
    }

    /**
     * Обработчик события выделения системы кликом на неё.
     */
    public void HandleSelection(PlanetSystem system)
    {
        switch (state)
        {
            case SelectionState.current:
                currentSystem = system;
                break;
            
            case SelectionState.target:
                if (targetSystems.Contains(system))
                    targetSystems.Remove(system);
                else
                    targetSystems.Add(system);
                break;
        }

        // updates camera observing objects
        UpdateCameraObservables();
    }

    /**
     * Обработчик события отмены выделения (клик по пустому пространству).
     */
    public void HandleCancelation()
    {
        switch (state)
        {
            case SelectionState.current:
                currentSystem = null;
                break;
            
            case SelectionState.target:
                // do nothing
                break;
        }

        UpdateCameraObservables();
    }

    private void UpdateCameraObservables()
    {
        cameraController.ClearObservables();
        if (currentSystem != null)
        {
            cameraController.SetMainObservable(currentSystem.gameObject);
            foreach (PlanetSystem s in targetSystems)
                cameraController.AddObservable(s.gameObject);
        }
        else
        {
            // observe entire graph
            GraphGenerator generator = GameObject.FindObjectOfType<GraphGenerator>();
            PlanetSystem[] systems = generator.GetComponentsInChildren<PlanetSystem>();
            foreach (PlanetSystem system in systems)
                cameraController.AddObservable(system.gameObject);
        }
    }

    private void UpdateText()
    {
        string textString;
        if (currentSystem == null)
            textString = "Текущая система не выбрана";
        else
            textString = $"Текущая система: {currentSystem.gameObject.name}";

        if (targetSystems.Count == 0)
            textString += "\nЦелевые системы не выбраны";
        else
        {
            textString += "\nЦелевые системы:";
            for (int i = 0; i < targetSystems.Count; i++)
                textString += $"\n  {i + 1}: {targetSystems[i].gameObject.name}";
        }

        text.text = textString;
    }

    /**
     * Обработка кликов по системам.
     * Обновление текстовых переменных.
     */
    private void Update()
    {
        if (inputController.IsClicked())
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                PlanetSystem planetSystem = hit.transform.parent.GetComponent<PlanetSystem>();
                if (planetSystem != null)
                    HandleSelection(planetSystem);
            }
            else
                HandleCancelation();
        }

        UpdateText();
    }
}
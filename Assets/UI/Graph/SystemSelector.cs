using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/**
 * \brief   Синглтон-класс для управления системами.
 * 
 * Мониторит, какие системы выделены в данный момент времени,
 * подсвечивая их. (не реализовано)
 */
public class SystemSelector : MonoBehaviour
{
    /**
     * \brief   Печерисление всех возможных состояний выделения систем.
     * 
     * current - выбор текущей системы
     * target - выбор целевых систем
     */
    public enum SelectionState
    {
        current,
        target
    }

    /**
     * \brief   Переменная текущего состояния выделения.
     */
    public SelectionState state = SelectionState.current;

    /**
     * \brief   Текущая выбранная система.
     */
    public PlanetSystem currentSystem;

    /**
     * \brief   Список выбранных целевых систем.
     */
    public List<PlanetSystem> targetSystems = new List<PlanetSystem>();

    /**
     * \brief   Ссылка на текст для вывода отладочной
     *          информации о количестве выбранных систем.
     */
    public TMP_Text text;

    /**
     * \brief   Кнопка для оправки флота на целевые
     *          системы (для отладки онли).
     */
    public Button sendButton;

    private InputController inputController;
    private CameraController cameraController;
    private FleetManager fleetManager;

    private void Awake()
    {
        inputController = GameObject.FindObjectOfType<InputController>();
        cameraController = GameObject.FindObjectOfType<CameraController>();
        fleetManager = GameObject.FindObjectOfType<FleetManager>();

        UpdateCameraObservables();
    }

    public void HandleSendButtonClick()
    {
        switch (state)
        {
            case SelectionState.current:
                state = SelectionState.target;
                sendButton.GetComponentInChildren<TMP_Text>().text = "Разослать флоты";
                break;
            
            case SelectionState.target:
                state = SelectionState.current;
                sendButton.GetComponentInChildren<TMP_Text>().text = "Выбрать целевые системы";
                SendFleet();
                targetSystems.Clear();
                break;
        }
    }

    /**
     * \brief   Метод, распределяющий флот с текущей системы
     *          по соседним выбранным в качестве целевых.
     */
    public void SendFleet()
    {
        if (currentSystem == null)
        {
            Debug.Log("Current system is not selected!");
            return;
        }

        if (targetSystems.Count == 0)
        {
            Debug.Log("Target systems are not selected!");
            return;
        }

        Fleet currentFleet = fleetManager.GetAssociatedFleet(currentSystem);
        int shipsAmount = currentFleet.ships.Count;
        int sendAmount = shipsAmount / targetSystems.Count;

        foreach (PlanetSystem targetSystem in targetSystems)
        {
            if (targetSystem == currentSystem)
                continue;
            
            Fleet newFleet = fleetManager.SplitPart(currentFleet, sendAmount);
            StartCoroutine(fleetManager.SendFleetFromTo(newFleet, currentSystem, targetSystem));
            Debug.Log($"Successfully sended {sendAmount} ships to anouter system!");
        }
    }

    /**
     * \brief   Обработчик события выделения системы кликом на неё.
     *
     * Если текущее состояние - выбор текущей системы, то
     * клик по системе сделает её текущей.
     * 
     * Если текущее состояние - выбор целевых систем, то
     * клик по системе сменит её состояние (выбрано/не выбрано).
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

        UpdateCameraObservables();
    }

    /**
     * \brief   Обработчик события отмены выделения
     *          (клик по пустому пространству).
     *
     * При нахождении в состоянии "выделение текущей
     * системы" просто сбрасывает выделение.
     */
    public void HandleCancelation()
    {
        switch (state)
        {
            case SelectionState.current:
                currentSystem = null;
                break;
            
            case SelectionState.target:
                break;
        }

        UpdateCameraObservables();
    }

    /**
     * \brief   Обновляет обозреваемые объекты у контроллера камерой.
     */
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

    // TODO: refactor this method to achieve best performance
    private void UpdateText()
    {
        string textString;
        if (currentSystem == null)
            textString = "Текущая система не выбрана";
        else
        {
            textString = $"Текущая система: {currentSystem.gameObject.name.Substring(25)} ";
            int shipsAmount = 0;
            Fleet fleet = fleetManager.GetAssociatedFleet(currentSystem);
            if (fleet != null)
                shipsAmount = fleet.ships.Count;
            textString += $"с фтолом в {shipsAmount} кораблей.";
        }

        if (targetSystems.Count == 0)
            textString += "\nЦелевые системы не выбраны";
        else
        {
            textString += "\nЦелевые системы:";
            for (int i = 0; i < targetSystems.Count; i++)
                textString += $"\n  {i + 1}: {targetSystems[i].gameObject.name.Substring(25)}";
        }

        text.text = textString;
    }

    /**
     * \brief   Обработка кликов по системам.
     * 
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
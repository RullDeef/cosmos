using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetManager : MonoBehaviour
{
    public Ship shipPrefab;

    public List<Fleet> fleets = new List<Fleet>();

    /**
     * \brief   Используется для создания нового флота
     *          (для отладки онли?).
     */
    public int shipsAmount;

    /**
     * \brief   Время в секундах, необходимое для перелёта
     *          флота с одной системы на другую.
     */
    public float flightTime = 2.0f;

    private PlanetSystemGenerator planetSystemGenerator;
    private SystemSelector systemSelector;

    private void Awake()
    {
        planetSystemGenerator = GameObject.FindObjectOfType<PlanetSystemGenerator>();
        systemSelector = GameObject.FindObjectOfType<SystemSelector>();
    }

    /**
     * \brief   Создаёт флот в текущей выбранной системе (для отладки онли?).
     */
    public Fleet CreateNewFleet(int shipsAmount)
    {
        PlanetSystem currentSystem = systemSelector.currentSystem;
        if (currentSystem == null)
        {
            Debug.Log("Attempt to create new fleet when selected no system");
            return null;
        }

        Fleet fleet = new Fleet();
        Transform ownerTransform = currentSystem.transform;

        // generate new ships using loop
        for (int i = 0; i < shipsAmount; i++)
        {
            // place new ship correctly
            float radius = GameObject.FindObjectOfType<PlanetSystemGenerator>().GetShipOrbitRadius();
            Vector3 direction = Random.onUnitSphere;
            Vector3 position = ownerTransform.position + radius * direction;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);
            
            Ship ship = Instantiate(shipPrefab, position, rotation, transform);
            fleet.ships.Add(ship);
            Debug.Log("New ship generated!");
        }

        // TODO: add method to calculate ship orbit radius
        fleet.SetOwner(currentSystem.gameObject, 4 * planetSystemGenerator.planetOrbitStep);
        fleets.Add(fleet);
        Debug.Log($"Added new fleet! owner = {fleets[fleets.Count - 1].GetOwner()}");
        return fleet;
    }

    /**
     * \brief   Удаляет все имеющиеся флоты (для отладки онли).
     */
    public void RemoveAllFleets()
    {
        try
        {
            while (transform.childCount != 0)
            {
                Transform childTransform = transform.GetChild(0);
                if (Application.isEditor)
                    DestroyImmediate(childTransform.gameObject);
                else
                    Destroy(childTransform.gameObject);
            }
        }
        catch
        {
            Debug.Log("Seems like you have deleted ships manually! All ok");
        }

        fleets.Clear();
    }

    /**
     * \brief   Производит поиск флота, привязанного к переданной системе.
     */
    public Fleet GetAssociatedFleet(PlanetSystem system)
    {
        foreach (Fleet fleet in fleets)
            if (fleet.GetOwner() == system.gameObject)
                return fleet;
        Debug.Log("No assiciated fleet with given planet system!");
        return null;
    }

    public Fleet SplitPart(Fleet sourceFleet, int splitAmount)
    {
        Fleet newFleet = new Fleet();

        if (splitAmount > sourceFleet.ships.Count)
            Debug.Log("Trying to split more than there are!");
        else
        {
            for (int i = 0; i < splitAmount; i++)
            {
                Ship ship = sourceFleet.ships[0];
                newFleet.ships.Add(ship);
                sourceFleet.ships.RemoveAt(0);
            }
        }

        return newFleet;
    }

    public IEnumerator SendFleetFromTo(Fleet fleet, PlanetSystem from, PlanetSystem to)
    {
        float startTime = Time.time;
        float currTime;
        float t;

        // TODO: wrap this into method
        float maxOrbitRadius = 4 * planetSystemGenerator.planetOrbitStep;
        float orbitRadius;

        Vector3 fromPosition = from.transform.position;
        Vector3 toPosition = to.transform.transform.position;
        Vector3 position;

        foreach (Ship ship in fleet.ships)
        {
            ship.targetPosition = fromPosition;
            ship.orbitRadius = maxOrbitRadius;
            ship.ChangeState(); // to flow
        }

        while (true)
        {
            currTime = Time.time;
            if (currTime - startTime > flightTime)
                break;

            t = (currTime - startTime) / flightTime;

            orbitRadius = maxOrbitRadius * (1 - 12 * Mathf.Pow(t * t - t, 2));
            position = fromPosition * (t - 1) + toPosition * t;

            foreach (Ship ship in fleet.ships)
            {
                ship.targetPosition = position;
                ship.orbitRadius = orbitRadius;
            }

            yield return new WaitForFixedUpdate();
        }

        foreach (Ship ship in fleet.ships)
            ship.ChangeState(); // to idle back

        fleet.SetOwner(to.gameObject, maxOrbitRadius);
        yield return null;
    }
}

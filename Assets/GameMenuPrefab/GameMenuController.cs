using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    public Transform camera;
    public bool isSelected = false;
    public GameObject menuSystem;
    public GameObject[] planets;
    public Text shipText;

    void Start()
    {
        
    }

    void Update()
    {
        transform.LookAt(camera);

        if(isSelected == false)
        {
            menuSystem.SetActive(false);
        }
        else
        {
            menuSystem.SetActive(true);
        }
    }
}

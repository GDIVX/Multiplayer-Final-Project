using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject optionMenu;

    private void Start()
    {
        optionMenu.SetActive(false);


    }

    private void Update()
    {
        //toggle the menu on and off
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            optionMenu.SetActive(!optionMenu.activeInHierarchy);
        }
    }
}

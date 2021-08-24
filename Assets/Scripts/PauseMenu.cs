using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject menu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(!menu.activeSelf);
            Cursor.lockState = (menu.activeSelf ? CursorLockMode.None : CursorLockMode.Locked);
        }
    }
}

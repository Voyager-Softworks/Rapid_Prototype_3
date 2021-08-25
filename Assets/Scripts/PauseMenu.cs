using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject menu;
    public GameObject win;
    public GameObject lose;

    public void Pause()
    {
        Cursor.lockState = CursorLockMode.None;
        menu.SetActive(true);
        win.SetActive(false);
        lose.SetActive(false);
    }

    public void EnableWinScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        menu.SetActive(false);
        win.SetActive(true);
        lose.SetActive(false);
    }

    public void EnableLoseScreen()
    {
        Cursor.lockState = CursorLockMode.None;
        menu.SetActive(false);
        win.SetActive(false);
        lose.SetActive(true);
    }

    public void Unpause()
    {
        Cursor.lockState = CursorLockMode.Locked;
        menu.SetActive(false);
        win.SetActive(false);
        lose.SetActive(false);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!win.activeSelf && !lose.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            if (menu.activeSelf) {
                Unpause();
            }
            else
            {
                Pause();
            }
        }
    }
}

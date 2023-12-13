using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject PauseMenu;
    public bool pauseMenuOpen;
    

    public bool gamePaused;
    // Start is called before the first frame update
    void Start()
    {
        PauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (gamePaused)
        {
            PauseGame();
        }

        if (!gamePaused)
        {
            UnPauseGame();
        }
    }

    public void OnPauseMenuToggle(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            pauseMenuOpen = !pauseMenuOpen;
        
        }

    }

    public void OpenPauseMenu()
    {
        if (pauseMenuOpen)
        {
            PauseMenu.SetActive(true);
            gamePaused = true;
        }
        else 
        {
            PauseMenu.SetActive(false);
            gamePaused = false;
        }
    
    
    }

    public void PauseGame() 
    {

        Time.timeScale = 0f;
    }

    public void UnPauseGame() 
    {
        Time.timeScale = 1;
    
    }

}

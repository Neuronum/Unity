using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gamePaused = false;

    public GameObject pauseMenuUI;

    private Animator anim; 

    private void Start()
    {
        anim = pauseMenuUI.GetComponent<Animator>();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        gamePaused = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        anim.Play("MenuFade");
        //pauseMenuUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        gamePaused =  false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
        gamePaused = true;
    }

    public void LoadMenu()
    {
        Debug.Log("Load Main Menu!");
        SceneManager.LoadScene(0);
    }

    public void Restart()
    {
        Debug.Log("restart game!");
        Resume();
        SceneManager.LoadScene(1);
        Debug.Log(gamePaused);
        //Time.timeScale = 1f;
        //gamePaused = false;
    }
}


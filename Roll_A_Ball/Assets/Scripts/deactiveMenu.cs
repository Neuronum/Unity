using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deactiveMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;

    void _deactive()
    {
        pauseMenuUI.SetActive(false);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableScript : MonoBehaviour
{
    public GameObject main_camera;

    void enableScript()
    {
        main_camera.GetComponent<PlayerFollow>().enabled = true;
        main_camera.GetComponent<Animator>().enabled = false;
    }
}

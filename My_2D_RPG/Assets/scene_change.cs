using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class scene_change : MonoBehaviour
{
    
    

    public void sceneChange()
    {
        Debug.Log("Scene Change Triggered");
        SceneManager.LoadScene("Scene2");
    }
}

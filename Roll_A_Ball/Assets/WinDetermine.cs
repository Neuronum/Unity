using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinDetermine : MonoBehaviour
{
    public GameObject WinMenuUI;
    public GameObject Sphere;
    private Vector3 SpherePos;
    public GameObject Destination; // 洞的位置
    private Vector3 desPos; //洞的坐标
    public GameObject _camera;
    float count;

    // Start is called before the first frame update
    void Start()
    {
        desPos = Destination.GetComponent<Transform>().position;
        count = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Distance" + Vector3.Distance(SpherePos, desPos));
        SpherePos = Sphere.GetComponent<Transform>().position; //获取球的位置信息

        if (Vector3.Distance(SpherePos, desPos) < 0.2f) //如果球进洞
        {
            count += Time.deltaTime; //利用count判断球在洞中位置是否高于1秒
            if (count >= 1.0f)
            {
                WinMenuUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                Time.timeScale = 0f; // Pause Game
                _camera.GetComponent<PlayerFollow>().enabled = false; // Freeze Camera
                GetComponent<PauseMenu>().enabled = false;
            }
        }
        else
            count = 0f;
    }

    public void finalRestart()
    {
        Cursor.lockState = CursorLockMode.Locked; //再次隐藏鼠标
        Time.timeScale = 1f; //继续动画
        //重新加载场景无需设置Component状态, 一切重置
        //GetComponent<PauseMenu>().enabled = true;
        //_camera.GetComponent<PlayerFollow>().enabled = true; //release camera
        SceneManager.LoadScene(1);
    }

    public void finalMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}

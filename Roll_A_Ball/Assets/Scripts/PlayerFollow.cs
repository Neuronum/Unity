using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    public Transform PlayerTransform;

    private Vector3 _cameraOffset;
    private Vector3 rotateVec; 
    private Vector3 newOffset; //暂时存储新计算出的cameraOffset，用来插值，已达到平滑效果

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _cameraOffset = transform.position - PlayerTransform.position;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        //获取摄像机的Rotation参数
        //Debug.Log("x: " + transform.rotation.eulerAngles.x + " y: " + transform.rotation.eulerAngles.y + " z: " + transform.rotation.eulerAngles.z);
        if (PauseMenu.gamePaused == false)
        {
            //鼠标X轴控制方向上进行平滑
            newOffset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * 10.0f, Vector3.up) * _cameraOffset;
            _cameraOffset = Vector3.Slerp(_cameraOffset, newOffset, SmoothFactor);

            //rotateVec 是Y方向的旋转中心轴
            rotateVec = _cameraOffset;
            rotateVec.y = 0;
            rotateVec = Quaternion.AngleAxis(-90, Vector3.up) * rotateVec;

            //function Vector3.Slerp: Spherically interpolates between two vectors.
            //鼠标y轴上进行平滑
            newOffset = Quaternion.AngleAxis(-Input.GetAxis("Mouse Y") * 10.0f, rotateVec) * _cameraOffset;
            _cameraOffset = Vector3.Slerp(_cameraOffset, newOffset, SmoothFactor);

            transform.position = PlayerTransform.position + _cameraOffset;

            transform.LookAt(PlayerTransform);


        }

    }

    void Update()
    {
        Debug.Log("Update time :" + Time.deltaTime);
    }

    void FixedUpdate()
    {
        Debug.Log("FixedUpdate time :" + Time.deltaTime);
    }
}

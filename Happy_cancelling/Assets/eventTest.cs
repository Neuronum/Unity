using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class eventTest : MonoBehaviour
{
    public GameObject fruit_script;
    //测试带参数的EVENT唤醒, 参见 https://docs.unity3d.com/ScriptReference/Events.UnityEvent_1.html
    [System.Serializable]
    public class MyEvent : UnityEvent<GameObject, int> //使用int型的唤醒
    {

    }


    public MyEvent testEvt;
    // Start is called before the first frame update
    void Start()
    {
        fruit_script = GameObject.Find("Fruits");
        testEvt = new MyEvent();
        testEvt.AddListener(test);

        //带参数的事件唤醒方式
        testEvt.Invoke(fruit_script, 2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void test(GameObject fruit_script, int i)
    {
        Debug.Log("In eventTest script, i = " + i);
        Debug.Log("gameobject name: " + fruit_script.name);
    }
}

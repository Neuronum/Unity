using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;


public class RecreateFruit : MonoBehaviour
{
    private Initialize_Fruits fruit_script;
    private int[][] fruit_types;
    private string[][] fruit_names;
    private Vector3[][] fruit_pos;

    private int ROWS = Cancellable.ROWS;
    private int COLS = Cancellable.COLS;

    //创建事件用以调用
    public UnityEvent recreateFruitEvent;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        fruit_script = GameObject.Find("Fruits").GetComponent<Initialize_Fruits>();
        fruit_types = fruit_script.FruitTypes;
        fruit_pos = fruit_script.PicPositions;
        fruit_names = fruit_script.FruitNames;

        if(recreateFruitEvent == null)
        {
            recreateFruitEvent = new UnityEvent();
        }
        //recreateFruitEvent.AddListener(recreateFruits);
    }

    public void recreateFruits()
    {
        Debug.Log("recreateFruits was called!");
        GameObject currObject;
        int currType;
        for(int i =0; i < ROWS; i++)
        {
            for (int j = 0; j < COLS; j++) 
            {
                //如果类型为空, 就创建水果
                if (fruit_types[i][j] == -1)
                {
                    //随机创建水果种类
                    currType = Random.Range(0, 3);
                    //更新类型数组
                    fruit_types[i][j] = currType;
                    //获取图片路径
                    string PicPath = fruit_script.TypeToPath(currType);
                    currObject = new GameObject();
                    currObject.name = ObjectNames.GetUniqueName(fruit_script.NameList,"Fruit"); //生成不重名的水果名称
                    
                    currObject.transform.position = fruit_pos[i][j];
                    //存储当前水果名称到名称数组
                    fruit_names[i][j] = currObject.name;
                    //更新一维名称数组, 以方便取名
                    fruit_script.refreshNameList();
                    //添加Sprite组件
                    SpriteRenderer render = currObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
                    Texture2D tex = Resources.Load(PicPath) as Texture2D;
                    Sprite spr = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    render.sprite = spr;
                    currObject.transform.parent = this.gameObject.transform; // add as a child of fruits

                    //Add script SelectFruit to each fruit
                    currObject.AddComponent<SelectFruit>();
                    SelectFruit currentScript = currObject.GetComponent<SelectFruit>();
                    currentScript.row = i;
                    currentScript.col = j;

                    //Add collider to each fruit for mouseDown detecting
                    currObject.AddComponent<BoxCollider2D>();
                    //Add Animation Component for exchange animation
                    currObject.AddComponent<Animation>();

                }
            }
        }
        Debug.Log("recreation finished!");
    }
}

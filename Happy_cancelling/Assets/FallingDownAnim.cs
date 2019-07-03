using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FallingDownAnim : MonoBehaviour
{
    //当水果被标记为要移动时, 其类型在FruitTypes数组中被标记为-1.

    //记录要移动水果位置
    private Cancellable.PositionStruct[] positions = null;
    //记录水果移动的目标位置
    private Vector3[] destinations = null;
    //记录要移动水果的数目
    private int LEN = 0;
    //记录自下而上第一个空位的行和列位置
    private Cancellable.PositionStruct vacPos;

    private Initialize_Fruits fruit_script;
    private int[][] fruit_types;
    private string[][] fruit_names;
    private Vector3[][] fruit_pos;

    public UnityEvent fallingProcessEvent;
    

    public int[] tst = { 1,2,3 };
    // Start is called before the first frame update
    IEnumerator Start() //注意start返回类型为IEnumerator
    {
        //wait for 0.5 seconds, then continue. To guarantee that arrays are all initialized!
        yield return new WaitForSeconds(0.5f);

        fruit_script = GameObject.Find("Fruits").GetComponent<Initialize_Fruits>();
        fruit_types = fruit_script.FruitTypes;
        fruit_pos = fruit_script.PicPositions;
        fruit_names = fruit_script.FruitNames;
        reverse(ref tst);
        Debug.Log("tst is :"+ tst[0]+ " " + tst[1]);

        if(fallingProcessEvent == null)
        {
            fallingProcessEvent = new UnityEvent();
        }
        fallingProcessEvent.AddListener(animateFalling);
        fallingProcessEvent.AddListener(refreshParam);
       
    } 

    //创建下落动画
    public void animateFalling()
    {
        //逐列处理
        for(int m=0;m<Cancellable.COLS; m++)
        {
            //更新LEN, positions和destinations
            prepareMove(m, Cancellable.ROWS);

            //创建下落动画
            //若有可下落的水果, 则创建下落动画
            if (LEN > 0)
            {
                GameObject currFruit = null;
                int row;
                int col;
                //当前水果的目标位置
                Vector3 currDest;

                for (int i = 0; i < LEN; i++)
                {
                    row = positions[i].i;
                    col = positions[i].j;
                    currFruit = GameObject.Find(fruit_script.FruitNames[row][col]);
                    currDest = destinations[i];
                    //创建下落动画
                    fruit_script.create_animation(currFruit, currDest);
                }
            }

            //更新参数
            refreshParam();


            //重置LEN, positions和destinations
            positions = null;
            destinations = null;
            LEN = 0;
        }       
        
    }

    //更新fruit_types, fruit_names, 以及fruit row & col
    public void refreshParam()
    {
        if (LEN > 0)
        {
            Cancellable.PositionStruct currPos;
            //destPos记录目标位置
            Cancellable.PositionStruct destPos;
            destPos = vacPos;

            GameObject currFruit;
            SelectFruit currScript;

            for (int i = 0; i < LEN; i++)
            {
                //更新下落后的类型数组fruit_script.FruitTypes;
                //更新当前位置和目标位置
                currPos = positions[i];
                destPos.i = vacPos.i - i;
                //目标位置变为当前位置种类
                fruit_types[destPos.i][destPos.j] = fruit_types[currPos.i][currPos.j];
                //当前位置种类设为-1
                fruit_types[currPos.i][currPos.j] = -1;

                //更新下落后的名称数组
                fruit_names[destPos.i][destPos.j] = fruit_names[currPos.i][currPos.j];
                //把下落前的位置名称记为Null
                fruit_names[currPos.i][currPos.j] = "Null";

                //更新下落后的row和col, 本句需在更新名称数组之后
                Debug.Log("fruit to find: " + fruit_names[destPos.i][destPos.j]);
                currFruit = GameObject.Find(fruit_names[destPos.i][destPos.j]);
                currScript = currFruit.GetComponent<SelectFruit>();
                currScript.row = destPos.i;
                currScript.col = destPos.j;
            }
        }
    }

   

    //反转数组
    public void reverse(ref int[] a)
    {
        a = new int[3];
        a[0] = 9;
        a[1] = 8;
        a[2] = 7;
    }

    //反转数组positions, 其长度为len
    public void reverse(ref Cancellable.PositionStruct[] positions, int len)
    {
        //记录颠倒顺序后的数组
        Cancellable.PositionStruct[] tmp = new Cancellable.PositionStruct[len];
        for(int i = 0; i < len; i++)
        {
            //倒向赋值
            tmp[i] = positions[len - 1 - i];
        }
        for(int i =0; i<len; i++)
        {
            //把颠倒顺序后的数组重新赋予原数组
            positions[i] = tmp[i];
        }
    }

    //生成需要移动的水果数组positions以及移动目标位置的数组destinations
    public void prepareMove(int colIndex /*在处理的列数索引, 从0算起, the column to be determined to move*/, int row /*类型数组的行数*/)
    {
        //记录当前分析水果的位置信息
        Cancellable.PositionStruct fruitPos = new Cancellable.PositionStruct();

        //记录要移动的水果们
        Cancellable.PositionStruct[] fruitsToMove = new Cancellable.PositionStruct[row];
        //记录上面数组的有效长度
        int tmpLen = 0;

        //在当前列中, 从下到上查找第一个空位
        for (int vacancy = row-1; vacancy >= 0; vacancy--)
        {
            //用到全局变量fruit_types
            int currType = fruit_types[vacancy][colIndex];
            //当currType为-1, 即此位为空时, 存储当前空位
            if(currType == -1)
            {
                //存储该空位位置, 供refreshParam调用
                vacPos = new Cancellable.PositionStruct();
                vacPos.i = vacancy;
                vacPos.j = colIndex;
                //从上到下查找最下方要移动的水果
                for(int fruitIndex = 0; fruitIndex <= vacancy; fruitIndex++)
                {
                    //判断当前水果的类型
                    currType = fruit_types[fruitIndex][colIndex];

                    if(currType != -1)
                    {
                        //存储当前分析水果的位置到数组
                        fruitPos.i = fruitIndex;
                        fruitPos.j = colIndex;
                        fruitsToMove[tmpLen] = fruitPos;

                        tmpLen++;
                    }
                    
                }

                //如果最后记录的水果所在行数大于第一个空位的行数, 表明需要进行移动
                if(tmpLen>0 && fruitsToMove[tmpLen-1].i < vacancy)
                {
                    //!!!!反转要移动的水果位置数组!!!! 反转后水果位置数组顺序也是从下到上
                    reverse(ref fruitsToMove, tmpLen);

                    //输出要移动的水果位置数组
                    positions = fruitsToMove;

                    destinations = new Vector3[tmpLen];
                    //输出要移动的目标位置
                    for(int m = 0; m < tmpLen; m++)
                    {
                        destinations[m] = fruit_pos[vacancy - m][colIndex]; //从第一个空位往上, 都将成为移动的目标位置
                    }
                }

                //赋予移动水果的个数
                LEN = tmpLen;
                //从下而上找到第一个空位即可停止程序
                break;
            }
            
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Cancellable : MonoBehaviour
{
    public Initialize_Fruits fruit_script;
    public UnityEvent cancelProcessEvent;

    //C#中, struct是值类型而非引用类型, 参见https://docs.microsoft.com/en-us/dotnet/csharp/structs
    public struct PositionStruct
    {
        public int i;
        public int j;
    };
    //数组每一行代表要消除的一组
    public PositionStruct[][] posToCancel = null;
    //要消除的组的个数
    private int groupsToCancle;
    //存储每一组要消除的个数
    public int[] numToCancel;
    public int[][] fruit_types; //!!!! fruit_types 在 Initialize_Fruits start()代码中初始化 !!!!!

    //记录水果的名字
    private string[][] fruit_names;

    //水果数组的行数和列数
    public static int ROWS = 3;
    public static int COLS = 3;

    private void Start()
    {
        //get fruit's script
        fruit_script = GameObject.Find("Fruits").GetComponent<Initialize_Fruits>();

        StartCoroutine(lateStart(0.4f));
        //!!!! fruit_types 在 Initialize_Fruits start()代码中初始化 !!!!!

        //消除处理事件
        if (cancelProcessEvent == null)
            cancelProcessEvent = new UnityEvent();

        cancelProcessEvent.AddListener(cancleBegin);
        cancelProcessEvent.AddListener(markUp); //标记待消除元素
        cancelProcessEvent.AddListener(cancelFruit); //删除水果

    }

    IEnumerator lateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        fruit_names = fruit_script.FruitNames;
    }

    //标记待消除元素类型为-1
    public void markUp()
    {
        int row;
        int col;
        if (groupsToCancle != 0)
        {
            for(int m = 0; m < groupsToCancle; m++)
            {
                for(int n = 0; n < numToCancel[m]; n++)
                {
                    row = posToCancel[m][n].i;
                    col = posToCancel[m][n].j;
                    fruit_types[row][col] = -1;
                }
            }
        }
    }

    //删除水果
    public void cancelFruit()
    {
        int row;
        int col;
        GameObject currFruit;
        if (groupsToCancle != 0)
        {
            for (int m = 0; m < groupsToCancle; m++)
            {
                for (int n = 0; n < numToCancel[m]; n++)
                {
                    row = posToCancel[m][n].i;
                    col = posToCancel[m][n].j;
                    currFruit = GameObject.Find(fruit_names[row][col]);
                    Destroy(currFruit);
                }
            }
        }
    }

    //横向和纵向处理
    public void cancleBegin()
    {
        //数组每一行代表要消除的一组
        posToCancel = new PositionStruct[20][];
        //要消除的组的个数
        groupsToCancle = 0;
        //存储每一组要消除的个数
        numToCancel = new int[20];

        PositionStruct startHori;
        startHori.i = 0;
        startHori.j = 0;

        PositionStruct startVert = startHori;
        //横向处理
        for(int i = 0; i < ROWS; i++)
        {
            cancelProcessHori(startHori, fruit_types, ROWS, COLS, posToCancel, numToCancel, ref groupsToCancle);
            startHori.i++;
        }
        //纵向处理
        for (int j = 0; j < COLS; j++)
        {
            cancelProcessVert(startVert, fruit_types, ROWS, COLS, posToCancel, numToCancel, ref groupsToCancle);
            startVert.j++;
        }
        Debug.Log("groups to be cancled: " + groupsToCancle);
    }

    //横向消除处理
    public void cancelProcessHori(PositionStruct start, int[][] fruit_types, int rows, int cols, PositionStruct[][] posToCancel, int[] numToCancel, ref int groupsToCancle)
    {
        PositionStruct init = start;
        //注意引用传参 ref groupsToCancle
        PositionStruct next = cancelGroupHori(init, fruit_types, rows, cols, posToCancel, numToCancel, ref groupsToCancle);
        while( !posEqual(init, next) ) //当init和next不相等时, 继续进行横向比较, 当到达边界时, 二者必相等
        {
            init = next;
            next = cancelGroupHori(init, fruit_types, rows, cols, posToCancel, numToCancel, ref groupsToCancle);
        }        
    }
    
    //判断两个PositionStruct A和B是否相等
    public bool posEqual(PositionStruct A, PositionStruct B)
    {
        int Ai = A.i;
        int Aj = A.j;
        int Bi = B.i;
        int Bj = B.j;

        if (Ai == Bi && Aj == Bj)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Horizontal comparison, return the final compare position, 本函数会更新posToCancel, numToCancel数组, 以及groupsToCancle的值
    public PositionStruct cancelGroupHori(PositionStruct start, int[][] fruit_types, int rows /*水果行数*/, int cols /*水果列数*/, PositionStruct[][] posToCancel,  int[] numToCancel,
        ref int grpToCancle) //按引用传递grouptsToCancle参数, 参见https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/passing-reference-type-parameters
    {
        //是否进行下一轮比较
        bool flag = true;

        int sameNum = 0;

        PositionStruct currpos = start;
        if (currpos.j >= cols - 1)  //如果比较已经到达边界, 就停止比较
        {
            flag = false;
            return currpos;
        }
        //next (horizontal) position of fruit
        PositionStruct nextpos = currpos; //struct is value type, thus the values of nextpos are copies rather than referenced!!
        nextpos.j++;

        int currType = fruit_types[currpos.i][currpos.j];
        //move horizontally
        int nextType = fruit_types[nextpos.i][nextpos.j];
        PositionStruct[] tmppos = new PositionStruct[20]; //用来暂存要消除的水果位置
        tmppos[0] = currpos; 

        while (flag)
        {
            //循环比较当前位置的水果和右侧水果是否相同, 相同就继续向右比较, 注意值为-1时表示空
            if (currType!=-1 && currType == nextType)
            {
                Debug.Log("Same fruit found, position: i = " + nextpos.i + "; j = " + nextpos.j);
                sameNum++;
                tmppos[sameNum] = nextpos; //把下一个水果的位置放入待消除数组
                if (nextpos.j < cols - 1) //判断是否到达边界
                {
                    //向右移动一格
                    nextpos.j += 1;                    
                    nextType = fruit_types[nextpos.i][nextpos.j];

                }
                else //如果比较到达边界, 就停止比较
                {
                    flag = false;
                }
            }
            else //如果下一个元素不同, 就停止下一轮比较
            {
                flag = false;
            }
        }
        //如果超过2个水果种类与第1个相同, 就把这些水果的位置存入posToCancel数组
        if (sameNum >= 2)
        {
            Debug.Log("sameNum: " + sameNum);
            posToCancel[grpToCancle] = tmppos;
            numToCancel[grpToCancle] = sameNum + 1; //记录该组的长度
            grpToCancle += 1; //更新要消除的组数           
        }
        return nextpos;        
    }

    //纵向消除处理
    public void cancelProcessVert(PositionStruct start, int[][] fruit_types, int rows, int cols, PositionStruct[][] posToCancel, int[] numToCancel, ref int groupsToCancle)
    {
        PositionStruct init = start;
        //注意引用传参 ref groupsToCancle
        PositionStruct next = cancelGroupVert(init, fruit_types, rows, cols, posToCancel, numToCancel, ref groupsToCancle);
        while (!posEqual(init, next)) //当init和next不相等时, 继续进行横向比较, 当到达边界时, 二者必相等
        {
            init = next;
            next = cancelGroupVert(init, fruit_types, rows, cols, posToCancel, numToCancel, ref groupsToCancle);
        }
    }
    //纵向处理
    public PositionStruct cancelGroupVert(PositionStruct start, int[][] fruit_types, int rows /*水果行数*/, int cols /*水果列数*/, PositionStruct[][] posToCancel, int[] numToCancel,
        ref int grpToCancle) //按引用传递grouptsToCancle参数, 参见https://docs.microsoft.com/zh-cn/dotnet/csharp/programming-guide/classes-and-structs/passing-reference-type-parameters
    {
        //是否进行下一轮比较
        bool flag = true;

        int sameNum = 0;

        PositionStruct currpos = start;
        if (currpos.i >= rows - 1)  //如果比较已经到达边界, 就停止比较
        {
            flag = false;
            return currpos;
        }
        //next (vertical) position of fruit
        PositionStruct nextpos = currpos; //struct is value type, thus the values of nextpos are copies rather than referenced!!
        nextpos.i++;

        int currType = fruit_types[currpos.i][currpos.j];
        //move vertically
        int nextType = fruit_types[nextpos.i][nextpos.j];
        PositionStruct[] tmppos = new PositionStruct[20]; //用来暂存要消除的水果位置
        tmppos[0] = currpos;

        while (flag)
        {
            //循环比较当前位置的水果和右侧水果是否相同, 相同就继续向右比较, 注意值为-1时表示空
            if (currType != -1 && currType == nextType)
            {
                Debug.Log("Vertically same fruit found, position: i = " + nextpos.i + "; j = " + nextpos.j);
                sameNum++;
                tmppos[sameNum] = nextpos; //把下一个水果的位置放入待消除数组
                if (nextpos.i < rows - 1) //判断是否到达边界
                {
                    //向下移动一格
                    nextpos.i += 1;
                    nextType = fruit_types[nextpos.i][nextpos.j];

                }
                else //如果比较到达边界, 就停止比较
                {
                    flag = false;
                }
            }
            else //如果下一个元素不同, 就停止下一轮比较
            {
                flag = false;
            }
        }
        //如果超过2个水果种类与第1个相同, 就把这些水果的位置存入posToCancel数组
        if (sameNum >= 2)
        {
            Debug.Log("sameNum: " + sameNum);
            posToCancel[grpToCancle] = tmppos;
            numToCancel[grpToCancle] = sameNum + 1; //记录该组的长度
            grpToCancle += 1; //更新要消除的组数           
        }
        return nextpos;
    }
}

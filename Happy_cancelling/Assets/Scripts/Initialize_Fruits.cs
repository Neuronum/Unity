using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initialize_Fruits : MonoBehaviour
{
    //save picture positions. In this case, its dimension is 3*3
    public Vector3[][] PicPositions;

    //use int to save fruit types. In this case, its dimension is 3*3
    public int[][] FruitTypes;

    //存储水果的名称数组
    public string[][] FruitNames;
    public string[] NameList = new string[9];

    //Exchange pic_A and pic_B
    public GameObject pic_A;
    public GameObject pic_B;
    public SelectFruit pic_A_script;
    public SelectFruit pic_B_script;

    //anim for pic exchange animation
    private Animation anim;
    private bool isAnimating = false;

    public float anim_time = 1.0f; //time last for anim

    public float timeToCancelFruits = 0;
    public bool isToCancel = false;

    //消除化代码引用
    public Cancellable cancelProcess;
    //下落动画脚本
    public FallingDownAnim fallingProcess;
    //重新创建水果脚本
    public RecreateFruit recreateProcess;

    // Start is called before the first frame update
    void Start()
    {
        init_Positions();
        init_PicsTypes(); //FruitTypes被初始化

        //for (int i = 0; i <= 2; i++)
        //    for (int j = 0; j <= 2; j++)
        //        Debug.Log(PicPositions[i][j]);

        pic_A = null;
        pic_B = null;
        pic_A_script = pic_B_script = null;

        //初始化消除代码
        cancelProcess = GetComponent<Cancellable>();
        cancelProcess.fruit_types = FruitTypes;

        //初始化下落动画脚本
        fallingProcess = GetComponent<FallingDownAnim>();

        //初始化重现创建水果的脚本
        recreateProcess = GetComponent<RecreateFruit>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //if user has selected two pics, exchange them and reset pic_A & pic_B
        if (isAnimating == false)
        {
            if (pic_A != null && pic_B != null)
            {
                isAnimating = true;
                pic_A_script = pic_A.GetComponent<SelectFruit>();
                pic_B_script = pic_B.GetComponent<SelectFruit>();

                //set Animating status
                pic_A_script.isAnimating = true;
                pic_B_script.isAnimating = true;

                //exchange pic_A and pic_B's positions
                exchange(pic_A, pic_B);
                //exchange FruitTypes
                exchangeType(pic_A_script, pic_B_script, FruitTypes);
                //exchange FruitNames
                exchangeName(pic_A_script, pic_B_script, FruitNames);
                //exchangeFruitName(pic_A, pic_B);
                //exchange row and column properties
                exchangeRowCol(pic_A_script, pic_B_script);

            }
        }
        else //check if pic_A and pic_B's animations are over
        {
            Debug.Log("A Type: " + FruitTypes[pic_A_script.row][pic_A_script.col] );
            isAnimating = (pic_A_script.isAnimating || pic_B_script.isAnimating);
            if (!isAnimating) //上一帧有动画, 而这一帧没有动画了, 表示A, B两段动画已经播放完毕了, 那么重置pic_A和pic_B的值
            {
                pic_A = pic_B = null;

                //进行消除处理
                //cancelProcess.cancelProcessEvent.Invoke();
                cancelProcess.cancleBegin();
                cancelProcess.markUp();
                cancelProcess.cancelFruit();

                //创建下落动画
                //fallingProcess.fallingProcessEvent.Invoke();
                fallingProcess.animateFalling(); //触发isToCancel = true;

                for(int i = 0; i < Cancellable.ROWS; i++)
                {
                    for(int j = 0; j < Cancellable.COLS; j++)
                    {
                        if (FruitTypes[i][j] == -1)
                        {
                            isToCancel = true;
                            break;
                        }
                    }
                }

                //更新一维名称数组, 以方便取名
                //refreshNameList();
                
            }
        }

        //如果要消除, 则积累时间开始
        if (isToCancel)
        {
            timeToCancelFruits += Time.deltaTime;
            Debug.Log("timeToCancel is " + timeToCancelFruits);
            if (timeToCancelFruits > 2.0f)
            {
                recreateProcess.recreateFruits();
                timeToCancelFruits = 0;
                isToCancel = false;
            }
        }

    }

    public void refreshNameList()
    {
        for(int i = 0; i < Cancellable.ROWS; i++)
        {
            for(int j=0; j < Cancellable.COLS; j++)
            {
                NameList[(i * Cancellable.ROWS) + j] = FruitNames[i][j];
            }
        }
    }

    //generate a position array, store all fruit positions
    void init_Positions()
    {
        Vector3 init_pos = new Vector3(-3.0f,3.0f,0f);
        PicPositions = new Vector3[3][];
        for (int i = 0; i <= 2; i++)
        {
            Vector3[] tmpVec = new Vector3[3];
            for (int j = 0; j <= 2; j++)
            {
                tmpVec[j] = init_pos + new Vector3(j * 3.0f, -i * 3.0f, 0); //注意i和j的位置
                //PicPositions[i][j] = init_pos + new Vector3(j * 3.0f, -i * 3.0f, 0);
            }
            PicPositions[i] = tmpVec; //动态数组需要这么麻烦的初始化!?
        }
    }

    //use currentType to decide which picture to choose
    public string TypeToPath(int currentType)
    {
        string path;
        switch (currentType)
        {
            case 0:
                path = "Fruits-Intel/Apple";
                break;
            case 1:
                path = "Fruits-Intel/Banana";
                break;
            default :
                path = "Fruits-Intel/Strawberry";
                break;
        }
        return path;
    }

    //初始化水果种类数组和名称数组
    void init_PicsTypes()
    {
        FruitTypes = new int[3][];
        FruitNames = new string[3][];

        for (int i = 0; i <= 2; i++)
        {
            int[] tmpTypes = new int[3];
            string[] tmpNames = new string[3];

            for (int j = 0; j <= 2; j++)
            {
                //generate a random type of 3 types
                int currentType = Random.Range(0, 3);
                //assign current type to FruitTypes array
                tmpTypes[j] = currentType;

                string PicPath = TypeToPath(currentType);
                //create sprite at current position
                GameObject currentObject = new GameObject("fruit_" + i + j);

                //更新水果名称数组
                tmpNames[j] = currentObject.name;

                //set position for current picture
                currentObject.transform.position = PicPositions[i][j];
                SpriteRenderer render = currentObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
                //Load sprite from Fruits Folder
                Texture2D tex = Resources.Load(PicPath) as Texture2D; //PicPath需要在Resources文件夹下!!!!!!!!!!!
                Sprite spr = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f)); //new Vector2(0.5f, 0.5f)表示重心在正中央
                //render.sprite = Resources.Load(PicPath) as Sprite;  // 此句错误, 图片无法导入为Sprite, 故返回null
                render.sprite = spr;
                currentObject.transform.parent = this.gameObject.transform; //add as a child of fruits

                //Add script SelectFruit to each fruit
                currentObject.AddComponent<SelectFruit>();
                SelectFruit currentScript = currentObject.GetComponent<SelectFruit>();
                currentScript.row = i;
                currentScript.col = j;

                //Add collider to each fruit for mouseDown detecting
                currentObject.AddComponent<BoxCollider2D>();
                //Add Animation Component for exchange animation
                currentObject.AddComponent<Animation>();
            }
            FruitTypes[i] = tmpTypes; //assign current row fruit types to FruitTypes array
            FruitNames[i] = tmpNames; //assign names to FruitNames array
        }
            
    }

    //exchange pic A and pic B's positions and play animation
    void exchange(GameObject A, GameObject B)
    {
        Vector3 localPosition_A = A.transform.localPosition;
        Vector3 localPosition_B = B.transform.localPosition;

        create_animation(pic_A, localPosition_B);
        create_animation(pic_B, localPosition_A);
        Debug.Log("pics exchanged");
    }

    public void create_animation(GameObject pic, Vector3 dest_position)  // dest_position means destination position
    {
        AnimationClip clip = new AnimationClip();
        clip.legacy = true;
        clip.wrapMode = WrapMode.Once;

        //for x
        Keyframe[] keys;
        keys = new Keyframe[2];
        keys[0] = new Keyframe(0f, pic.GetComponent<Transform>().localPosition.x);
        keys[1] = new Keyframe(anim_time, dest_position.x);
        AnimationCurve curve = new AnimationCurve(keys);
        clip.SetCurve("", typeof(Transform), "localPosition.x", curve);

        //for y
        keys[0] = new Keyframe(0f, pic.GetComponent<Transform>().localPosition.y);
        keys[1] = new Keyframe(anim_time, dest_position.y);
        curve = new AnimationCurve(keys);
        clip.SetCurve("", typeof(Transform), "localPosition.y", curve);

        //for z
        keys[0] = new Keyframe(0f, pic.GetComponent<Transform>().localPosition.z);
        keys[1] = new Keyframe(anim_time, dest_position.z);
        curve = new AnimationCurve(keys);
        clip.SetCurve("", typeof(Transform), "localPosition.z", curve);

        //for event frame, set pic_A and pic_B to null
        AnimationEvent anim_event = new AnimationEvent();
        anim_event.time = anim_time;
        anim_event.functionName = "reset_pic";
        clip.AddEvent(anim_event);

        //AssetDatabase.CreateAsset(clip, "Assets/AutoAnimation_" + pic.name + ".anim");
        anim = pic.GetComponent<Animation>();
        anim.AddClip(clip, "Anim" + pic.name);
        anim.Play("Anim" + pic.name);
    }

    void exchangeType(SelectFruit fruitA, SelectFruit fruitB, int[][] typeArray)
    {
        int rowA = fruitA.row;
        int colA = fruitA.col;
        int rowB = fruitB.row;
        int colB = fruitB.col;

        //store current types
        int typeA = typeArray[rowA][colA];
        int typeB = typeArray[rowB][colB];

        //exchange types in typeArray
        typeArray[rowA][colA] = typeB;
        typeArray[rowB][colB] = typeA;

        
    }

    void exchangeName(SelectFruit fruitA, SelectFruit fruitB, string[][] nameArray)
    {
        int rowA = fruitA.row;
        int colA = fruitA.col;
        int rowB = fruitB.row;
        int colB = fruitB.col;

        //store current names
        string nameA = nameArray[rowA][colA];
        string nameB = nameArray[rowB][colB];

        //exchange names in nameArray
        nameArray[rowA][colA] = nameB;
        nameArray[rowB][colB] = nameA;


    }


    void exchangeRowCol(SelectFruit fruitA, SelectFruit fruitB)
    {
        int rowA = fruitA.row;
        int colA = fruitA.col;
        int rowB = fruitB.row;
        int colB = fruitB.col;

        //refresh row and col in fruitA and fruitB
        fruitA.row = rowB;
        fruitA.col = colB;
        fruitB.row = rowA;
        fruitB.col = colA;
    }
}

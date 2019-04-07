using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Pic_Exchange : MonoBehaviour
{
    private GameObject[] pic_children;
    //Exchange pic_A and pic_B
    private GameObject pic_A; 
    private GameObject pic_B;
    private bgRimShow pic_A_script;
    private bgRimShow pic_B_script;


    private Animation anim;
    private bool isAnimating = false; 

    public float anim_time = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        pic_children = GameObject.FindGameObjectsWithTag("pic_child");
        for(int i =0; i < pic_children.Length; i++)
        {
            Debug.Log(pic_children[i].name);
        }
        pic_A = null;
        pic_B = null;
    }

    // Update is called once per frame
    void Update()
    {
        //assign pic_A and pic_B values when the pic_child is selected by mouse
        assign_tmpPic();

        //if user has selected two pics, exchange them and reset pic_A & pic_B
        if(isAnimating == false)
        {
            if (pic_A != null && pic_B != null)
            {
                isAnimating = true;
                pic_A_script = pic_A.GetComponent<bgRimShow>();
                pic_B_script = pic_B.GetComponent<bgRimShow>();

                //set Animating status
                pic_A_script.isAnimating = true;
                pic_B_script.isAnimating = true;

                //exchange pic_A and pic_B's positions
                exchange(pic_A, pic_B);

            }
        }
        else //check if pic_A and pic_B's animations are over
        {
            isAnimating = (pic_A_script.isAnimating || pic_B_script.isAnimating);
            if(!isAnimating) //上一帧有动画, 而这一帧没有动画了, 表示A, B两段动画已经播放完毕了, 那么重置pic_A和pic_B的值
            {
                pic_A = pic_B = null;
            }
        }

            


    }

    //assign pic_A and pic_B values when the pic_child is selected by mouse
    void assign_tmpPic()
    {
        for (int i = 0; i < pic_children.Length; i++) //find if there are selected pics
        {
            bgRimShow child_script = pic_children[i].GetComponent<bgRimShow>();

            if (child_script.isBGActive)
            {
                //Debug.Log("Active pic found: " +  pic_children[i].name);
                if (pic_A == null)
                {
                    pic_A = pic_children[i];
                }
                else
                {
                    if(pic_A != pic_children[i])
                        pic_B = pic_children[i];

                }

            }
        }
    }

    void exchange(GameObject A, GameObject B)
    {
        Vector3 localPosition_A = A.transform.localPosition;
        Vector3 localPosition_B = B.transform.localPosition;

        create_animation(pic_A, localPosition_B);
        create_animation(pic_B, localPosition_A);
        Debug.Log("pics exchanged");
    }

    void create_animation(GameObject pic, Vector3 dest_position)  // dest_position means destination position
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

}

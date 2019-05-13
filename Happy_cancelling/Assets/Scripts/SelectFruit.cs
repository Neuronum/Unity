using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectFruit : MonoBehaviour
{
    public int row = 0; //current fruit's row index in FruitTypes array
    public int col = 0; //current fruit's column index in FruitTypes array
    public bool isSelected = false;
    public bool isAnimating = false;

    private Initialize_Fruits parentScript; 
    
    void Start()
    {
        parentScript = GameObject.Find("Fruits").GetComponent<Initialize_Fruits>(); //Get Fruits Script
    }

    private void OnMouseDown()
    {
        isSelected = true;
        Debug.Log("Mouse Click Triggered");

        //prepare for pic exchange
        if(parentScript.pic_A == null)
        {
            parentScript.pic_A = this.gameObject;
        }
        else
        {
            if (parentScript.pic_B == null)
                parentScript.pic_B = this.gameObject;
        }
    }

    public void reset_pic()
    {
        isSelected = false;
        Debug.Log("reset_pic triggered!");
        isAnimating = false;
    }
}

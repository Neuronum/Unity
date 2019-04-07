using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgRimShow : MonoBehaviour
{
    [SerializeField] private GameObject bg;
    public bool isBGActive = false;
    public bool isAnimating = false;
    // Start is called before the first frame update
    void Start()
    {
        bg.SetActive(false);
        isBGActive = false;
    }

    private void OnMouseDown()
    {
        bg.SetActive(true);
        isBGActive = true;
        Debug.Log("Mouse Click Triggered");
    }

    public void hideBG()
    {
        bg.SetActive(false);
        isBGActive = false;
    }

    public void reset_pic()
    {
        hideBG();
        Debug.Log("reset_pic triggered!");
        isAnimating = false;
    }
}

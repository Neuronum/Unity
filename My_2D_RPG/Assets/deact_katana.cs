using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deact_katana : MonoBehaviour
{
    public GameObject katana;
    public void deact_kata()
    {
        katana.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMother : MonoBehaviour
{
    public Transform sphere;

    [Range(0.01f, 1.0f)]
    public float SmoothFactor = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = sphere.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = sphere.position;
    }
}

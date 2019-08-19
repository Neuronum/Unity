using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackedForce : MonoBehaviour
{
    public Rigidbody2D enemy_rigidbody;
    public float upForce = 2000f;
    private void Awake()
    {
        enemy_rigidbody = GetComponent<Rigidbody2D>();
    }
    public void attackedBehavior()
    {
        Debug.Log("Being Attacked");
        enemy_rigidbody.AddForce(new Vector2(0f, upForce));
    }
}

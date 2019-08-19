using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SphereController : MonoBehaviour
{
    public Transform cameraRef;

    private Vector3 init_pos;
    private Rigidbody rb;
    public float speed;

    private Vector3 _cameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        init_pos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        float hInput = Input.GetAxis("Horizontal");
        float vInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(hInput, 0, vInput);

        _cameraOffset = cameraRef.position - transform.position;

        float angle = Vector3.SignedAngle(Vector3.forward, -_cameraOffset, Vector3.up); //相对相机位置施加力量
        movement = Quaternion.AngleAxis(angle, Vector3.up) * movement;

        rb.AddForce(movement * speed);

        if (transform.position.y < 30)
        {
            rb.velocity = new Vector3(0, 0, 0); //球越位则速度变为0
            transform.position = init_pos;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            other.gameObject.SetActive(false);
        }
    }
}

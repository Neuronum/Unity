using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MovementControl : MonoBehaviour
{
    [SerializeField] private Rigidbody2D m_RigidBody2D;
    [SerializeField] private float m_JumpForce = 600f;
    private float horizontalMove = 0f;
    private float groundedAccTime = 0f;

    public LayerMask groundLayer;
    public bool InAir = false;
    public Transform m_GroundCheck;
    public float groundRadius = 0.02f;
    public CircleCollider2D circleCollider;
    public BoxCollider2D boxCollider;
    public CapsuleCollider2D capsuleCollider;

    public GameObject katana;
    private Animator anim_katana;
    private bool isAttacking = false;

    public float runSpeed = 40f;
    private float face = 1f;
    private Quaternion initRotation;
    private bool isSliding = false;
    Animator anim;

    //check whether to change scene
    [Header("Events")]
    [Space]
    public UnityEvent sceneChangeEvent;
    public LayerMask sceneChangeMask;

    //check whether to damage enemies
    [Header("Events")]
    [Space]
    public UnityEvent enemyAttackEvent;
    public float enemyRadius = 2f;
    public LayerMask enemyMask;
    public Transform enemyChk;


    // Start is called before the first frame update
    private void Awake()
    {
        initRotation = transform.localRotation;

        m_RigidBody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        anim_katana = katana.GetComponent<Animator>();

        if (sceneChangeEvent == null)
            sceneChangeEvent = new UnityEvent();
        if (enemyAttackEvent == null)
            enemyAttackEvent = new UnityEvent();
    }

    // Update is called once per frame


    private void Update()
    {
        //check if in the air
        InAir = Is_InAir();
        if (InAir) //determine which animation to be played
        {
            anim.SetBool("IsInAir", true);
            anim.SetFloat("AirVelocityY", m_RigidBody2D.velocity.y);
            groundedAccTime = 0f;
        }
        else
        {
            anim.SetBool("IsInAir", false);
            anim.SetFloat("AirVelocityY", 0f);
            if (groundedAccTime < 0.2f)
                groundedAccTime += Time.deltaTime;
        }

        //Control Player Movement
        horizontalMove = Input.GetAxis("Horizontal") * runSpeed;

        //Determine if the character changes position
        if (horizontalMove * face < 0f)
        {
            Flip(); //change picture position
        }

        Move(horizontalMove * Time.fixedDeltaTime * 10f );

        anim.SetFloat("Speed", Mathf.Abs(horizontalMove)); // control the animator

        //record current face direction
        if (horizontalMove !=0f )
            face = horizontalMove;

        //control to jump
        if (Input.GetButtonDown("Jump"))
        {
            m_RigidBody2D.velocity = new Vector2(m_RigidBody2D.velocity.x,0);
            m_RigidBody2D.AddForce(new Vector2(0f, m_JumpForce));
        }

        //control to slide
        anim.SetFloat("GroundVelocityY", m_RigidBody2D.velocity.y);
        anim.SetFloat("GroundedTime", groundedAccTime);

        //when sliding, rigidbody effects rotation
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("slide"))
        {
            if(isSliding == false) //if not sliding before, turn capsuleCollider on and other colliders off
            {
                isSliding = true;
                m_RigidBody2D.freezeRotation = false;
                boxCollider.enabled = false;
                circleCollider.enabled = false;
                capsuleCollider.enabled = true;
            }
            
        }
        else
        {
            if(isSliding) //if is sliding before, turn capsuleCollider off and other colliders on 
            {
                isSliding = false;
                m_RigidBody2D.freezeRotation = true;
                boxCollider.enabled = true;
                circleCollider.enabled = true;
                capsuleCollider.enabled = false;
                transform.rotation = initRotation;
            }
            
        }

        //control attacks
        if (isAttacking == true)
        {
            anim_katana.SetBool("attack", false);
            isAttacking = false;
        }
        
        if (Input.GetButtonDown("attack"))
        {
            katana.SetActive(true);
            anim_katana.SetBool("attack", true);
            isAttacking = true;
        }

        //whether to change scene
        if (Physics2D.OverlapCircle(m_GroundCheck.position, groundRadius, sceneChangeMask))
        {
            sceneChangeEvent.Invoke();
        }

        //whether damage to enemies
        if (Physics2D.OverlapCircle(enemyChk.position, enemyRadius, enemyMask))
        {
            if (katana.activeSelf)
                enemyAttackEvent.Invoke();
        }
                      

    }

    public void Move(float speed)
    {
        Vector3 targetVelocity = new Vector2(speed, m_RigidBody2D.velocity.y); //control y direction speed
        m_RigidBody2D.velocity = targetVelocity;
    }

    public void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    public bool Is_InAir( )
    {
        bool isInAir;
        if (Physics2D.OverlapCircle(m_GroundCheck.position, groundRadius, groundLayer))
        {
            isInAir = false;
        }
        else
        {
            isInAir = true;
        }

        return isInAir;
    }

}

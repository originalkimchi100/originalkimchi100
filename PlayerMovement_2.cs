 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerMovement_2 : MonoBehaviour
{
    public float movePower = 8f;
    public float jumpPower = 12f;
    public float moveSpeed = 8f;
    public Rigidbody2D rigid;
    Animator animator;
    public Vector3 movement;
    
    public bool isJumping = true;
    bool isPlayerDead = false;
    public bool isSwinging;
    private CapsuleCollider2D capsule;
    public ContactFilter2D ContactFilter;
    
    [SerializeField] private LayerMask jumpableground;
    [SerializeField] public stat health;
    [SerializeField] public stat mana;
    [SerializeField] private float debuffDuration;
    [SerializeField] private int damage;
    [SerializeField] private TrailRenderer tr;
    private float initHealth;
    private int initMana;
    private float Starpoint = 0;
    //private float Coin = 0;
    public bool starcoin;

    public bool cutsceneon;
    private Vector3 respawnPoint;
    public bool attacked;
    public bool leftsee;
    public float acceleration;
    private List<Debuff> debuffs = new List<Debuff>();
    private List<Debuff> debuffsToRemove = new List<Debuff>();
    private List<Debuff> newdebuffs = new List<Debuff>();
    public bool debuffon;
    public bool stoproping;
    GameObject controller;
    private bool jumpable;
    private int jumpcount;
    private float jumpTimer;
    public float jumpSpeed = 15f;
    public float jumpDelay = 0.25f;
    public float linearDrag = 0.15f;
    public float gravity = 0.15f;
    public float fallMultiplier = 0.15f;
    public Vector2 direction;
    public bool gethurtbool;
    public GameObject timeline;
    private bool canDash = true;
    public bool isDashing;
    private float dashingPower = 80f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;
    public GameObject particleondash;
    public Transform dashtransform;
    skillon killon;

    public float checkRadius;
    bool isTouchingFront;
    public Transform frontCheck;
    bool wallsliding;
    public float wallSlidingspeed;
    bool walljumping;
    float fallmult;
    
    void Awake()
    {
 
    }
    void Start()
    {
        killon= gameObject.GetComponent<skillon>();
        controller = GameObject.FindWithTag("GameController");
        initHealth = controller.GetComponent<statcontroller>().health;
        initMana = controller.GetComponent<statcontroller>().mana;
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
        capsule = GetComponent<CapsuleCollider2D>();
        health.Initialize(initHealth, initHealth);
        mana.Initialize(initMana, initMana);
        StartCoroutine(CheckPlayerDeath());

        respawnPoint = transform.position;
 
    }

    // Update is called once per frame
    void Update()
    {

  
        if (isDashing)//위에있어야함
        {
            return;
        }

        if (timeline.GetComponent<timelinescripting>().timelineon)
        {
            //animator.SetBool("ismoving", false);
        }

        GetInput();
        IsOnGround();
        IsOnGround_2();
        if(IsOnGround() || IsOnGround_2())
        {
            jumpcount = 1;
        }
        if (Input.GetButtonDown("Jump") )
        {
           
            if ( IsOnGround() || IsOnGround_2())
            {


                isJumping = true;
              
            }
            else
            {
                /*
                if(jumpcount > 0)
                {
                    jumpcount--;
                    isJumping = true;
                    animator.SetTrigger("doublejump");
                    Debug.Log("doublejump");
                }
                */
     
            }    

        }

        if (!IsOnGround() &&  !IsOnGround_2() && jumpcount == 1)
        {
            if (Input.GetButtonDown("Jump"))
            {
                animator.SetTrigger("doublejump");
                isJumping = true;

                jumpcount--;
                    
            }
        }
        isTouchingFront = Physics2D.OverlapCircle(frontCheck.position, checkRadius, jumpableground);
        if(isTouchingFront == true && IsOnGround() == false && IsOnGround_2() == false)
        {
            wallsliding = true;

        }
        else
        {
            wallsliding = false;
        }
        if (wallsliding)
        {
            rigid.velocity = new Vector2(rigid.velocity.x, Mathf.Clamp(rigid.velocity.y, -wallSlidingspeed,float.MaxValue));
        }
        if(Input.GetButtonDown("Jump") && wallsliding == true)
        {
            walljumping = true;
           
        }

        if (walljumping == true)
        {
            isJumping = false;
            rigid.velocity = Vector2.zero;
            Vector2 jumpVelocity = new Vector2(0, jumpPower *1.7F);
            rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);
            walljumping = false;
        }

        if (isPlayerDead) return;
        HandleDebuffs();

        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    void FixedUpdate()
    {

        if (isDashing)
        {
            return;
        }
        if (!isPlayerDead && !timeline.GetComponent<timelinescripting>().timelineon)
        { 
            Move();
            Jump();

        }
        else
        {




        }
        modifyPhysics();
    }

    void SetWallJumpingToFalse()
    {
        walljumping = false;
    }
    void Move()
    {
        if (killon.chargeon == false)
        {


            Vector3 moveVelocity = Vector3.zero;
            if (Input.GetAxisRaw("Horizontal") < 0)
            {
                animator.SetBool("ismoving", true);
                transform.localScale = new Vector3(0.58f, 0.55f, 1);
                moveVelocity = Vector3.left;
                leftsee = true;


            }
            else if (Input.GetAxisRaw("Horizontal") > 0)
            {
                animator.SetBool("ismoving", true);
                transform.localScale = new Vector3(-0.58f, 0.55f, 1);
                leftsee = false;
                moveVelocity = Vector3.right;

            }
            else
            {

                animator.SetBool("ismoving", false);
            }


            transform.position += moveVelocity * movePower * Time.deltaTime;



        }




    }

    public void TakeDamage(int damage, string typeofhurt)
    {
        if(typeofhurt == "ad")
        {
            health.MyCurrentValue -= damage* 100/(controller.GetComponent<statcontroller>().armor+100);
            animator.SetTrigger("Hurt");
            
        }
        if(typeofhurt == "ap")
        {
            health.MyCurrentValue -= damage * 100 / (controller.GetComponent<statcontroller>().magicresistant + 100);
            animator.SetTrigger("Hurt");
        }
        if(typeofhurt == "static")
        {
            health.MyCurrentValue -= damage;
            animator.SetTrigger("Hurt");
        }
    }
    void modifyPhysics()
    {
        bool changingDirections = (direction.x > 0 && rigid.velocity.x < 0) || (direction.x < 0 && rigid.velocity.x > 0);

        if (IsOnGround() || IsOnGround_2())
        {
            if (Mathf.Abs(direction.x) < 0.4f || changingDirections)
            {
                rigid.drag = linearDrag;
            }
            else
            {
                rigid.drag = 1f;
            }
            rigid.gravityScale = 1;
        }
        else
        {
            rigid.gravityScale = gravity;
            rigid.drag = linearDrag * 1f;
            if (rigid.velocity.y < 0)
            {
                rigid.gravityScale = gravity * (fallMultiplier);
            }
            else if (rigid.velocity.y > 0 && !Input.GetButton("Jump"))
            {
                rigid.gravityScale = gravity * (fallMultiplier);
            }
            else
            {
                rigid.gravityScale = gravity * (fallMultiplier);
            }
        }
    }

    public bool IsOnGround()
    {
        Vector3 a = capsule.bounds.center;
        Vector3 b = new Vector3(-0.3f, 0, 0);
        Vector3 c = a + b;

        return Physics2D.Raycast(c, Vector2.down, 4.1f, jumpableground);   
    }
    public bool IsOnGround_2()
    {
        Vector3 a = capsule.bounds.center;
        Vector3 b = new Vector3(0.3f, 0, 0);
        Vector3 c = a + b;

        return Physics2D.Raycast(c, Vector2.down, 4.1f, jumpableground);
    }

    public void Jump()
    {
        if (isJumping)
        {
            /*
            if (jumpcount > 0)
            {
                jumpcount--;
                isJumping = true;
                animator.SetTrigger("doublejump");
            }
            */
            rigid.velocity = Vector2.zero;
            Vector2 jumpVelocity = new Vector2(0, jumpPower);
            rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);

            isJumping = false;

        }

        else
        {


   


        }

    }
    private void GetInput()
    {




        if (Input.GetKeyDown(KeyCode.I))
        {
            health.MyCurrentValue -= 10;
            mana.MyCurrentValue -= 10;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            health.MyCurrentValue += 10;
            mana.MyCurrentValue += 10;
        }

    }
    void OnTriggerEnter2D(Collider2D o) //충돌한 o의 Object의 tag가 Coin이면 충돌한 오브젝트의 이름을 출력하겠다는 뜻이다.
    {
        if (o.gameObject.tag == "obstacle")
        {
            
            health.MyCurrentValue -= 10;
        }

        if (o.gameObject.tag == "star")
        {

            o.gameObject.SetActive(false);
            respawnPoint = transform.position;
            Starpoint = Starpoint + 1;
            Debug.Log(Starpoint);
 

        }


        if(o.gameObject.tag == "ground")
        {
            stoproping = true;
        }

        if (o.gameObject.tag == "barrier")
        {

            o.transform.GetChild(0).gameObject.SetActive(true);
        }


    }
    void OnTriggerExit2D(Collider2D O)
    {
        if(O.gameObject.tag == "ground")
        {
            stoproping = false;
        }
    }
    public IEnumerator KnockBack(float knockbackpwr)
    {
   
        rigid.velocity = Vector2.zero;
        Vector2 jumpVelocity = new Vector2(-5, knockbackpwr);
        rigid.AddForce(jumpVelocity, ForceMode2D.Impulse);
        yield return 0;
    }

    IEnumerator CheckPlayerDeath()
    {
        while (true)
        {
            // 땅 밑으로 떨어졌다면

            // 체력이 0이하일 때
            if (health.MyCurrentValue <= 0)
            {
                isPlayerDead = true;
                //animator.SetTrigger("DEAD");
                yield return new WaitForSeconds(0.5F); // 1초 기다리기
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                
            }
            yield return new WaitForEndOfFrame(); // 매 프레임의 마지막 마다 실행
        }
    }
    private IEnumerator Dash()
    {
        if (mana.MyCurrentValue > 10)
        {
            canDash = false;
            isDashing = true;
            Instantiate(particleondash, dashtransform.transform.position, Quaternion.identity);
            float originalGravity = rigid.gravityScale;
            rigid.gravityScale = 0f;
            rigid.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
            mana.MyCurrentValue -= 10;
            tr.emitting = true;
            yield return new WaitForSeconds(dashingTime);
            tr.emitting = false;
            rigid.gravityScale = originalGravity;
            isDashing = false;
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;

        }

    } 

    public void AddDebuff(Debuff debuff)
    {
        if(!debuffs.Exists(x => x.GetType() == debuff.GetType()))
        {
            newdebuffs.Add(debuff);
        }

      
    }
    public void RemoveDebuff(Debuff debuff)
    {
        debuffsToRemove.Add(debuff);
        movePower = moveSpeed;

    }

    public void HandleDebuffs()
    {
        if(newdebuffs.Count >0)
        {
            debuffs.AddRange(newdebuffs);

            newdebuffs.Clear();
        }

        foreach(Debuff debuff in debuffsToRemove)
        {
            debuffs.Remove(debuff);
        }

        debuffsToRemove.Clear();

        foreach(Debuff debuff in debuffs)
        {
            debuff.Update();
        }

    }
    public int Damage
    {
        get
        {
            return damage;

        }
    }

    public float DebuffDuration
    {
        get
        {
            return DebuffDuration;
        }

        set
        {
            this.debuffDuration = value;
        }

    }
}

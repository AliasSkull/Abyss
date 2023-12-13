using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;


public enum Weapon 
{ 
    Sword,
    Crossbow

}
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    public Animator playerAnim;
    public SpriteRenderer sprite;
    
    public float speed;
    public float jumpForce;
    public float dashForce;
    public bool canJump;
    public bool isMoving;
    public bool canDash;
    public Vector2 moveDirection;
    public float coyoteTime = 0.5f;
    public float currentCoyoteTime;
    public float jumpCount;
   
    private float x;

    public bool isFacingRight;
    public bool isGrounded;
    public CapsuleCollider2D playerCollider;
    public PlayerLightController lightController;

    //Attack
    public Weapon activeWeapon;
    public Transform attackBox;
    public float attackRange;
    public int meleeDamage;
    public LayerMask enemyLayer;
    public GameObject arrowPrefab;
    public GameObject shootPOS;

    //Player stats
    private float maxHealth;
    private float currentHealth;

    private InputMapping input = null;

    [Header("Cool Down Checks")]
    public float currentDashTime;
    public float dashTime;



    private void Awake()
    {
        input = new InputMapping();
    }

    private void OnEnable()
    {
        input.Enable();
        
    }

    private void OnDisable()
    {
        input.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerAnim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - GetComponent<CapsuleCollider2D>().bounds.extents.y), 0.5f, LayerMask.GetMask("Ground")))
        {

            currentCoyoteTime = coyoteTime;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (!isGrounded)
        {
            currentCoyoteTime -= Time.deltaTime;
        }

        GetMoveDirection();

        if (isGrounded)
        {
            rb.AddForce(new Vector2(x, this.transform.position.y) * speed, ForceMode2D.Force);
        }



        if (rb.velocity.x >= 0.5 || rb.velocity.x <= -0.5)
        {
            playerAnim.SetBool("isRunning", true);
        }
        else 
        {
            playerAnim.SetBool("isRunning", false);
        }


        CheckPlayerMovement();
        DashCoolDown();
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        x = context.ReadValue<Vector2>().x;

    }

    public void OnJump(InputAction.CallbackContext context) 
    {
        if (context.performed && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            playerAnim.SetTrigger("Jump");
        }
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && isMoving || !isGrounded)
        {
            rb.AddForce(new Vector2(x,0) * dashForce, ForceMode2D.Impulse);
            currentDashTime = dashTime;
            canDash = false;
            Invoke(nameof(OnDashFinish), 0.5f);
            
        }
    }

    public void OnDashFinish() 
    {
        rb.velocity = new Vector2(0, 0);
    
    }

    public void DashCoolDown() 
    {
        if (currentDashTime > 0)
        {
            currentDashTime -= Time.deltaTime;
        }

        if (currentDashTime < 0)
        {
            currentDashTime = 0;
            canDash = true;
        }

    }

    public void OnAttack(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            switch (activeWeapon)
            {
                case Weapon.Sword:
                    //Melee Attack
                    playerAnim.SetTrigger("MeleeAttack1");
                    Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(attackBox.position, attackRange, enemyLayer);
                    foreach (Collider2D enemy in enemyColliders)
                    {
                        Debug.Log("Hit" + " " + enemy.name + " " + "for" + " " + meleeDamage);
                        enemy.GetComponent<Enemy>().TakeDamage(meleeDamage);
                    }
                    break;

                case Weapon.Crossbow:
                    GameObject arrow = Instantiate(arrowPrefab, shootPOS.transform.position, Quaternion.identity);
                    Rigidbody2D arrowrb = arrow.GetComponent<Rigidbody2D>();
                    arrowrb.AddForce(Vector2.right * 15, ForceMode2D.Impulse);
                    break;

            }
        }
    
    }

    public void OnWeaponChange(InputAction.CallbackContext context) 
    {
        if (context.performed)
        {
            if (activeWeapon == Weapon.Sword)
            {
                activeWeapon = Weapon.Crossbow;
            }

            if (activeWeapon == Weapon.Crossbow)
            {
                activeWeapon = Weapon.Sword;
            }
        }
    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "LightSource")
        {
            Debug.Log("Light Up");
            lightController.playerLight.intensity = lightController.maxLightIntensity;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackBox == null)
        {
            return;
        }
        Gizmos.DrawWireSphere(attackBox.position, attackRange);
    }

    public void GetMoveDirection() 
    {
        float x = Input.GetAxis("Horizontal");
        moveDirection = new Vector2(x, 0);
        moveDirection.Normalize();

        if (moveDirection.x > 0)
        {
            isFacingRight = true;

            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (moveDirection.x < 0)
        {
            isFacingRight = false;
            transform.rotation = Quaternion.Euler(0, 180f, 0);
        }
    
    }

    public bool CheckPlayerMovement()
    {
        if (rb.velocity.x > 0.5 || rb.velocity.x < -0.5)
        {
            return isMoving = true;
        }

        return isMoving = false;
    }
}

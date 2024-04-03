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

    private Vector2 moveVector;
    public float speed;
    public float jumpForce;
    public float dashForce;
    public bool canJump;
    public bool isJumping;
    public bool isMoving;
    public bool canDash;
    public bool isDashing;
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
    public float shootForce;

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
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCancelled;
        input.Player.WeaponChange.performed += OnWeaponChange;
        input.Player.Attack.performed += OnAttackPerformed;
        input.Player.Jump.performed += OnJumpPerformed;
        input.Player.Jump.canceled += OnJumpCancelled;
        input.Player.Dash.performed += OnDashPerformed;
        input.Player.Dash.canceled += OnDashCancelled;
        
    }

    private void OnDisable()
    {
        input.Disable();
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCancelled;
        input.Player.WeaponChange.performed -= OnWeaponChange;
        input.Player.Attack.performed -= OnAttackPerformed;
        input.Player.Jump.performed -= OnJumpPerformed;
        input.Player.Jump.canceled -= OnJumpCancelled;
        input.Player.Dash.performed -= OnDashPerformed;
        input.Player.Dash.canceled -= OnDashCancelled;
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
            rb.velocity = moveVector * speed;
            canJump = true;
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

    private void FixedUpdate()
    {
        if (isJumping == true)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = false;

        }

        if (isDashing == true)
        {
            rb.AddForce(moveDirection * dashForce, ForceMode2D.Impulse);
        }
    }

    public void OnMovementPerformed(InputAction.CallbackContext context)
    {
        moveVector = context.ReadValue<Vector2>();   

    }

    public void OnMovementCancelled(InputAction.CallbackContext context)
    {
        moveVector = Vector2.zero;
    }

    public void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            isJumping = true;
        }
    }

    public void OnJumpCancelled(InputAction.CallbackContext context)
    {
        isJumping = false;
    }

    public void OnDashPerformed(InputAction.CallbackContext context)
    {
        if (canDash) { isDashing = true; }
    }

    public void OnDashCancelled(InputAction.CallbackContext context)
    {
        isDashing = false;
        moveVector = Vector2.zero;
    
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

    public void OnAttackPerformed(InputAction.CallbackContext context) 
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
                    if (isFacingRight)
                    {
                        arrowrb.AddForce(Vector2.right * shootForce, ForceMode2D.Impulse);
                    }
                    if (!isFacingRight)
                    {
                        arrowrb.AddForce(-Vector2.right * shootForce, ForceMode2D.Impulse);
                    }
                    break;

            }
        
    
    }

    public void OnWeaponChange(InputAction.CallbackContext context) 
    {
        
            if (activeWeapon == Weapon.Sword)
            {
                activeWeapon = Weapon.Crossbow;
            }
            else if (activeWeapon == Weapon.Crossbow)
            {
                activeWeapon = Weapon.Sword;
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

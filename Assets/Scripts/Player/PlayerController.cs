using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using Pathfinding.Util;

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
    public CapsuleCollider2D playercollider;

    private Vector2 moveVector;
    public float speed;
    public float jumpForce;
    public float dodgeForce;
    public bool canJump;
    public bool isJumping;
    public bool isMoving;
    private bool canMove;
    public bool canDodge;
    public bool isDodging;
    public bool isCrouching;
    public bool isVulnerable = true;
    public Vector2 moveDirection;
    public float coyoteTime = 0.5f;
    public float currentCoyoteTime;
    public float jumpCount;
   
    private float x;

    public bool isFacingRight;
    public bool onGround;
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
        input.Player.Dodge.performed += OnDodgePerformed;
        input.Player.Crouch.performed += OnCrouchPerformed;
        input.Player.Crouch.canceled += OnCrouchCancelled;

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
        input.Player.Dodge.performed -= OnDodgePerformed;
        input.Player.Crouch.performed -= OnCrouchPerformed;
        input.Player.Crouch.canceled -= OnCrouchCancelled;

    }
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerAnim = GetComponent<Animator>();
    
        canDodge = true;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded();

        onGround = isGrounded();
       
        if (isGrounded())
        {
            currentCoyoteTime -= Time.deltaTime;
            canMove = true;
        }

        GetMoveDirection();

        if (isGrounded() && canMove)
        {
            rb.velocity = moveVector * speed;
          
        }



        if (rb.velocity.x >= 0.5 || rb.velocity.x <= -0.5)
        {
            playerAnim.SetBool("isRunning", true);
        }
        else 
        {
            playerAnim.SetBool("isRunning", false);
        }


        if (isDodging)
        {
            canMove = false;
        }

        if (isCrouching)
        {
            playercollider.offset = new Vector2(0, -0.08f);
            playercollider.size = new Vector2(0.24f, 0.0001f);
        }
        else 
        {
            playercollider.offset = new Vector2(-0.0048015574f, 0.0432126261f);
            playercollider.size = new Vector2(0.246332571f, 0.393575042f);
        }

        CheckPlayerMovement();
        
    }

    private void FixedUpdate()
    {
        if (isJumping == true)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = false;

        }

        if (isDodging == true)
        {

            playerAnim.SetTrigger("Dodge");
            rb.AddForce(moveDirection * dodgeForce, ForceMode2D.Impulse);
           

        }

    }

    public void OnDodgeStart()
    {
        isDodging = false;
        isVulnerable = false;
        
      
    }

    public void OnDodgeEnd()
    {
        isVulnerable = true;
        StartCoroutine(DodgeCooldown());
    }

    private bool isGrounded() 
    {
        //Gizmos.DrawCi
        if (isCrouching)
        {
            return Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y - 2f), 1f, LayerMask.GetMask("Ground"));
        }
        else
        { return Physics2D.OverlapCircle(new Vector2(this.transform.position.x, this.transform.position.y + 0.25f), 1f, LayerMask.GetMask("Ground")); }
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
        if (context.performed && isGrounded())
        {
            isJumping = true;
        }
    }

    public void OnJumpCancelled(InputAction.CallbackContext context)
    {
        if (context.canceled && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
    }

    public void OnDodgePerformed(InputAction.CallbackContext context)
    {
        if (context.performed && canDodge && isGrounded() &&isMoving)
        {
            Debug.Log("Dash");
            isDodging = true;
            canDodge = false;
        }
    }

    public void OnCrouchPerformed(InputAction.CallbackContext context)
    {
        isCrouching = true;
    }

    public void OnCrouchCancelled(InputAction.CallbackContext context)
    {
        isCrouching = false;
    }

    public IEnumerator DodgeCooldown() 
    {

        yield return new WaitForSeconds(1);
        canDodge = true;
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

using System.Collections;
using System.Collections.Generic;
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
    
    public float speed = 10f;
    public float jumpForce;
    public float turnRate;
    public float deaccelerationRate;
    public Vector2 moveDirection;

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
            isGrounded = true;

        }
        else 
        {
            isGrounded = false;
        }

        GetMoveDirection();

        if (isGrounded)
        {
            Move();
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetMouseButtonDown(0))
        {

            Attack();
        }


        if (rb.velocity.x >= 0.5 || rb.velocity.x <= -0.5)
        {
            playerAnim.SetBool("isRunning", true);
        }
        else 
        {
            playerAnim.SetBool("isRunning", false);
        }

    }

    public void Move()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            if (rb.velocity.x > 0 && isGrounded)
            {
                //quick turn
                rb.AddForce(Vector2.left * speed * turnRate * Time.deltaTime, ForceMode2D.Impulse);
            }
            rb.AddForce(Vector2.left * speed);


        }

        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            if (rb.velocity.x < 0 && isGrounded)
            {
                //quick turn
                rb.AddForce(Vector2.right * speed * turnRate * Time.deltaTime, ForceMode2D.Impulse);
            }

            rb.AddForce(Vector2.right * speed);
        }

        else if (rb.velocity.x != 0)
        {

            rb.AddForce(new Vector2(-rb.velocity.x, 0) * deaccelerationRate * Time.deltaTime, ForceMode2D.Impulse);
        }

    }

    public void Jump() 
    {
        
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        playerAnim.SetTrigger("Jump");

    }

    public void Attack() 
    {
        switch (activeWeapon)
        {
            case Weapon.Sword:
                //Melee Attack
                playerAnim.SetTrigger("MeleeAttack1");
                Collider2D [] enemyColliders = Physics2D.OverlapCircleAll(attackBox.position, attackRange, enemyLayer);
                foreach (Collider2D enemy in enemyColliders)
                {
                    Debug.Log("Hit" + " " + enemy.name + " " + "for" +" " + meleeDamage);
                    enemy.GetComponent<Enemy>().TakeDamage(meleeDamage);
                }
                break;

            case Weapon.Crossbow:
                break;
        
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
}

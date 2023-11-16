using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Rigidbody2D rb;

    public int maxHealth;
    public int currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(int Damage) 
    {

        currentHealth -= Damage;
        if (currentHealth <= 0)
        { 
            
        }
    
    }

    public void Death() 
    {
        Destroy(this.gameObject);
    }


}

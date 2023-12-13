using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Base Info")]
    public int currentHealth;
    public int maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;    
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth < 0)
        {
            Death();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth = currentHealth - damage;
    }

    public void Death() 
    { 
    
    }
}

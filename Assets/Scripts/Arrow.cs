using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float deathTime;
    public float currentDeathTime;

    // Start is called before the first frame update
    void Start()
    {
        currentDeathTime = deathTime;
    }

    // Update is called once per frame
    void Update()
    {
        currentDeathTime -= Time.deltaTime;

        if (currentDeathTime <= 0)
        {
            Destroy();
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        { 
            
        }
    }
}

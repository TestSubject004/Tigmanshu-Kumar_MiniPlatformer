using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collisionManager : MonoBehaviour
{

    public GameObject deathScreen;
    
    public GameObject endLevel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision) //collision checks for whether the player is dead or has finished the level
    {
        if(collision.gameObject.CompareTag ("trap"))
        {
            Time.timeScale = 0;
            Debug.Log("you are dead");
            deathScreen.SetActive(true);
        }
        if (collision.gameObject.CompareTag("finish"))
        {
            Debug.Log("END OF LEVEL");
            endLevel.SetActive(true);
        }
    }

   
}

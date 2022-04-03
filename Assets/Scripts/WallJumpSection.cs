using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpSection : MonoBehaviour
{
    // Fields
    private GameObject enemy;
    public float enemySpeed;

    /*private GameObject player;
    public float wallSlideSpeed;*/


    // Start
    private void Start()
    {
        enemy = GameObject.Find("Enemy");
        //player = GameObject.Find("Player");
    }

    // OnTriggerEnter
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player enters wall jump zone
        if (collision.gameObject.CompareTag("Player"))
        {
            enemy.GetComponent<Enemy>().wallJumpSpeed = enemySpeed;
            //player.GetComponent<Player>().wallSlideSpeed = wallSlideSpeed;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDeathCollider : MonoBehaviour
{
    // Fields
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Colliding with spikes. Should kill the player and trigger reset method
        if (collision.gameObject.CompareTag("Spike"))
        {
            // Plays transition sound
            if (player.isDead == false)
            {
                FindObjectOfType<AudioMan>().Play("Transition");
            }


            player.isDead = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MucusEntered : MonoBehaviour
{
    public float slow_intensity = 10.0f;
    public float jump_slow_intensity = 4.0f;
    public float damage_intensity = 0.5f;

    private GameObject current_target = null;
    private float last_damage_time = 0.0f;
    private int times_entered = 0;

    // Whenever the player enters the mucus they will immediately be slowed
    // Every second that passes with the player in the mucus they will take damage
    // Whenever the player exits the mucus they will no longer be slowed or take damage

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (current_target != null)
        {
            // If 1 second has passed then iterate damage to the current target
            if (Time.time - last_damage_time >= 1.0)
            {
                current_target.SendMessage("ModifyHealth", damage_intensity * -1);
                last_damage_time = Time.time;
            }   
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        // When the player enters the mucus then immediately slow the player
        GameObject other = collision.gameObject;
        if (other.tag == "Player")
        {
            // Targetting the player
            current_target = other;
            // When the player has entered twice, they are leaving and therefore don't need these reapplied
            times_entered++;
            if (times_entered == 1)
            {
                current_target.SendMessage("ModifySpeed", slow_intensity * -1);
                current_target.SendMessage("ModifyJumpSpeed", jump_slow_intensity * -1);
                current_target.SendMessage("ModifyHealth", damage_intensity * -1);
                last_damage_time = Time.time;
            }
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        // When the player leaves the mucus then regain the speed and drop the player as a target
        GameObject other = collision.gameObject;
        if (other.tag == "Player")
        {
            // If the player has entered the edge collider twice, they are exiting the mucus area, therefore drop mucus effects
            if (times_entered == 2)
            {
                current_target.SendMessage("ModifySpeed", slow_intensity);
                current_target.SendMessage("ModifyJumpSpeed", jump_slow_intensity);
                times_entered = 0;
                current_target = null;
            }
            // Otherwise do nothing - this is assumed that the player is now in the mucus area since they have entered and left
            // the edge collider
        }
    }

    // Overriding function to simulate that the player has left
    public void PlayerExitOverride()
    {
        if (current_target != null)
        {
            current_target.SendMessage("ModifySpeed", slow_intensity);
            current_target.SendMessage("ModifyJumpSpeed", jump_slow_intensity);
            times_entered = 0;
            current_target = null;
        }
    }
}

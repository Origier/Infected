using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Set the x and y values of the camera position to that of the player to follow the player around the game
        Vector3 new_pos = new Vector3(Player.transform.position.x, Player.transform.position.y, transform.position.z);
        transform.position = new_pos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirusController : MonoBehaviour
{
    public GameObject player;
    public float movement_speed = 5.0f;
    public float player_follow_range = 1.0f;
    public float time_between_coordinate_update = 0.5f;
    public float destination_range = 0.05f;

    private GameObject target;
    private bool hasTarget = false;
    private float current_target_x = 0.0f;
    private float current_target_y = 0.0f;
    private float time_since_last_coor_update = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            AttackTarget();
        }
        else
        {
            time_since_last_coor_update += Time.deltaTime;
            if (time_since_last_coor_update >= time_between_coordinate_update)
            {
                time_since_last_coor_update = 0.0f;
                current_target_x = player.transform.position.x + Random.Range(-player_follow_range, player_follow_range);
                current_target_y = player.transform.position.y + Random.Range(-player_follow_range, player_follow_range);
            }

            bool x_arrived = transform.position.x >= current_target_x - destination_range && transform.position.x <= current_target_x + destination_range;
            bool y_arrived = transform.position.y >= current_target_y - destination_range && transform.position.y <= current_target_y + destination_range;
            if (!(x_arrived && y_arrived))
            {
                FollowPlayer();
            }
            
        }
    }

    private void AttackTarget()
    {
        // FIXME: Implement following a target and colliding with the target to deal damage
    }

    private void FollowPlayer()
    {
        // A normalized vector to point the virus toward the direction of the player
        Vector2 direction = new Vector2(current_target_x - transform.position.x, current_target_y - transform.position.y).normalized;
        float x_change = transform.position.x + direction.x * movement_speed * Time.deltaTime;
        float y_change = transform.position.y + direction.y * movement_speed * Time.deltaTime;
        transform.position = new Vector3(x_change, y_change, transform.position.z);
    }
}

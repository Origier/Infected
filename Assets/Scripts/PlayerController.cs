using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    public float max_player_speed = 12.0f;
    public float max_player_speed_jump = 10.0f;
    public float max_player_health = 3.0f;
    public float cell_attachment_error_range = 0.05f;

    private Vector2 attachment_vector;
    private Vector3 space_force_vector;
    private Vector3 movement_force_vector;
    private float horizontal_input = 0.0f;
    private bool space_pressed = false;
    private Collider2D current_attached_wall;
    private float current_speed = 0.0f;
    private float current_speed_jump = 0.0f;
    private float current_health = 0.0f;
    private int mucus = 0;
    private Vector2 cells_location;
    private bool attached_to_cell = false;

    void Start()
    {
        current_health = max_player_health;
        current_speed = max_player_speed;
        current_speed_jump = max_player_speed_jump;
    }

    // Update is called once per frame
    void Update()
    {
        // Whenever the spacebar is pressed, any momentum in the horizontal axis will be maintained
        // The blob will continue to travel until it collides with a wall where it then sticks
        if (space_pressed)
        {
            attached_to_cell = false;

            // Apply the space force to the player based on jump speed
            transform.position = transform.position + (space_force_vector * Time.deltaTime * current_speed_jump);

            // Persist any momentum from sideways movement
            transform.position = transform.position + (movement_force_vector * horizontal_input * Time.deltaTime * current_speed);
        }
        else
        {
            // Movement left and right
            horizontal_input = Input.GetAxis("Horizontal");
            space_pressed = Input.GetKey(KeyCode.Space);

            transform.position = transform.position + (movement_force_vector * horizontal_input * Time.deltaTime * current_speed);
            if (attached_to_cell)
            {
                UpdatePositionRelativeToCell();
                UpdateForceVectorsRelativeToCell();
            }
            else
            {
                UpdateForceVectors();
            }
        }
    }

    private void UpdateForceVectors()
    {
        // Determine the closest point from the connected wall and then determine the difference vector to express this attachment
        Vector2 closest_point = current_attached_wall.ClosestPoint(transform.position);
        attachment_vector = new Vector2(closest_point.x - transform.position.x, closest_point.y - transform.position.y);

        // Calculating the direction of the space force by the direction of the attachment vector
        // Normalize so that the space_force is always a factor of 1
        space_force_vector = new Vector3(attachment_vector.x * -1, attachment_vector.y * -1, 0).normalized;

        // Calculating the movement force to apply based on the attachment vector, ensuring that the blob always
        // moves perpendicular to the surface of the object it is attached to
        Vector2 perp_attachment_vector = Vector2.Perpendicular(attachment_vector);
        movement_force_vector = new Vector3(perp_attachment_vector.x, perp_attachment_vector.y, 0).normalized;
    }

    private void UpdatePositionRelativeToCell()
    {
        // Getting the updated attachment vector and cell location
        cells_location = current_attached_wall.transform.position;
        Vector2 current_attachment_vector = new Vector2(cells_location.x - transform.position.x, cells_location.y - transform.position.y);

        // Getting a range for capturing the magnitude
        float upper_bound = attachment_vector.magnitude + cell_attachment_error_range;
        float lower_bound = attachment_vector.magnitude - cell_attachment_error_range;

        // If the player is not the same distance to the cell as they were previously then move the player according to the change
        if (!(current_attachment_vector.magnitude >= lower_bound && current_attachment_vector.magnitude <= upper_bound))
        {
            // Place the player at the expected position based on the attachement vector
            transform.position = cells_location + attachment_vector;
            print("Adjusting position");
        }
    }

    private void UpdateForceVectorsRelativeToCell()
    {
        // Determine the closest point from the connected wall and then determine the difference vector to express this attachment
        Vector2 closest_point = current_attached_wall.transform.position;
        attachment_vector = new Vector2(closest_point.x - transform.position.x, closest_point.y - transform.position.y);

        // Calculating the direction of the space force by the direction of the attachment vector
        // Normalize so that the space_force is always a factor of 1
        space_force_vector = new Vector3(attachment_vector.x * -1, attachment_vector.y * -1, 0).normalized;

        // Calculating the movement force to apply based on the attachment vector, ensuring that the blob always
        // moves perpendicular to the surface of the object it is attached to
        Vector2 perp_attachment_vector = Vector2.Perpendicular(attachment_vector);
        movement_force_vector = new Vector3(perp_attachment_vector.x, perp_attachment_vector.y, 0).normalized;
    }

    // When a wall is hit, end the vertical momentum
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;

        if (other.tag == "Wall")
        {
            // Setting up the known wall that we are attached to
            current_attached_wall = collision;

            UpdateForceVectors();

            space_pressed = false;
            attached_to_cell = false;
        }

        // When the other object is a cell then maintain the closest point must be the largest distance from the cell
        // Accomadate the cell movement by checking that the connecting distance is maintained
        if (other.tag == "Cell")
        {
            cells_location = other.transform.position;
            attachment_vector = new Vector2(cells_location.x - transform.position.x, cells_location.y - transform.position.y);
            attached_to_cell = true;

            // Setting up the known wall that we are attached to
            current_attached_wall = collision;

            UpdateForceVectors();

            space_pressed = false;
        }
    }

    public void ModifyJumpSpeed(float speed_change)
    {
        // Changes the current speed of the player by the speed_change, cannot exceed max speed and cannot go below 0
        current_speed_jump += speed_change;
        if (current_speed_jump > max_player_speed_jump)
        {
            current_speed_jump = max_player_speed_jump;
        } else if (current_speed_jump < 0.0)
        {
            current_speed_jump = 0.0f;
        }
    }

    public void ModifySpeed(float speed_change)
    {
        // Changes the current speed of the player by the speed_change, cannot exceed max speed and cannot go below 0
        current_speed += speed_change;
        if (current_speed > max_player_speed)
        {
            current_speed = max_player_speed;
        }
        else if (current_speed < 0.0)
        {
            current_speed = 0.0f;
        }
    }

    public void ModifyHealth(float health_change)
    {
        // Changes the players health by the amount specified, cannot exceed max health and cannot go below 0
        current_health += health_change;
        if (current_health > max_player_health)
        {
            current_health = max_player_health;
        }
        else if (current_health < 0.0)
        {
            current_health = 0.0f;
        }
    }

    public void SetMucus(int value)
    {
        mucus = value;
    }

    public int GetMucus()
    {
        return mucus;
    }

    public float GetHealth()
    {
        return current_health;
    }
}

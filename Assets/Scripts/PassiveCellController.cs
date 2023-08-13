using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveCellController : MonoBehaviour
{
    // The force that is randomly applied to the cell
    public float random_force = 500.0f;
    // The time the cell waits when coming to a stop before moving again
    public float wait_time = 1.0f;
    public float max_health = 3.0f;
    public float destruction_countdown = 0.5f;
    public Color fully_infected_color;
    public GameObject virus_prefab;
    public int max_number_of_virus_output = 5;
    public float virus_spawn_range = 1.5f;

    private float time_since_last_move = 0.0f;
    private Rigidbody2D cell_rb;
    private Rigidbody2D player_rb;
    private bool ready_to_move = false;
    private bool player_attached = false;
    private float time_since_last_damage = 0.0f;
    private bool start_destroy_countdown = false;
    private bool infected = false;
    private float infect_damage = 0.0f;
    private float current_health;
    private float time_till_death = 0.0f;

    // color change variables
    private float red_change_per_sec = 0.0f;
    private float green_change_per_sec = 0.0f;
    private float blue_change_per_sec = 0.0f;
    private SpriteRenderer cell_renderer;

    // Used to establish that the cell has stopped moving
    private Vector2 still_vector = new Vector2(0, 0);

    // Start is called before the first frame update
    void Start()
    {
        cell_rb = GetComponent<Rigidbody2D>();

        wait_time = Random.Range(0.0f, 1.0f);

        current_health = max_health;

        cell_renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ready_to_move)
        {
            // Apply the random force in a random direction to the cells rigidbody to simulate random meandering
            float rand_y = Random.Range(-1.0f, 1.0f);
            float rand_x = Random.Range(-1.0f, 1.0f);

            // Normalize the random direction to get a discrete 1 unit direction
            Vector2 force_vec = new Vector2(rand_x, rand_y).normalized;

            // Apply the force and set the ready to move flag
            cell_rb.AddForce(force_vec * random_force);
            if (player_attached)
            {
                player_rb.AddForce(force_vec * random_force);
            }
            ready_to_move = false;
            time_since_last_move = 0.0f;
        }
        else
        {
            // As long as the cell has stopped moving then allow wait time to pass before allowing movement again
            if (cell_rb.velocity == still_vector)
            {
                time_since_last_move += Time.deltaTime;
                if (time_since_last_move >= wait_time)
                {
                    ready_to_move = true;
                }
            }
        }

        // Checking for infection
        if (infected)
        {
            time_since_last_damage += Time.deltaTime;
            if (cell_renderer.color != fully_infected_color)
            {
                UpdateInfectedColor();
            }
            if (time_since_last_damage >= 1.0f)
            {
                current_health -= infect_damage;
                time_since_last_damage = 0.0f;
                if (current_health <= 0.0f)
                {
                    // If the player is still on the cell then launch the player off before destroying the cell
                    if (player_attached)
                    {
                        player_rb.gameObject.SendMessage("EjectFromCell");
                    }

                    // Starts a delayed destruction to allow the player to leave the cell first
                    start_destroy_countdown = true;
                }
            }
            else if(start_destroy_countdown && time_since_last_damage >= destruction_countdown)
            {
                // When the cell is infected and killed then spawn a random number of viruses from that kill
                int amount_of_virus_to_spawn = Random.Range(1, max_number_of_virus_output);

                for(int i = 0; i < amount_of_virus_to_spawn; i++)
                {
                    Instantiate(virus_prefab).transform.position = transform.position;
                    GameObject.FindGameObjectWithTag("Player").SendMessage("AddVirus");
                }

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the cell collides with a wall then the cell will bounce off that wall at the same angle it hit it at
        if (collision.tag == "Wall" || collision.tag == "Cell")
        {
            Vector2 vel = cell_rb.velocity;

            // Determine the closest point from the connected wall and then determine the difference vector to express this attachment
            Vector2 closest_point = collision.ClosestPoint(transform.position);
            Vector2 collision_vector_verticle = new Vector2(closest_point.x - transform.position.x, closest_point.y - transform.position.y).normalized;

            Vector2 collision_vector_horizontal = Vector2.Perpendicular(collision_vector_verticle);

            // Ensuring that the collision vector and the velocity vector are pointing the same way
            if ((collision_vector_horizontal.x > 0 && vel.x < 0) || (collision_vector_horizontal.x < 0 && vel.x > 0))
            {
                collision_vector_horizontal = collision_vector_horizontal * -1;
            }

            if ((collision_vector_horizontal.y > 0 && vel.y < 0) || (collision_vector_horizontal.y < 0 && vel.y > 0))
            {
                collision_vector_horizontal = collision_vector_horizontal * -1;
            }

            // Project velocity onto the wall direction vector to get the velocity in that direciton
            float dot_prod = (vel.x * collision_vector_horizontal.x) + (vel.y * collision_vector_horizontal.y);
            float wd_mag_squared = collision_vector_horizontal.magnitude * collision_vector_horizontal.magnitude;
            Vector2 velocity_proj_horizontal = collision_vector_horizontal * (dot_prod / wd_mag_squared);

            // Project velocity onto the verticle collision vector
            dot_prod = (vel.x * collision_vector_verticle.x) + (vel.y * collision_vector_verticle.y);
            wd_mag_squared = collision_vector_verticle.magnitude * collision_vector_verticle.magnitude;
            Vector2 velocity_proj_vertical = collision_vector_verticle * (dot_prod / wd_mag_squared);

            // Inversing the vertical projection to simply flip the verticle speed relative to the wall
            velocity_proj_vertical = velocity_proj_vertical * -1;

            Vector2 new_velocity = velocity_proj_vertical + velocity_proj_horizontal;
            cell_rb.velocity = new_velocity;
            if (player_attached)
            {
                player_rb.velocity = new_velocity;
            }
        }
    }

    // Attaches the player to the cell
    public void PlayerAttached(GameObject player)
    {
        player_rb = player.GetComponent<Rigidbody2D>();
        player_attached = true;
        player_rb.velocity = cell_rb.velocity;
    }

    // Detaches the player from the cell
    public void PlayerDetached()
    {
        player_rb.velocity = new Vector2(0.0f, 0.0f);
        player_rb = null;
        player_attached = false;
    }

    private void UpdateInfectedColor()
    {
        Color infected_color = new Color();
        infected_color.r = (Time.deltaTime * red_change_per_sec) + cell_renderer.color.r;
        infected_color.g = (Time.deltaTime * green_change_per_sec) + cell_renderer.color.g;
        infected_color.b = (Time.deltaTime * blue_change_per_sec) + cell_renderer.color.b;
        infected_color.a = 1.0f;
        cell_renderer.color = infected_color;
    }

    // Infects the cell with the virus, dealing the damage given per second
    public void InfectCell(float damage_per_second)
    {
        // Setting up the spriterenderer to change color each frame to represent the infection
        time_till_death = current_health / damage_per_second;
        float current_red = cell_renderer.color.r;
        float current_green = cell_renderer.color.g;
        float current_blue = cell_renderer.color.b;

        red_change_per_sec = (fully_infected_color.r - current_red) / time_till_death;
        green_change_per_sec = (fully_infected_color.g - current_green) / time_till_death;
        blue_change_per_sec = (fully_infected_color.b - current_blue) / time_till_death;


        // Setting up the cell to take damage each second
        infect_damage = damage_per_second;
        infected = true;
    }
}

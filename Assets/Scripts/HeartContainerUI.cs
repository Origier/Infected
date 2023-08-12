using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartContainerUI : MonoBehaviour
{
    public GameObject player;
    public GameObject[] full_heart_spots;
    public GameObject[] half_heart_spots;

    private PlayerController player_script;

    // Start is called before the first frame update
    void Start()
    {
        player_script = player.GetComponent<PlayerController>();

        UpdatePlayerHearts();
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePlayerHearts();
    }

    public void UpdatePlayerHearts()
    {
        float health = player_script.GetHealth();

        int whole_hearts = (int)(Mathf.Floor(health));
        int half_hearts = 0;
        if (health - whole_hearts > 0)
        {
            half_hearts = 1;
        }

        // Each heart container needs to have the alpha refreshed to remove old hearts
        for (int i = 0; i < full_heart_spots.Length; i++)
        {
            // For each whole heart, change the alpha for the heart object to 0
            Image heart = full_heart_spots[i].GetComponent<Image>();
            var temp_color = heart.color;
            temp_color.a = 0.0f;
            heart.color = temp_color;
        }

        for (int i = 0; i < half_heart_spots.Length; i++)
        {
            // For each whole heart, change the alpha for the heart object to 0
            Image heart = half_heart_spots[i].GetComponent<Image>();
            var temp_color = heart.color;
            temp_color.a = 0.0f;
            heart.color = temp_color;
        }

        for (int i = 0; i < whole_hearts; i++)
        {
            // For each whole heart, change the alpha for the heart object to 255
            Image heart = full_heart_spots[i].GetComponent<Image>();
            var temp_color = heart.color;
            temp_color.a = 1.0f;
            heart.color = temp_color;
        }

        // If there are half hearts then place them after all of the whole hearts
        if (half_hearts == 1)
        {
            Image heart = half_heart_spots[whole_hearts].GetComponent<Image>();
            var temp_color = heart.color;
            temp_color.a = 1.0f;
            heart.color = temp_color;
        }
    }
}

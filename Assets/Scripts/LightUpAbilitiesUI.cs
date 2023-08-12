using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightUpAbilitiesUI : MonoBehaviour
{
    public Image infect_ui_image;
    public Image consume_ui_image;
    public Image target_ui_image;
    public float inactive_alpha = 0.5f;
    public float active_alpha = 1.0f;
    public GameObject player;

    private PlayerController player_script;

    // Start is called before the first frame update
    void Start()
    {
        player_script = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
        // Updating the nessecary alphas for each ui image
        if (player_script.GetAttachedToCell())
        {
            print(player_script.GetAttachedToCell());
            // Updating the infect icon
            var temp_color = infect_ui_image.color;
            temp_color.a = active_alpha;
            infect_ui_image.color = temp_color;

            // Updating the consume icon
            temp_color = consume_ui_image.color;
            temp_color.a = active_alpha;
            consume_ui_image.color = temp_color;
        } 
        else
        {
            print(inactive_alpha);
            // Updating the infect icon
            var temp_color = infect_ui_image.color;
            temp_color.a = inactive_alpha;
            infect_ui_image.color = temp_color;

            // Updating the consume icon
            temp_color = consume_ui_image.color;
            temp_color.a = inactive_alpha;
            consume_ui_image.color = temp_color;
        }
    }
}

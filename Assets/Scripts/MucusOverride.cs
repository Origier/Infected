using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MucusOverride : MonoBehaviour
{
    public GameObject child_mucus;
    // Start is called before the first frame update
    void Start()
    {
        //unused
    }

    // Update is called once per frame
    void Update()
    {
        //unused
    }

    // Simple override for sending a message to the child mucus object that the player has left
    public void OnTriggerExit2D(Collider2D collision)
    {
        GameObject other = collision.gameObject;
        if (other.tag == "Player")
        {
            child_mucus.SendMessage("PlayerExitOverride");
        }
    }
}

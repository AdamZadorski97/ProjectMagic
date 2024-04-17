using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    public float climbSpeed = 3.0f; // Speed at which the player will climb
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FirstPersonController>())
        {
            other.GetComponent<FirstPersonController>().EnableLadderClimbing(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<FirstPersonController>())
        {
            other.GetComponent<FirstPersonController>().EnableLadderClimbing(false);
        }
    }
}

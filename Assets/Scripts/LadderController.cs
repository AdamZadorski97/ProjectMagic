using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderController : MonoBehaviour
{
    public float climbSpeed = 3.0f; // Speed at which the player will climb
    public LadderSide isSide;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            other.GetComponent<PlayerController>().EnableLadderClimbing(true, isSide);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerController>())
        {
            other.GetComponent<PlayerController>().EnableLadderClimbing(false, isSide);
        }
    }
}

public enum LadderSide
{
    top, bottom, left, right
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCAI : MonoBehaviour
{
    [Header("Weapon Setting")]
    [SerializeField] private GameObject Weapon;
    [SerializeField] private float DamageMultiplayer;

    [Header("Movement Setting")]
    [SerializeField] private float MovementSpeed;

    private Transform Target;
    private NavMeshAgent Agent;



    private void Start()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    private void Patrol()
    {
        if (Target == null)
        {
            FindNewTarget();
        }
    }

    private void FindNewTarget()
    {

    }


    private IEnumerator GotoTarget()
    {
        while (Vector3.Distance(transform.position, Target.position) < 0.1f)
        {

           
        }
        yield return null;
    }

    private void Chese()
    {

    }

    private void Attack()
    {

    }
    

}

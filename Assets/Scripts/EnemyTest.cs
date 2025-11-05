using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class EnemyTest : MonoBehaviour
{
    public NavMeshAgent agent;

    private Transform TargetPoint;

    public float VisionRange = 10f;

    public bool Attacking = false;

    //public LayerMask PlayerLayer;

    public void Start()
    {
        GameObject targetGameObject = GameObject.FindGameObjectWithTag("target");
        TargetPoint = targetGameObject.GetComponent<Transform>();
    }

    void Update()
    {
        GameObject targetGameObject = GameObject.FindGameObjectWithTag("target");
        TargetPoint = targetGameObject.GetComponent<Transform>();
        if (TargetPoint != null)
        {
            if (!Attacking)
            {
                Debug.Log("Going");
                agent.SetDestination(TargetPoint.position);
            }
            else if (Attacking)
            {
                Debug.Log("Found target, stopped moving");
            }
        }
    }
}

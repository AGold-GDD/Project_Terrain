using JetBrains.Annotations;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{

    public EnemyTest enemy;
    public float AttackDelay;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            Debug.Log("Target in range");
            enemy.Attacking = true;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            Debug.Log("Dealing damage");
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Target"))
        {
            Debug.Log("Target Left sight");
            enemy.Attacking = false;
        }
    }
}

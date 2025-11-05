using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject EnemyPrefabs;

    public Transform SpawnPoint;

    public float TimeDelay = 4f;

    public float Timer = 0;

    public void Update()
    {
        Timer += Time.deltaTime;
        //Debug.Log(Timer);

        if (Timer >= TimeDelay)
        {
            Debug.Log("Reset Timer");
            Timer = 0;
            Instantiate(EnemyPrefabs, SpawnPoint);
        }
    }
}

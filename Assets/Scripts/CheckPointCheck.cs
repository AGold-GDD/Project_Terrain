using UnityEngine;

public class CheckPointCheck : MonoBehaviour
{
    public GameObject Respawner;
    public bool CheckPoint = false;

    public void Start()
    {
        Respawner.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Rock")
        {
            Debug.Log("check");
            Respawner.SetActive(true);
            CheckPoint = true;
        }

    }
}

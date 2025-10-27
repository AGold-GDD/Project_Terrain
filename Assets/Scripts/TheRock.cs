using Unity.VisualScripting;
using UnityEngine;

public class TheRock : MonoBehaviour
{

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "0")
        {
            this.gameObject.SetActive(false);
        }
    }
}

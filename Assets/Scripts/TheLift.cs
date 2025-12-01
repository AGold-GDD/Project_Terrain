using Unity.VisualScripting;
using UnityEngine;

public class TheLift : MonoBehaviour
{
    public bool LiftIsOn = false;

    public float speed = 2;

    public GameObject Door;

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(speed);
        if (!LiftIsOn)
        {
            Door.SetActive(false);
            //Debug.Log("reached");

            if (!(transform.position.y <= 101.5))
            {
                transform.position -= Vector3.up * speed * Time.deltaTime;
                transform.localEulerAngles = new Vector3(0, -90, 0);
            }
            else if (transform.position.y <= 101.5)
            {
                transform.position = transform.position;
            }
        } 
        else if (LiftIsOn)
        {
            Door.SetActive(true);
            //Debug.Log("reached");
            if (!(transform.position.y >= 113))
            {
                transform.position += Vector3.up * speed * Time.deltaTime;
            }
            else if (transform.position.y >= 113)
            {
                transform.position = transform.position;
                transform.localEulerAngles = new Vector3(15, -90, 0);
            }
        }
    }
}

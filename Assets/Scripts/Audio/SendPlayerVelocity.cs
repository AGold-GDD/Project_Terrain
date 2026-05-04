using UnityEngine;

public class SendPlayerVelocity : MonoBehaviour
{
    
    private Rigidbody rb;
    private AudioBehavior audioBehavior;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start()
    {
        audioBehavior = AudioBehavior.Instance;
    }


    void FixedUpdate()
    {
        audioBehavior ??= AudioBehavior.Instance;
        if (audioBehavior is not null)
        {
            audioBehavior.PlayerVelocity = rb.linearVelocity.magnitude;   
        }
    }
}

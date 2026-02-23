using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class CartSystem : MonoBehaviour
{

    //This script handles the function to reset the cart. When the player makes it to a new checkpoint
    //They will be able to call the cart near the checkpoint they reached.

    //list of checkpoints
    public List<Transform> CartRespawnPoints;

    //the script for tracking the checkpoints
    public TrackManager track;

    public int num;

    public void Update()
    {
        num = track.nextCheckpoint;
        
        if (Input.GetKeyDown(KeyCode.T))
        {
            RespawnCart();
        }
    }

    public void RespawnCart()
    {
        transform.position = CartRespawnPoints[num].position;
    }
}

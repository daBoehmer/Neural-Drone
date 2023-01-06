using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomComponent : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if(!(collision.gameObject.TryGetComponent<Drone>(out Drone drone))) return; // if collided object isn't a drone, do nothing
        Controller.Instance.KillDrone(drone, DeathCause.Crash);
    }

}

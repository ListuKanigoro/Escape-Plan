using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOffTerrain : MonoBehaviour
{
    [SerializeField] GameObject terrain;
    Vector3 initialPosition;

    void Start() 
    {
        initialPosition = terrain.transform.position;
    }

    void OnTriggerStay(Collider other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            terrain.transform.position = new(87.38734f, -600.0f, 60.79514f);
        }
    }

    void OnTriggerExit(Collider other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            MovePosition(initialPosition);
        }
    }

    void MovePosition(Vector3 location)
    {
        terrain.transform.position = location;
    }
}
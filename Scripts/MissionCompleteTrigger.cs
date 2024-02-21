using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MissionCompleteTrigger : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI missionComplete;

    BoxCollider missionCompleteTrigger;

    void Start() 
    {
        missionComplete.fontSize = 0;

        missionCompleteTrigger = GetComponent<BoxCollider>();
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("Player"))
        {
            missionComplete.fontSize = 36;
        }
    }

    void OnTriggerExit(Collider other) 
    {
        missionCompleteTrigger.isTrigger = false;
    }
}

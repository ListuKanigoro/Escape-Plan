using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemMulfunction : MonoBehaviour
{
    [SerializeField] float warningSign;
    Image nearDeath;
    CharacterControl player;

    private float heartBeat;

    void Start()
    {
        nearDeath = GetComponent<Image>();
        player = FindObjectOfType<CharacterControl>();
    }

    // Update is called once per frame
    void Update()
    {
        heartBeat = Mathf.Sin(Time.time/warningSign);

        if(player.PlayerHealth < 35)
        {
            Color color = nearDeath.color;
            color.a = (heartBeat + 1) / 2; 
            nearDeath.color = color;
        }
        else
        {
            Color color = nearDeath.color;
            color.a = 0f;
            nearDeath.color = color;
        }
    }
}

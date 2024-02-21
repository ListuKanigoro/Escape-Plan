using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFollow : MonoBehaviour
{
    CinemachineFramingTransposer transposer;
    CinemachineVirtualCamera vcam;
    CharacterControl player;

    void Start()
    {
        player = FindObjectOfType<CharacterControl>();

        vcam = GetComponent<CinemachineVirtualCamera>();
        transposer = vcam.AddCinemachineComponent<CinemachineFramingTransposer>();

        transposer.m_CameraDistance = 2.1f;
        transposer.m_TrackedObjectOffset.x = 0.75f;
        transposer.m_TrackedObjectOffset.y = 1.8f;
        transposer.m_TrackedObjectOffset.z = 0.0f;
        transposer.m_XDamping = 1;
        transposer.m_YDamping = 1;
    }

    // Update is called once per frame
    void Update()
    {
        RunningZoomOut();

        Shooting();
    }

    void Shooting()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            transposer.m_TrackedObjectOffset.x = 0.79f;
            transposer.m_TrackedObjectOffset.y = 1.86f;
            transposer.m_TrackedObjectOffset.z = 2.5f;
            transposer.m_CameraDistance = 3.42f;
            transposer.m_XDamping = Mathf.Lerp(transposer.m_XDamping, 0, 2.4f * Time.deltaTime);
            transposer.m_YDamping = Mathf.Lerp(transposer.m_YDamping, 0, 2.4f * Time.deltaTime);
            transform.position = new Vector3(transposer.m_TrackedObjectOffset.x, transposer.m_TrackedObjectOffset.y, transposer.m_TrackedObjectOffset.z);
        }
        else
        {
            transposer.m_TrackedObjectOffset.x = 0.75f;
            transposer.m_TrackedObjectOffset.y = 1.8f;
            transposer.m_TrackedObjectOffset.z = 0.0f;
            transposer.m_CameraDistance = 2.1f;
            transposer.m_XDamping = 1;
            transposer.m_YDamping = 1;
        }
    }

    void RunningZoomOut()
    {
        if (player.GetComponent<Animator>().GetBool("isRunning") == true)
        {
            transposer.m_TrackedObjectOffset.x = 0f;
            transposer.m_TrackedObjectOffset.y = 1.54f;
            transposer.m_TrackedObjectOffset.z = 0f;
            transposer.m_CameraDistance = 3.42f;
            transform.position = new Vector3(transposer.m_TrackedObjectOffset.x, transposer.m_TrackedObjectOffset.y, transposer.m_TrackedObjectOffset.z);
        }
        else
        {
            transposer.m_TrackedObjectOffset.x = 0.75f;
            transposer.m_TrackedObjectOffset.y = 1.8f;
            transposer.m_TrackedObjectOffset.z = 0.0f;
            transposer.m_CameraDistance = 2.1f;
        }
    }
}

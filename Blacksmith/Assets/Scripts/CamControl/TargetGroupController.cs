using Unity.Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TargetGroupController : MonoBehaviour
{
    CinemachineTargetGroup targetGroup;
    
    [SerializeField] CinemachineCamera cinemachineCamera;
    CinemachineFollow cinemachineFollow;
    CinemachineRotationComposer cinemachineRotationComposer;
    CinemachineGroupFraming cinemachineGroupFraming;
    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    const string Anvil = "Anvil";
    const string Hone = "Hone";
    const string Brazier = "Brazier";
    const string QuenchingTank = "QuenchingTank";

    void Awake()
    {
        if(!cinemachineCamera)
            Debug.LogError("No CinemachineCamera found on " + gameObject.name);

        cinemachineFollow = cinemachineCamera.GetComponent<CinemachineFollow>();
        cinemachineRotationComposer = cinemachineCamera.GetComponent<CinemachineRotationComposer>();
        cinemachineGroupFraming = cinemachineCamera.GetComponent<CinemachineGroupFraming>();
        cinemachineBasicMultiChannelPerlin = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();

        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    void SetTarget(string key)
    {
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetTarget(Anvil);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetTarget(Hone);
        }
    }
}

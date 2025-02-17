using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

public class ViewController : MonoBehaviour
{
    const string Anvil = "Anvil";
    const string Hone = "Hone";
    const string Brazier = "Brazier";
    const string QuenchingTank = "QuenchingTank";

    [SerializeField] List<Transform> targets;

    CinemachineCamera cinemachineCamera;
    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;

    void Start()
    {
        if (null == targets)
            return;

        cinemachineCamera = GetComponent<CinemachineCamera>();
        cinemachineBasicMultiChannelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();

        ChangeTarget(Anvil);
    }

    void ChangeTarget(string key)
    {
        var target = targets.Find(t => t.name == key);
        if (null == target)
            return;

        cinemachineCamera.Follow = target;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeTarget(Anvil);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeTarget(Hone);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeTarget(Brazier);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeTarget(QuenchingTank);
        }
    }
}

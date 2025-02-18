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

    Dictionary<ForgeActionController.EAction, string> targetDic = new Dictionary<ForgeActionController.EAction, string>
    {
        {ForgeActionController.EAction.Hammering, Anvil},
        {ForgeActionController.EAction.Honing, Hone},
        {ForgeActionController.EAction.Brazing, Brazier},
        {ForgeActionController.EAction.Quenching, QuenchingTank},
    };

    void Start()
    {
        if (null == targets)
            return;

        cinemachineCamera = GetComponent<CinemachineCamera>();
        cinemachineBasicMultiChannelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();

        ChangeTarget(ForgeActionController.EAction.Hammering);
    }

    public void ChangeTarget(ForgeActionController.EAction key)
    {
        if(!targetDic.TryGetValue(key, out string targetName))
        {
            return;
        }

        var target = targets.Find(t => t.name == targetName);
        if (null == target)
        {
            return;
        }

        cinemachineCamera.Follow = target;
    }
}

using UnityEngine;

public interface IForgeAction 
{
    FPSArm FPSArm { get; set; }
    void Init();
    void UpdateAction();
    void WheelAction(float value);
}

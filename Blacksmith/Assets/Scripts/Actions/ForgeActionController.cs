using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ForgeActionController : MonoBehaviour
{
    delegate void ForgeAction();
    delegate void WheelAction(float value);

    public enum EAction
    {
        Hammering,
        Quenching,
        Brazing,
        Honing
    }


    [SerializeField] ViewController viewController;
    [SerializeField] Hammering hammering;

    List<ForgeAction> forgeActions = new List<ForgeAction>();

    IForgeAction currentAction;

    private void Start()
    {
        hammering?.Init();

        forgeActions.Add(ChangeHammerAction);
        forgeActions.Add(ChangeQuenchingAction);
        forgeActions.Add(ChangeBrazingAction);
        forgeActions.Add(ChangeHoningAction);
        
        ChangeAction(EAction.Hammering);
    }

    #region Change Forge Action

    void ChangeHammerAction()
    {
        currentAction = hammering;
        viewController.ChangeTarget(EAction.Hammering);
    }

    void ChangeQuenchingAction()
    {

    }

    void ChangeBrazingAction()
    {

    }

    void ChangeHoningAction()
    {

    }

    public void ChangeAction(EAction action)
    {
        forgeActions[(int)action]();
    }

    #endregion

    private void Update()
    {
        if (MouseInputHelper.IsMouseButtonDown)
            currentAction?.UpdateAction();

        currentAction?.WheelAction(MouseInputHelper.MouseWheel);

        #region Change Forge Action

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeAction(EAction.Hammering);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeAction(EAction.Quenching);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeAction(EAction.Brazing);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeAction(EAction.Honing);
        }

        #endregion
    }
}

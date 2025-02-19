using Unity.VisualScripting;
using UnityEngine;

public class MouseInputHelper 
{
    public static RaycastHit GetRaycastHit()
    {
        RaycastHit raycastHit;
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(mouseRay, out raycastHit);
        return raycastHit;
    }

    public static bool IsMouseButtonDown => Input.GetKeyDown(KeyCode.Mouse0);
    public static bool IsMouseButtonUp => Input.GetKeyUp(KeyCode.Mouse0);
    public static float MouseWheel => Input.GetAxis("Mouse ScrollWheel");
}

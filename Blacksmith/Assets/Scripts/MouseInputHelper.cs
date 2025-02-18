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
}

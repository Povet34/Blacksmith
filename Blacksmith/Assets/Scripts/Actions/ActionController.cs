using UnityEngine;

public class ActionController : MonoBehaviour
{
    [SerializeField] Hammering hammering;

    private void Start()
    {
        hammering?.Init();
    }
}

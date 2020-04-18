
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UdonCarThrottle : UdonSharpBehaviour
{
    [UdonSynced(UdonSyncMode.None)]
    public bool throttle = false;

    void Start()
    {
        
    }

    public override void OnPickupUseDown()
    {
        throttle = true;
    }

    public override void OnPickupUseUp()
    {
        throttle = false;
    }
}

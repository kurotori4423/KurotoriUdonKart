
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UdonCarSeat : UdonSharpBehaviour
{

    public UdonCarSystem carSystem;
    public GameObject exitPoint;

    public GameObject throttleR, throttleL, handle, seatSetting, carState;
    public GameObject leaveButton;

    public VRCStation station;

    public GameObject[] SyncUdonObjects;

    void Start()
    {
        
    }

    public override void Interact()
    {
        Networking.LocalPlayer.UseAttachedStation();

        Networking.SetOwner(Networking.LocalPlayer, carSystem.gameObject);
        carSystem.OnSeat();

        Networking.SetOwner(Networking.LocalPlayer, throttleR);
        Networking.SetOwner(Networking.LocalPlayer, throttleL);
        Networking.SetOwner(Networking.LocalPlayer, handle);
        Networking.SetOwner(Networking.LocalPlayer, seatSetting);
        Networking.SetOwner(Networking.LocalPlayer, leaveButton);
        Networking.SetOwner(Networking.LocalPlayer, carState);

        if(SyncUdonObjects != null)
        {
            for(int i = 0; i < SyncUdonObjects.Length; ++i)
            {
                Networking.SetOwner(Networking.LocalPlayer, SyncUdonObjects[i]);
            }
        }

        leaveButton.SetActive(true);
    }

    public void LeaveSeat()
    {
        //Networking.LocalPlayer.TeleportTo(exitPoint.transform.position, exitPoint.transform.rotation);
        station.ExitStation(Networking.LocalPlayer);
        leaveButton.SetActive(false);
    }
}

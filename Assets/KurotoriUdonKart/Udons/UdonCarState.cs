
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// 車の状態を同期保持するためのUDON

public class UdonCarState : UdonSharpBehaviour
{
    public UdonCarSystem carSystem;

    [UdonSynced(UdonSyncMode.None)]
    public bool seated;
    [UdonSynced(UdonSyncMode.None)]
    public float steering;
    [UdonSynced(UdonSyncMode.None)]
    public float velocity;

    void Start()
    {
        seated = false;
    }

    public void SetSeated(bool flag)
    {
        seated = flag;
    }

    public bool GetSeated()
    {
        return seated;
    }

    private void LateUpdate()
    {
        if (Networking.LocalPlayer == null)
        {
            steering = carSystem.steering;
            velocity = carSystem.velocity;
            return;
        }

        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;

        // オーナーでのみ動作

        steering = carSystem.steering;
        velocity = carSystem.velocity;
    }
}

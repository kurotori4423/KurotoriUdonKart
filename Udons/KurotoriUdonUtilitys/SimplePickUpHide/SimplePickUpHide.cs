
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// ピックアップ時に消える機能

public class SimplePickUpHide : UdonSharpBehaviour
{
    public GameObject hideObject;


    void Start()
    {
        
    }

    public override void OnPickup()
    {
        hideObject.SetActive(false);
    }

    public override void OnDrop()
    {
        hideObject.SetActive(true);
    }
}

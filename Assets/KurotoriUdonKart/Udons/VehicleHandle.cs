
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class VehicleHandle : UdonSharpBehaviour
{

    public Transform leftHand;
    public Transform rightHand;

    public VRC_Pickup pickupL, pickupR;

    private Vector3 posL, posR;
    private Quaternion rotL, rotR;


    public Text angleText;

    void Start()
    {
        if (angleText != null)  angleText.text = "0";

        posL = leftHand.localPosition;
        posR = rightHand.localPosition;

        rotL = leftHand.localRotation;
        rotR = rightHand.localRotation;
    }

    [UdonSynced(UdonSyncMode.None)]
    public float angle;



    public void FixedUpdate()
    {
        if (!Networking.IsOwner(gameObject)) return;

        Vector3 LtoR = (rightHand.localPosition - leftHand.localPosition);
        LtoR.z = 0.0f;

        angle =  Mathf.Acos(Vector3.Dot(LtoR.normalized, Vector3.up)) * Mathf.Rad2Deg - 90.0f;  
      
        if(angleText != null) angleText.text = angle.ToString("F1");

        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    public void ForceDrop()
    {
        pickupL.Drop();
        pickupR.Drop();
    }


    public void PickupDisable()
    {
        pickupL.pickupable = false;
        pickupR.pickupable = false;
    }

    public void PickupEnable()
    {
        pickupL.pickupable = true;
        pickupR.pickupable = true;
    }

    public void ResetHandlePos()
    {
        leftHand.localPosition = posL;
        rightHand.localPosition = posR;

        leftHand.localRotation = rotL;
        rightHand.localRotation = rotR;
    }
}

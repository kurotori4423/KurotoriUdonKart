
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class UdonCarSettings : UdonSharpBehaviour
{

    public Transform SeatObject;
    public GameObject settingWindow;

    public Slider seatHeightSlider;
    public float heightOffset = 0;
    public float heightWidth = 0.1f;

    public Slider seatFBSlider;
    public float FBOffset = 0;
    public float FBWidth = 0.4f;

    [UdonSynced(UdonSyncMode.None)]
    private float seatHeight;

    [UdonSynced(UdonSyncMode.None)]
    private float seatFB;

    private bool toggle = false;




    void Start()
    {
        seatHeightSlider.maxValue = heightOffset + heightWidth;
        seatHeightSlider.minValue = heightOffset - heightWidth;
        seatHeightSlider.value = heightOffset;
        seatHeight = heightOffset;

        seatFBSlider.maxValue = FBOffset + FBWidth;
        seatFBSlider.minValue = FBOffset - FBWidth;
        seatFBSlider.value = FBOffset;
        seatFB = FBOffset;

    }

    private void LateUpdate()
    {
        var temp = SeatObject.localPosition;
        SeatObject.localPosition = new Vector3(temp.x, seatHeight, seatFB);
    }

    public void ChangeSeatHeight()
    {
        seatHeight = seatHeightSlider.value;
    }

    public void ChangeSeatFB()
    {
        seatFB = seatFBSlider.value;
    }

    public void ToggleSeting()
    {
        toggle = !toggle;
        if (toggle)
        {
            settingWindow.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            settingWindow.transform.localScale = new Vector3(0, 0, 0);
        }
    }
}

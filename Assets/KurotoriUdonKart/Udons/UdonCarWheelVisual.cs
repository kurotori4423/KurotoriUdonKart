
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UdonCarWheelVisual : UdonSharpBehaviour
{
    public UdonCarState carState;

    public Transform leftFrontWheelVisual;
    public Transform rightFrontWheelVisual;

    public Transform leftFrontSteerVisual;
    public Transform rightFrontSteerVisual;

    public Transform rearWheelVisual;

    public Vector3 rotationAxis = new Vector3(0,1,0);

    public const float VISUAL_WHEEL_SPEED = 5.0f;  // タイヤの見た目の回転速度係数
    
    void Start()
    {
        
    }

    private void LateUpdate()
    {
        var steerRotation = Quaternion.Euler(0, 0, carState.steering);
        leftFrontSteerVisual.localRotation = steerRotation;
        rightFrontSteerVisual.localRotation = steerRotation;

        var spinSpeed = rotationAxis * carState.velocity * VISUAL_WHEEL_SPEED;
        leftFrontWheelVisual.transform.Rotate(spinSpeed);
        rightFrontWheelVisual.transform.Rotate(spinSpeed);
        rearWheelVisual.transform.Rotate(spinSpeed);
    }
}


using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class UdonCarSystem : UdonSharpBehaviour
{
    public WheelCollider leftFrontWheel;
    public WheelCollider rightFrontWheel;

    public WheelCollider leftRearWheel;
    public WheelCollider rightRearWheel;

    public const float MAX_SPEED = 150.0f;          // 最大スピード
    public const float MAX_MOTOR_TORQUE = 300;      // 最大モータートルク
    public const float MAX_STEERING_ANGLE = 30;     // 最大ステアリング角度
    public const float MAX_STEERING_ANGLE_VR = 60.0f; // VRでの最大ステアリング角度
    public const float MAX_BRAKE_TORQUE = 100;      // 最大ブレーキトルク
    public const float CONST_BRAKE_TORQUE = 0.0f;   // 恒常的なブレーキ
    public const float DOWN_FORCE = 0.1f;           // ダウンフォースの強さ
    public const float CENTER_MASS_POS = 0.0f;      // 重心の低さ
    
    public const float steerHelper = 0.0f; // ステアリング補助

    public UdonCarSeat carSeat;
    public VehicleHandle VRHandle;
    public UdonCarThrottle throttleF, throttleB;

    public UdonCarState state;
    public UsingStatusDisplay usingStatusDisplay;

    public Text debugText;
    
    private float motor = 0;
    
    public float steering = 0;

    private float brake = 0;

    [UdonSynced(UdonSyncMode.None)]
    private bool seated = false;

    public float velocity;

    private Vector3 resetPos;
    private Quaternion resetRot;
    private Rigidbody rigidBody;

    private bool vrMode;
    private float oldRotation;

    // 速度が上がるほどハンドルのききを抑制する。
    float HandleMapping(float sp, float maxsp, float min)
    {
        return Mathf.Clamp((Mathf.Abs(sp) * (-min / maxsp) + 1.0f), min, 1.0f);
    }

    float Remap(float value, float min1, float max1, float min2, float max2)
    {
        return min2 + (value - min1) * (max2 - min2) / (max1 - min1);
    }

    void Start()
    {
        debugText.text = "";
        resetPos = transform.position;
        resetRot = transform.rotation;
        rigidBody = GetComponent<Rigidbody>();

        rigidBody.centerOfMass = new Vector3(0.0f, CENTER_MASS_POS, 0.0f);

        vrMode = true;
    }

    private void CalcCarVelocity()
    {
        var direction = transform.forward;
        if (rigidBody.velocity.magnitude < 0.0001f)
        {
            velocity = 0.0f;
            return;
        }
        var sign = Mathf.Sign(Vector3.Dot(rigidBody.velocity.normalized, direction));
        velocity = Vector3.Project(rigidBody.velocity, direction).magnitude * sign;
    }

    private void MessageUpdate()
    {
        debugText.text = "";
        if (Networking.GetOwner(gameObject) != null)
        {
            debugText.text += "Owner: " + Networking.GetOwner(gameObject).displayName + "\n";
        }

        var direction = transform.forward;
        
        float KmHour = velocity * 3.6f;
        debugText.text += "Velocity: " + KmHour.ToString("F1") + " km/s \n";

        if (Networking.LocalPlayer != null)
        {
            if (Networking.LocalPlayer.IsUserInVR())
            {
                debugText.text += "LTrigger:Back RTrigger:Forward\n";
                debugText.text += "Exit : Intaract Me!\n";
            }
            else
            {
                debugText.text += "WASD Control\n";
                debugText.text += "Exit : Q Key\n";
            }
        }
    }

    public void LateUpdate()
    {   

        if (Networking.LocalPlayer == null)
        {
            SteerHelper();
            AdjustSpeed();
            CalcCarVelocity();
            MessageUpdate();
            ControllerEditorDebug();
            // ダウンフォース
            rigidBody.AddForce(0, Mathf.Abs(velocity) * DOWN_FORCE, 0);
            return;
        }

        var owner = Networking.GetOwner(gameObject);
        if (owner == null) return;

        
        
        MessageUpdate();

        if (!Networking.IsOwner(gameObject)) return;

        SteerHelper();
        AdjustSpeed();
        CalcCarVelocity();

        // 降車時のオートブレーキ
        if (!seated)
        {
            leftFrontWheel.brakeTorque = 10000.0f;
            rightFrontWheel.brakeTorque = 10000.0f;
            leftRearWheel.brakeTorque = 10000.0f;
            rightRearWheel.brakeTorque = 10000.0f;

            return;

        }else
        {
            leftFrontWheel.brakeTorque = .0f;
            rightFrontWheel.brakeTorque = .0f;
            leftRearWheel.brakeTorque = .0f;
            rightRearWheel.brakeTorque = .0f;
        }
        
        // ダウンフォース
        rigidBody.AddForce(0, Mathf.Abs(velocity) * DOWN_FORCE, 0);
        
        if(vrMode)
        {
            ControllerVR();
        }
        else
        {
            ControllerDesktop();
        }

        leftFrontWheel.steerAngle = steering;
        rightFrontWheel.steerAngle = steering;

        leftRearWheel.motorTorque = motor;
        rightRearWheel.motorTorque = motor;

        leftFrontWheel.brakeTorque = brake;
        rightFrontWheel.brakeTorque = brake;
        leftRearWheel.brakeTorque = brake;
        rightRearWheel.brakeTorque = brake;

    }

    private void ControllerDesktop()
    {
        brake = 0.0f;

        if (Input.GetKey(KeyCode.S))
        {
            if (velocity > 0.00001f)
            {
                motor = 0.0f;
                brake = MAX_BRAKE_TORQUE;
            }
            else
            {
                motor = -MAX_MOTOR_TORQUE;
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            if (velocity < -0.00001f)
            {
                motor = 0.0f;
                brake = MAX_BRAKE_TORQUE;
            }
            else
            {
                motor = MAX_MOTOR_TORQUE;
            }
        }
        else
        {
            motor = CONST_BRAKE_TORQUE;
        }


        if (Input.GetKey(KeyCode.A))
        {
            steering = -MAX_STEERING_ANGLE * HandleMapping(velocity, 90, 0.2f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            steering = MAX_STEERING_ANGLE * HandleMapping(velocity, 90, 0.2f);
        }else
        {
            steering = 0;
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            LeaveSeat();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTransform();
        }

    }

    private void ControllerVR()
    {
        brake = 0.0f;
        steering = Remap(VRHandle.angle, -90.0f, 90.0f, -MAX_STEERING_ANGLE_VR, MAX_STEERING_ANGLE_VR) * HandleMapping(velocity, 90, 0.1f);

        if (throttleB.throttle)
        {
            if (velocity > 0.00001f)
            {
                motor = 0.0f;
                brake = MAX_BRAKE_TORQUE;
            }
            else
            {
                motor = -MAX_MOTOR_TORQUE;
            }
        }
        else if (throttleF.throttle)
        {
            if (velocity < -0.00001f)
            {
                motor = 0.0f;
                brake = MAX_BRAKE_TORQUE;
            }
            else
            {
                motor = MAX_MOTOR_TORQUE;
            }
        }
        else
        {
            motor = 0;
        }

    }

    private void ControllerEditorDebug()
    {
        // Editorモードでのデバッグ

        leftFrontWheel.brakeTorque = .0f;
        rightFrontWheel.brakeTorque = .0f;
        leftRearWheel.brakeTorque = .0f;
        rightRearWheel.brakeTorque = .0f;

        ControllerDesktop();

        leftFrontWheel.steerAngle = steering;
        rightFrontWheel.steerAngle = steering;

        //leftRearWheel.motorTorque = motor;
        //rightRearWheel.motorTorque = motor;

        leftFrontWheel.motorTorque = motor;
        rightFrontWheel.motorTorque = motor;

        leftFrontWheel.brakeTorque = brake;
        rightFrontWheel.brakeTorque = brake;
        leftRearWheel.brakeTorque = brake;
        rightRearWheel.brakeTorque = brake;
    }

    private void SteerHelper()
    {
        if(Mathf.Abs(oldRotation - transform.eulerAngles.y) < 10f)
        {
            var turnadjust = (transform.eulerAngles.y - oldRotation) * steerHelper;
            Quaternion velRotation = Quaternion.AngleAxis(turnadjust, Vector3.up);
            rigidBody.velocity = velRotation * rigidBody.velocity;
        }
        oldRotation = transform.eulerAngles.y;
    }

    public void ResetTransform()
    {
        // 地下に飛ばして強制テレポートさせる。
        transform.position = new Vector3(0,-1000,0);
        transform.rotation = resetRot;

        rigidBody.velocity = new Vector3(0, 0, 0);
    }

    public void RestTransformNetwork()
    {
        SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "ResetTransform");
    }

    public void LeaveSeat()
    {
        seated = false;
        VRHandle.ForceDrop();
        VRHandle.PickupDisable();
        VRHandle.ResetHandlePos();
        usingStatusDisplay.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "SetSeatedDisable");
        carSeat.LeaveSeat();
    }

    public void OnSeat()
    {
        seated = true;
        VRHandle.PickupEnable();
        vrMode = Networking.LocalPlayer.IsUserInVR();
        usingStatusDisplay.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "SetSeatedEnable");
    }

    public bool GetSeated()
    {
        return seated;
    }

    public void AdjustSpeed()
    {
        if(Mathf.Abs(velocity * 3.6f) > MAX_SPEED)
        {
            rigidBody.velocity = (MAX_SPEED / 3.6f) * rigidBody.velocity.normalized;
        }
    }
}

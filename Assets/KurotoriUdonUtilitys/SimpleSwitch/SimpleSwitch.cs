
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// インタラクト時に指定したUDONの関数を実行するシンプルなスイッチ

public class SimpleSwitch : UdonSharpBehaviour
{

    public UdonBehaviour udon;
    public string Method;
    

    public override void Interact()
    {
        udon.SendCustomEvent(Method);
    }
}


using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

// �C���^���N�g���Ɏw�肵��UDON�̊֐������s����V���v���ȃX�C�b�`

public class SimpleSwitch : UdonSharpBehaviour
{

    public UdonBehaviour udon;
    public string Method;
    

    public override void Interact()
    {
        udon.SendCustomEvent(Method);
    }
}

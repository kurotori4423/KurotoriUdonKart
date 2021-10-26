
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UsingStatusDisplay : UdonSharpBehaviour
{
    public GameObject freeButton, occupiedButton;

    [UdonSynced(UdonSyncMode.None)]
    bool seated;

    void Start()
    {
    }

    private void Update()
    {
        freeButton.SetActive(!seated);
        occupiedButton.SetActive(seated);
    }

    public void SetSeatedEnable()
    {
        seated = true;
    }

    public void SetSeatedDisable()
    {
        seated = false;
    }
}

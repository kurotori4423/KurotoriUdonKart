
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class UdonTimeAttack : UdonSharpBehaviour
{

    public GameObject[] checkPoints;

    public UdonCarTimeRanking Ranking;

    public Text timeText;

    private float time;
    private int couseIndex = 0;
    private bool enable = false;

    [UdonSynced(UdonSyncMode.None)]
    public float RaceTime = 9999999999.9f;
    [UdonSynced(UdonSyncMode.None)]
    public int PlayerID;
    [UdonSynced(UdonSyncMode.None)]
    public bool Stating;


    void Start()
    {
        time = 0;
        PlayerID = -1;
        Stating = false;
        timeText.text = "";

        if (checkPoints != null && checkPoints.Length > 2) enable = true;
    }

    private void Update()
    {
        if(Stating) time += Time.deltaTime;

        timeText.text = time.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (enable)
        {
            var a = other.gameObject;

            if (a.Equals(checkPoints[0]))
            {
                Stating = true;
                time = 0;
                couseIndex = 1;
                Debug.Log("RaceStart!");
                return;
            }

            if (Stating && couseIndex == checkPoints.Length - 1 && a.Equals(checkPoints[checkPoints.Length - 1]))
            {
                Stating = false;
                RaceTime = time;
                PlayerID = Networking.GetOwner(gameObject).playerId;

                Debug.Log("RACETIME: " + time);

                Ranking.SendCustomNetworkEvent(VRC.Udon.Common.Interfaces.NetworkEventTarget.Owner, "UpdateRanking");
                return;
            }

            if (Stating && a.Equals(checkPoints[couseIndex]))
            {
                Debug.Log("CHECKPOINT: " + couseIndex);
                couseIndex += 1;
            }
        }
    }
}

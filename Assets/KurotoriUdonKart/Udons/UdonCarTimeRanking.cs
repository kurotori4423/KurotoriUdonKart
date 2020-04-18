
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

public class UdonCarTimeRanking : UdonSharpBehaviour
{

    public UdonTimeAttack[] timeAttacker;

    [UdonSynced(UdonSyncMode.None)]
    string highScoreText;

    [UdonSynced(UdonSyncMode.None)]
    float topTime = 9999999999.9f;
    [UdonSynced(UdonSyncMode.None)]
    int topPlayer = -1;

    public Text highScoreTextObject;

    void Start()
    {
        highScoreTextObject.text = "PLAYER1 : XX:XX:XX";
        highScoreText = "PLAYER1 : XX:XX:XX";
    }

    private void Update()
    {
        highScoreTextObject.text = highScoreText;
    }

    public void UpdateRanking()
    {
        bool scoreUpdate = false;

        for(int i = 0; i < timeAttacker.Length; i++)
        {
            if (timeAttacker[i].Stating) continue;

            var time = timeAttacker[i].RaceTime;
            var id = timeAttacker[i].PlayerID;
            
            if(time < topTime)
            {
                topTime = time;
                topPlayer = id;

                Debug.Log("topTime: " + time.ToString());
                Debug.Log("topPlayer: " + time.ToString());
                scoreUpdate = true;
            }
        }

        if(scoreUpdate)
        {
            HighScoreEvent();
        }

    }

    void HighScoreEvent()
    {
        var player = VRCPlayerApi.GetPlayerById(topPlayer);
        if (player == null) return;

        highScoreText = VRCPlayerApi.GetPlayerById(topPlayer).displayName + " : ";
        highScoreText += topTime.ToString();
    }
    
}

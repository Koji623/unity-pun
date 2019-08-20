using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class RoomContentController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text roomName = null;

    [SerializeField]
    private TMP_Text playerCount = null;

    [SerializeField]
    private Button joinButton = null;


    public void SetContent(string roomName, int playerCount, int maxPlayer, UnityAction<string> action)
    {
        this.roomName.text = roomName;
        this.playerCount.text = playerCount + "/" + maxPlayer;
        joinButton.onClick.AddListener(() => action(roomName));
    }

}

using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;


public class TitleController : MonoBehaviourPunCallbacks
{
    #region Private Constants

    // Store the PlayerPref Key to avoid typos
    const string playerNamePrefKey = "PlayerName";

    #endregion



    #region Private Serializeble Fields

    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;

    [SerializeField]
    private GameObject LoadingObj = null;

    [SerializeField]
    private Transform ContentsHolder = null;

    #endregion



    #region Private Fields

    private InputField nameField;

    private string nameValue = "Player";
    string gameVersion = "1";

    #endregion



    #region MonoBehaviour Callbacks

    void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.GameVersion = gameVersion;
    }

    void Start()
    {
        nameField = GameObject.FindWithTag("NameField").GetComponent<WebGLNativeInputField>();
        //プレイヤー名をPlayerPrefsから取得
        if (PlayerPrefs.HasKey(playerNamePrefKey))
        {
            nameValue = PlayerPrefs.GetString(playerNamePrefKey);
            nameField.text = nameValue;
        }
        Connect();
    }

    #endregion



    #region Public Methods

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            //PhotonNetwork.JoinRandomRoom();
            PhotonNetwork.JoinLobby();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void SetName(string value)
    {
        nameValue = value;
        PlayerPrefs.SetString(playerNamePrefKey, value);
    }

    //ランダム参加
    public void joinGame()
    {
        PhotonNetwork.LocalPlayer.NickName = nameValue;
        PhotonNetwork.JoinRandomRoom();
        LoadingObj.SetActive(true);
    }

    //ルーム名を選択して参加
    public void JoinSelectedRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    //ルーム作成 & 参加
    public void CreateRoom()
    {
        PhotonNetwork.LocalPlayer.NickName = nameValue;
        LoadingObj.SetActive(true);

        RoomOptions roomOptions = new RoomOptions { MaxPlayers = maxPlayersPerRoom };
        // ルームオプションにカスタムプロパティを設定
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "Ball_status",false},
            { "redScore", 0 },
            { "greenScore", 0 }
        };
        roomOptions.CustomRoomProperties = customRoomProperties;
        //ルームを作成
        PhotonNetwork.CreateRoom(PhotonNetwork.LocalPlayer.NickName, roomOptions);
    }

    #endregion



    #region MonoBehaviourPunCallbacks Callbacks

    // マスターサーバーへの接続が成功した時に呼ばれるコールバック
    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN / OnConnectedMaster");
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(Photon.Realtime.DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN / OnDisconnected{0}", cause);
    }

    // 部屋の作成に失敗した時
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("PUN / OnCreateRoomFailed");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("PUN / Created Room");
    }

    //ルームリストに変更があったときルーム一覧表示 (JoinLobbyしたときに呼ばれる)
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("PUN / Update to RoomList");

        //コンテンツホルダー内のデータを全消去
        foreach (Transform n in ContentsHolder.transform)
        {
            GameObject.Destroy(n.gameObject);
        }

        //プレハブから各ルームのビューを生成
        GameObject contentPrefab = (GameObject)Resources.Load("RoomView");
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.PlayerCount == 0)
            {
                continue;
            }
            GameObject roomObj = Instantiate(contentPrefab, Vector3.zero, Quaternion.identity, ContentsHolder);
            RoomContentController roomContentController = roomObj.GetComponent<RoomContentController>();
            roomContentController.SetContent(roomInfo.Name, roomInfo.PlayerCount, roomInfo.MaxPlayers, str => PhotonNetwork.JoinRoom(str));
        }
    }

    // ランダムな部屋への入室に失敗した時
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN / OnJoinRandamFailed");
        CreateRoom();
    }

    // 特定の部屋への入室に失敗した時
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("PUN / OnJoinRoomFailed");
    }

    // マッチングが成功した時に呼ばれるコールバック
    public override void OnJoinedRoom()
    {
        PhotonNetwork.IsMessageQueueRunning = false;
        Debug.Log("PUN / OnJoinedRoom");
        PhotonNetwork.LoadLevel("PUNFootball");
    }

    #endregion
}

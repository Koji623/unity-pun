using UnityEngine;
using TMPro;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityStandardAssets.Cameras;
public class GameController : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Transform[] respawn = null;

    [SerializeField]
    private LookatTarget targetcam = null;

    public TMP_Text greenScoreText = null;

    public TMP_Text redScoreText = null;

    private int redScore;
    private int greenScore;

    public int ClearScore = 10;

    public TMP_Text game_Status;

    private BallScript ball;


    void Start()
    {
        PhotonNetwork.IsMessageQueueRunning = true;
        Invoke("GameStart", 0.5f);
    }

    public void GameStart()
    {
        CreatePlayer(GetAssignForTeamColor());
        CheckSphereActive();
        GetScore();
    }

    //プレイヤーを作成しチームにアサイン
    public void CreatePlayer(Color color)
    {
        //生成位置
        int respawnNumber = 0;
        if (color == Color.red)
        {
            //Redチーム
            respawnNumber = (int)UnityEngine.Random.Range(0f, 2f);
        }
        else
        {
            //Greenチーム
            respawnNumber = (int)UnityEngine.Random.Range(2f, 4f);
        }
        GameObject player = PhotonNetwork.Instantiate("PhotonThirdPersonController", respawn[respawnNumber].position, Quaternion.identity);
        player.GetComponent<CharacterControllerScript>().SetTeamColor(color);

        player.SetActive(true);
        targetcam.SetTarget(player.transform);
    }

    //既存プレイヤーの数が少ないチームカラーを取得
    Color GetAssignForTeamColor()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        int redMemberCount = 0, greenMemberCount = 0;
        foreach (GameObject player in players)
        {
            Color color = player.GetComponent<CharacterControllerScript>().GetTeamColor();
            if (color == Color.red)
            {
                redMemberCount++;
            }
            else if (color == Color.green)
            {
                greenMemberCount++;
            }
        }
        if (redMemberCount < greenMemberCount)
        {
            return Color.red;
        }
        else
        {
            return Color.green;
        }

    }

    //ボールがなかったら作成する
    private void CheckSphereActive()
    {
        bool ball_status = true;
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("Ball_status", out object ball_status_Object))
        {
            ball_status = (bool)ball_status_Object;
        }
        if (!ball_status)
        {
            var properties = new Hashtable();
            properties["Ball_status"] = true;
            PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
            CreateSphere();
        }
    }

    //ボールオブジェクトの生成
    private void CreateSphere()
    {
        var v = new Vector3(-0.43f, 2.7f, 9.3f);
        GameObject ballObj = PhotonNetwork.Instantiate("PhotonSphere", v, Quaternion.identity);
        ball = ballObj.GetComponent<BallScript>();
        //イベント登録
        ball.action = CalculateScore;
    }

    //得点計算(自身が生成したボールオブジェクトがゴールしたときのみ呼ばれる)
    private void CalculateScore(string goalColor)
    {
        if (goalColor == "Green")
        {
            greenScore++;
        }
        else if (goalColor == "Red")
        {
            redScore++;
        }
        var properties = new Hashtable();
        properties["greenScore"] = greenScore;
        properties["redScore"] = redScore;
        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        CreateSphere();
    }


    //カスタムプロパティが更新されたときにコールバックされる
    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        // 更新されたキーと値のペアを、デバッグログに出力する
        foreach (var p in changedProps)
        {
            Debug.Log($"{p.Key}: {p.Value}");
        }
        GetScore();
        check_clear();
    }

    //カスタムプロパティからスコアを取得しスコアボードに反映
    void GetScore()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("redScore", out object redScoreObject))
        {
            redScore = (int)redScoreObject;
            redScoreText.text = redScore.ToString();
        }
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("greenScore", out object greenScoreObject))
        {
            greenScore = (int)greenScoreObject;
            greenScoreText.text = greenScore.ToString();
        }
    }

    //ゲーム終了判定
    void check_clear()
    {
        if (redScore >= ClearScore)
        {
            game_Status.text = "RED Team WIN ! \n return to Title...";
            Invoke("RestartGame", 5f);
        }
        else if (greenScore >= ClearScore)
        {
            game_Status.text = "GREEN Team WIN! \n return to Title...";
            Invoke("RestartGame", 5f);
        }
    }

    //ルームから退出する
    public void RestartGame()
    {
        targetcam.SetTarget(this.transform);
        PhotonNetwork.DestroyAll();
        PhotonNetwork.LeaveRoom();
    }

    //ルームを退出したときにコールバックされる
    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("TitleScene");
    }
}

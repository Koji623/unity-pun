using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
public class BallScript : MonoBehaviourPunCallbacks
{
    private bool isGoal = false;

    void OnCollisionEnter(Collision other)
    {
        if (photonView.IsMine && !isGoal)
        {
            if (other.gameObject.tag == "Goal_Green")
            {
                GameObject.FindWithTag("GameController").GetComponent<GameController>().CalculateScore("Red");
                photonView.RPC("PutGoalMessage", RpcTarget.All, "Red");
            }
            else if (other.gameObject.tag == "Goal_Red")
            {
                GameObject.FindWithTag("GameController").GetComponent<GameController>().CalculateScore("Green");
                photonView.RPC("PutGoalMessage", RpcTarget.All, "Green");

            }
            else
            {
                return;
            }
            isGoal = true;
            Invoke("DestroyMine", 1f);
        }
    }

    void DestroyMine()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    void PutGoalMessage(string teamColor)
    {
        GameObject.FindWithTag("GameController").GetComponent<GameController>().goalAction(teamColor);
    }
}

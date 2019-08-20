using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(Rigidbody))]
public class BallScript : MonoBehaviourPunCallbacks
{
    //GameControllerで登録するイベント
    public UnityAction<string> action;
    private bool isGoal = false;

    void OnCollisionEnter(Collision other)
    {
        if (photonView.IsMine && !isGoal)
        {
            if (other.gameObject.tag == "Goal_Green")
            {
                action("Red");
            }
            else if (other.gameObject.tag == "Goal_Red")
            {
                action("Green");
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
}

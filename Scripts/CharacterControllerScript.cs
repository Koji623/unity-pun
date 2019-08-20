using UnityEngine;
using Photon.Pun;
using TMPro;

public class CharacterControllerScript : MonoBehaviourPunCallbacks, IPunObservable
{

    [SerializeField]
    private TMP_Text nameLabel = null;

    [SerializeField]
    private GameObject uwagiObj = null;

    private Color teamColor;

    void Start()
    {
        nameLabel.text = this.photonView.Owner.NickName;
    }

    public void SetTeamColor(Color color)
    {
        teamColor = color;
        nameLabel.color = color;
        ChangeMaterialColor(color);
    }

    public Color GetTeamColor()
    {
        return teamColor;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(teamColor.r);
            stream.SendNext(teamColor.g);
            stream.SendNext(teamColor.b);
            stream.SendNext(teamColor.a);
        }
        else
        {
            float r = (float)stream.ReceiveNext();
            float g = (float)stream.ReceiveNext();
            float b = (float)stream.ReceiveNext();
            float a = (float)stream.ReceiveNext();
            SetTeamColor(new Vector4(r, g, b, a));
        }
    }

    //マテリアルの色を変更
    void ChangeMaterialColor(Color color)
    {
        Material preMaterial = uwagiObj.GetComponent<SkinnedMeshRenderer>().material;
        Material newMaterial = new Material(preMaterial);
        newMaterial.color = color;
        uwagiObj.GetComponent<SkinnedMeshRenderer>().material = newMaterial;
    }
}
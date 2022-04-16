using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;

    GameObject Controller;
    public LvlManager lm;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (PV.IsMine)
        {
            lm = GameObject.Find("LevelManager").GetComponent<LvlManager>();
            CreateController();
            
        }
    }
    
    void CreateController()
    {
        int spawn = Random.Range(0, lm.GetComponent<LvlManager>().spawnPositions.Length);
       
        Controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerController"), lm.GetComponent<LvlManager>().spawnPositions[spawn].position, Quaternion.identity, 0,new object[] { PV.ViewID});
    }

    public void Die()
    {
        PhotonNetwork.Destroy(Controller);
        CreateController();
    }
}

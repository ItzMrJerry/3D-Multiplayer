using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using ExitGames.Client.Photon;
using Photon.Realtime;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [Header("Objects")]
    [SerializeField] GameObject cameraHolder;

    [SerializeField] float mouseSens, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [Header("Audio")]
    public float FootStepDelay = 0.1f;
    public AudioSource aud;
    private bool FootStepDone = true;

    [Header("Item")]
    [SerializeField] Item[] items;

    int itemIndex;
    int previousItemIndex = -1;

    float verticalLookRotation;
    bool isgrounded;
    Vector3 SmoothMoveVelocity;
    Vector3 moveAmount;
    Vector3 moveDir;

    Rigidbody rb;

    PhotonView PV;

    const float maxHealth = 100f;
    float currentHealth = maxHealth;

    PlayerManager PlayerManager;


    private void Awake()
    {
    
        rb = GetComponent<Rigidbody>();

        PV = GetComponent<PhotonView>();


        PlayerManager = PhotonView.Find((int)PV.InstantiationData[0]).GetComponent<PlayerManager>();
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            EquipItem(0);
        }
        else
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
    }

    private void Update()
    {
        
        if (isgrounded && moveDir != new Vector3(0, 0, 0) && !aud.isPlaying && FootStepDone)
        {
            StartCoroutine(StepSound());
        }
        if (!PV.IsMine) return;

        Look();

        Move();

        Jump();

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()))
            {
                EquipItem(i);
                break;
            }
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0f)
        {
            if (itemIndex >= items.Length - 1) EquipItem(0);

            else EquipItem(itemIndex + 1);

        }else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0f)
        {
            if (itemIndex <= 0) EquipItem(items.Length - 1);

            else EquipItem(itemIndex - 1);
        }

        if (Input.GetMouseButtonDown(0))
        {
            items[itemIndex].Use();
        }



       
    }

    IEnumerator StepSound()
    {
        FootStepDone = false;
        aud.volume = Random.Range(0.05f, 0.15f);
        aud.pitch = Random.Range(0.8f, 1.1f);
        aud.Play();
        yield return new WaitForSeconds(FootStepDelay);
        FootStepDone = true;
    }

    private void Move()
    {
        moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref SmoothMoveVelocity, smoothTime);
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isgrounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }

    private void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSens);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSens;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);


        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    public void SetGroundedSate(bool _grounded)
    {
        isgrounded = _grounded;
    }

    private void FixedUpdate()
    {
        if (!PV.IsMine) return;
        rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.deltaTime);
    }

    void EquipItem(int _index)
    {
        if (_index == previousItemIndex) return;
        itemIndex = _index;

        items[itemIndex].itemGameobject.SetActive(true);

        if (previousItemIndex != -1)
        {
            items[previousItemIndex].itemGameobject.SetActive(false);
        }
        previousItemIndex = itemIndex;

        if (PV.IsMine)
        {
            Hashtable hash = new Hashtable();
            hash.Add("itemIndex",itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            EquipItem((int)changedProps["itemIndex"]);
        }
    }

    public void TakeDamage(float damage)
    {
        PV.RPC("RPC_TakeDamge", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamge(float damage)
    {
        if (!PV.IsMine) return;
        Debug.Log("Took Damage: " + damage);
        currentHealth -= damage;

        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        PlayerManager.Die();
    }
}

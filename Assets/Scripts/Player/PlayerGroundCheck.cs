using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playercontroller;

    private void Awake()
    {
        playercontroller = GetComponentInParent<PlayerController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == playercontroller.gameObject) return;
        playercontroller.SetGroundedSate(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == playercontroller.gameObject) return;
        playercontroller.SetGroundedSate(false);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject == playercontroller.gameObject) return;
        playercontroller.SetGroundedSate(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == playercontroller.gameObject) return;
        playercontroller.SetGroundedSate(true);
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == playercontroller.gameObject) return;
        playercontroller.SetGroundedSate(false);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == playercontroller.gameObject) return;
        playercontroller.SetGroundedSate(true);
    }
}

using FishNet.Object;
using System;
using TriggerTactics.Gameplay;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;
using Cursor = UnityEngine.Cursor;

public class PlayerController : NetworkBehaviour
{

    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;
    float rotationX = 0;

    [SerializeField]
    private float cameraYOffset = 1.5f;
    private Camera playerCamera;

    public override void OnStartClient()
    {
        base.OnStartClient();
        GameManager.UpdateUIPlayer();
        if (base.IsOwner)
        {
            playerCamera = Camera.main;
            playerCamera.transform.position = new Vector3(transform.position.x, transform.position.y + cameraYOffset, transform.position.z);
            playerCamera.transform.SetParent(transform);
        }
        else
        {
            gameObject.GetComponent<PlayerController>().enabled = false;
        }
    }

    void Start()
    {
        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        if (GameManager.IsMyTurn(OwnerId))
        {
            if(Input.GetMouseButtonDown(MouseButton.RightMouse.GetHashCode()))
            {
                GameManager.PlayerSelectTarget(OwnerId);
            }
            else if(Input.GetMouseButtonDown(MouseButton.LeftMouse.GetHashCode()))
            {
                GameManager.PlayerShoot(OwnerId);
            }
        }
        // Player and Camera rotation
        if (playerCamera != null)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        }
    }
}

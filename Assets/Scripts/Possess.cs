using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Possess : MonoBehaviour
{
    public CinemachineFreeLook freeLook;
    public ObjectOutliner objOutlined;
    public BoxCollider objBCol;
    public Transform ghostyLookAt;
    public Transform ghostObj;
    public ThirdPersonCam thirdPersonCam;
    public Rigidbody objRB;
    public Rigidbody ghostRB;
    public Outline pOutline;
    public GhostMovement ghostMove;
    public GameObject orientation;
    public bool isPossessing = false;
    public bool cooldownOver = true;

    void Update()
    {
        if (objOutlined.CurrentOutlinedObject != null)
        {
            Debug.Log(objOutlined.CurrentOutlinedObject.name);

            if (Input.GetKeyDown(KeyCode.F) && !isPossessing && cooldownOver)
            {
                StartPossessing();
            }

            if (isPossessing)
            {
                UpdatePossessing();
            }
        }
        else
        {
            Debug.Log("No object outlined right now");
        }

        if (Input.GetKeyDown(KeyCode.R) && isPossessing)
        {
            EndPossessing();
        }
    }

    void StartPossessing()
    {
        isPossessing = true;

        GameObject objOutlinedChild = objOutlined.CurrentOutlinedObject.transform.GetChild(0).gameObject;
        objRB = objOutlined.CurrentOutlinedObject.GetComponent<Rigidbody>();
        pOutline = objOutlined.CurrentOutlinedObject.GetComponent<Outline>();

        PlayerMovement pMove;
        if (!objOutlined.CurrentOutlinedObject.TryGetComponent<PlayerMovement>(out pMove))
        {
            pMove = objOutlined.CurrentOutlinedObject.AddComponent<PlayerMovement>();
        }

        pMove.enabled = true;

        freeLook.LookAt = objOutlined.CurrentOutlinedObject.transform;
        freeLook.Follow = objOutlined.CurrentOutlinedObject.transform;

        objOutlined.CurrentOutlinedObject.gameObject.tag = "Player";
        ghostMove.enabled = false;

        gameObject.tag = "Untagged";

        pOutline.OutlineWidth = 4f;
        pOutline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
        pOutline.enabled = true;

        thirdPersonCam.player = objOutlined.CurrentOutlinedObject.transform;
        thirdPersonCam.playerObj = objOutlinedChild.transform;
        thirdPersonCam.rb = objRB;

        ghostRB.isKinematic = true;

        pMove.orientation = orientation.transform;
        pMove.moveSpeed = 7f;
        pMove.groundDrag = 4f;
        pMove.jumpForce = 6f;
        pMove.jumpCooldown = 0.25f;
        pMove.airMultiplier = 0.4f;
        pMove.playerHeight = 1f;
        pMove.whatIsGround = LayerMask.GetMask("whatIsGround");

        objOutlined.enabled = false;
    }

    void EndPossessing()
    {
        GameObject objOutlinedChild = objOutlined.CurrentOutlinedObject.transform.GetChild(0).gameObject;
        PlayerMovement pMove = objOutlined.CurrentOutlinedObject.gameObject.GetComponent<PlayerMovement>();
        objRB = objOutlined.CurrentOutlinedObject.GetComponent<Rigidbody>();
        pOutline = objOutlined.CurrentOutlinedObject.GetComponent<Outline>();

        isPossessing = false;

        freeLook.LookAt = ghostyLookAt;
        freeLook.Follow = ghostyLookAt;

        objOutlined.CurrentOutlinedObject.gameObject.tag = "Possessable";
        ghostMove.enabled = true;

        gameObject.tag = "Player";

        pOutline.OutlineWidth = 2f;
        pOutline.OutlineMode = Outline.Mode.OutlineAll;
        pOutline.enabled = false;

        thirdPersonCam.player = gameObject.transform;
        thirdPersonCam.playerObj = ghostObj.transform;
        thirdPersonCam.rb = ghostRB;

        ghostRB.isKinematic = false;

        pMove.enabled = false;

        objOutlined.enabled = true;

        StartCoroutine(Cooldown());
    }
    
    IEnumerator Cooldown()
    {
        cooldownOver = false;
        yield return new WaitForSeconds(10f);
        cooldownOver = true;
    }

    void UpdatePossessing()
    {
        if (objOutlined.CurrentOutlinedObject == null) return;

        Transform objOutlinedChild = objOutlined.CurrentOutlinedObject.transform.GetChild(0);
        objOutlinedChild.rotation = Quaternion.Euler(-90, objOutlinedChild.eulerAngles.y, 0);
    }
}

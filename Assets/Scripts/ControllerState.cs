﻿using UnityEngine;
using UnityEngine.UI; //used for stepping so can remove if that is no longer here
using System.Collections;

public class ControllerState : MonoBehaviour
{

    public enum States { Grip, Shoot };

    
    public MeshRenderer StatusSphere;
    public States controllerState;
    public Material GripMaterial;
    public Material ShootMaterial;
    public SteamVR_Controller.Device device;
    public Rigidbody ObjectToPickUp;
    public Rigidbody Holding;




    //end area of locomotion variables

    [HideInInspector]
    public Vector3 prevPos;
    [HideInInspector]
    public Vector3 curPos;
    [HideInInspector]
    public SteamVR_TrackedObject controller;


    public bool canGrip;
    public bool canPickUp;
    public GameObject GripObject;


    // Use this for initialization
    void Start()
    {
        controller = GetComponent<SteamVR_TrackedObject>();
        prevPos = controller.transform.localPosition;
        curPos = controller.transform.localPosition;
        controllerState = States.Grip;
        device = SteamVR_Controller.Input((int)controller.index);
    }

    void Update()
    {
        device = SteamVR_Controller.Input((int)controller.index);
        //prevPos = controller.transform.localPosition;
        prevPos = curPos;
        curPos = controller.transform.localPosition;
    }



    void OnTriggerEnter(Collider other)
    {
        InteractionAttributes ia = other.GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            if (ia.CanClimb)
            {
                canGrip = true;
                GripObject = other.gameObject;
            } else if (ia.CanPickUp)
            {
                canPickUp = true;
                ObjectToPickUp = ia.ObjectToPickUp; //TODO if this is null, then assignt PickUpObject as other
            }
        }
        


    }

    void OnTriggerExit(Collider other)
    {
        InteractionAttributes ia = other.GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            if (ia.CanClimb)
            {
                canGrip = false;
                GripObject = null;
            }
            else if (ia.CanPickUp)
            {
                canPickUp = false;
                ObjectToPickUp = null;
            }
        }
    }



}


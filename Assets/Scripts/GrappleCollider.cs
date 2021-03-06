﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleCollider : MonoBehaviour {

    public bool Grappled = false; //pretty sure this isn't used adn can be removed
    public bool HookshotFired = false;
    public Rigidbody Body;
    int i = 0;
    private ControllerState controller;
    private float GrappleSpeed;
    private LineRenderer lr;
    IEnumerator coroutineShoot;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter(Collider other)
    {

        InteractionAttributes ia = other.GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            if (ia.CanHookshot)
            {
                this.transform.parent = other.gameObject.transform;
                Grappled = true; //pretty sure this isn't used adn can be removed
                IEnumerator coroutineReelInPlayer;
                coroutineReelInPlayer = ReelInPlayer(controller);
                StartCoroutine(coroutineReelInPlayer);

                StopCoroutine(coroutineShoot);
            }
        }
    }


    public void Shoot(GameObject grappleTarget, ControllerState controller, float GrappleLength, float GrappleSpeed)
    {
        HookshotFired = true;
        UnHideGrapple();


        coroutineShoot = ShootGrapple(grappleTarget, controller, GrappleLength, GrappleSpeed);
        StartCoroutine(coroutineShoot);
    }

    IEnumerator ShootGrapple(GameObject grappleTarget, ControllerState controller2, float GrappleLength, float GrappleSpeed2)
    {

        controller = controller2; // BIG HUGE UGLY HACK. I am just doing this now to see if I can fix a race condition
        GrappleSpeed = GrappleSpeed2; // AND I DID IT AGAIN. Definitely need to think about how to better organize this code. But prototype the hookshot first.

        Vector3 grappleStart = this.transform.position;

        grappleTarget.transform.parent = null;
        this.transform.parent = null;

        float totalTime = Vector3.Distance(grappleStart, grappleTarget.transform.position) / GrappleSpeed;
        float elapsedTime = 0.0f;

        lr = this.GetComponent<LineRenderer>();
        lr.enabled = true;
        lr.SetPosition(0, grappleStart);
        lr.SetPosition(1, this.transform.position);

        while (elapsedTime < totalTime)
        {

            this.transform.position = Vector3.Lerp(grappleStart, grappleTarget.transform.position, elapsedTime / totalTime);
            elapsedTime += Time.deltaTime;
            lr.SetPosition(1, this.transform.position);
            lr.SetPosition(0, controller.controller.transform.position);

            yield return null;
        }

        elapsedTime = 0.0f;



        while (elapsedTime < totalTime)
        {

            this.transform.position = Vector3.Lerp(grappleTarget.transform.position, controller.controller.transform.position, elapsedTime / totalTime);
            elapsedTime += Time.deltaTime;
            lr.SetPosition(1, this.transform.position);
            yield return null;
        }

        HideGrapple();
        lr.enabled = false; //fyi, should refactor this into Hide Grapple

        //Destroy(grappleTarget);
        HookshotFired = false; 
    }

    IEnumerator ReelInPlayer(ControllerState controller)
    {

        Vector3 handToBody = controller.controller.transform.position - Body.transform.position;
        Vector3 bodyStartPosition = Body.transform.position;
        float timeToComplete = Vector3.Distance(bodyStartPosition, this.transform.position) / GrappleSpeed;
        float timeElapsed = 0;


        while ((Body.transform.position - (this.transform.position - controller.controller.transform.position + Body.transform.position)).magnitude > .005) //if there is anything brittle, it is this logic
        {
            Body.transform.position = Vector3.Lerp(bodyStartPosition, this.transform.position - controller.controller.transform.position + Body.transform.position, timeElapsed / timeToComplete); //when hookshotting moving objects, is something happening while this coroutine runs to move collider back to orign?



            timeElapsed += Time.deltaTime;
            lr.SetPosition(0, controller.controller.transform.position);
            yield return null;
        }


        /*
        while ((Body.transform.position - (this.transform.position - handToBody)).magnitude > .001) //if there is anything brittle, it is this logic
        {
            Body.transform.position = Vector3.Lerp(bodyStartPosition, this.transform.position - handToBody, timeElapsed / timeToComplete ); //when hookshotting moving objects, is something happening while this coroutine runs to move collider back to orign?


            Debug.Log("Time: " + timeElapsed + " | Target Position: " + this.transform.position);

            timeElapsed += Time.deltaTime;
            lr.SetPosition(0, controller.controller.transform.position);
            yield return null;
        }
        */
        Grappled = false; //pretty sure this isn't used and can be removed
        HookshotFired = false; // this needs to be refactored into some code common with the case where it doesn't hit anything
        HideGrapple();
        lr.enabled = false;
        this.transform.parent = null;
        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        this.transform.localScale = new Vector3(.05f, .001f, .02f);

    }
    

    void UnHideGrapple()
    {
        this.GetComponent<MeshRenderer>().enabled = true;
        this.GetComponent<SphereCollider>().enabled = true;
    }

    void HideGrapple()
    {

        this.GetComponent<MeshRenderer>().enabled = false;
        this.GetComponent<SphereCollider>().enabled = false;
        //this.transform.position = new Vector3(0.0f, 0.0f, 0.0f); //this is a hack to see if the reason we can detach ourselves from blcoks is the continued presence of the grapple
    }

}

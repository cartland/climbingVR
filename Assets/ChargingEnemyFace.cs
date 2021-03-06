﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemyFace : MonoBehaviour
{

    public ChargingEnemy ce;
    public GameObject pivot;
    public float faceRotateSpeed;
    private bool trackPlayerIsRunning;

    public Vector3 PlayerDirection;
    public GameObject Player;
    private Quaternion rot;

    public void Start()
    {
        IEnumerator trackPlayer = TrackPlayer();
        StartCoroutine(trackPlayer);
    }


    public void Update()
    {
        if (ce.attack && !ce.IsStunned)
        {
            trackPlayerIsRunning = true;
            float step = faceRotateSpeed * Time.deltaTime;
            pivot.transform.localRotation = Quaternion.RotateTowards(pivot.transform.localRotation, rot, step);
            //pivot.transform.eulerAngles = new Vector3(pivot.transform.rotation.eulerAngles.x,
                //pivot.transform.rotation.eulerAngles.y, pivot.transform.rotation.eulerAngles.z);
            pivot.transform.localEulerAngles = new Vector3(Mathf.Clamp(pivot.transform.localRotation.eulerAngles.x, -15, 15),
            Mathf.Clamp(pivot.transform.localRotation.eulerAngles.y, -15, 15), Mathf.Clamp(pivot.transform.localRotation.eulerAngles.z, -15, 15));
        }
        else
        {
            trackPlayerIsRunning = false;
        }
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("GT hit: " + other.gameObject.name);
            ///other.transform.position += chargingVelocity * ChargingDirection * Time.deltaTime;
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();


            IEnumerator StunPlayerCoroutine = StunPlayer(rb);

            StartCoroutine(StunPlayerCoroutine);
        }
    }

    IEnumerator StunPlayer(Rigidbody rb)
    {
        rb.isKinematic = false;
        yield return new WaitForSeconds(2.0f);
        rb.isKinematic = true;
        rb.transform.rotation = Quaternion.identity;
    }

    private IEnumerator TrackPlayer()
    {
        while (true)
        {
            if (trackPlayerIsRunning)
            {
                UpdateTarget();
                //Quaternion rot = Quaternion.LookRotation(playerDirection);
            }
            yield return new WaitForSeconds(.2f);
        }
    }

    private void UpdateTarget()
    {
        PlayerDirection = (pivot.transform.InverseTransformDirection(Player.transform.position) - pivot.transform.position);

        rot = Quaternion.LookRotation(PlayerDirection);

    }


}
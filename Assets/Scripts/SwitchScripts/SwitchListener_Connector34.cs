﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchListener_Connector34 : MonoBehaviour
{

    public SwitchControl sc;

    public ScriptedMovement sm;

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (sc.activated)
        {
            Activate();
        }
    }

    void Activate()
    {
        sm.EnableMovement = true;
    }

}


﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public int health = 500;
    public float fireRate = 100f;

    public bool disableAutoFire = false;
    public GameObject bullet;
    public List<Transform> bulletSpawns = new List<Transform>();
    public bool alternateFire;

    protected GameObject[] players;
    protected GameObject targetPlayer;

    protected float lastShot = 0f;
    protected int lastSpawn = 0;

    // Update is called once per frame
    protected void Update()
    {
        if (players == null)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
        }

        AcquireTarget();
    }

    protected virtual void AcquireTarget() {}

    protected void GetClosestPlayer(List<GameObject> targets)
    {
        float distanceTarget1 = Vector2.Distance(targets[0].transform.position, transform.position);
        float distanceTarget2 = Vector2.Distance(targets[1].transform.position, transform.position);

        if (distanceTarget1 < distanceTarget2)
        {
            targetPlayer = targets[0];
        }
        else
        {
            targetPlayer = targets[1];
        }
    }

    protected virtual void FixedUpdate()
    {
        if (!disableAutoFire && targetPlayer != null)
        {
            AutoFire();
        }
    }

    protected virtual void AutoFire()
    {
        if (bullet != null)
        {
            if (Time.time > (60f/fireRate) + lastShot)
            {
                if (!alternateFire)
                {
                    for (int i = 0; i < bulletSpawns.Count; i++)
                    {
                        Transform bulletSpawn = bulletSpawns[i];
                        Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                        lastShot = Time.time;
                    }
                } else
                {
                    lastSpawn = (lastSpawn + 1) % bulletSpawns.Count;
                    Transform bulletSpawn = bulletSpawns[lastSpawn];
                    Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
                    
                }
                lastShot = Time.time;
            }
        }
        else
        {
            Debug.LogError("Fort is trying to fire, but it has no bullet assigned!");
        }
    }
}
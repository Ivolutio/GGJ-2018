﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public GameObject Ship1Prefab;
    public GameObject Ship2Prefab;
    public Vector2 Ship1Spawn;
    public Vector2 Ship2Spawn;
    public GameObject ShipTetherPrefab;

    public float moveSpeed = 3f;
    public float rotateSpeed = 3f;

    public Ship ship1;
    public Ship ship2;
    public ShipTether tether;

    private Vector2 p1movement;
    private Vector2 p2movement;
    private Vector2 p1rotation;
    private Vector2 p2rotation;

    public float teleportDistance = 5f;
    public float fullyChargedValue = 3f;
    public float chargeSpeed = 1f;

    private float p1charge;
    private float p2charge;

    private float minX, minY, maxX, maxY;

    void Start ()
    {
        //Spawn players	
        ship1 = Instantiate(Ship1Prefab, Ship1Spawn, Quaternion.identity).GetComponent<Ship>();
        ship2 = Instantiate(Ship2Prefab, Ship2Spawn, Quaternion.identity).GetComponent<Ship>();

        //Spawn tether
        tether = Instantiate(ShipTetherPrefab).GetComponent<ShipTether>();

        tether.ship1 = ship1;
        tether.ship2 = ship2;
    }

    void SetGameBounds()
    {
        Vector2 bottomCorner = Camera.main.ViewportToWorldPoint(Camera.main.transform.position);
        Vector2 topCorner = Camera.main.ViewportToWorldPoint(new Vector3(1,1,Camera.main.transform.position.z));

        minX = bottomCorner.x;
        minY = bottomCorner.y;
        maxX = topCorner.x;
        maxY = topCorner.y;

        Transform shipTransform = ship1.transform;
        
        for (int i = 0; i < 2; i++)
        {
            if (i == 1)
            {
                shipTransform = ship2.transform;
            }

            Vector3 pos = shipTransform.position;
            // Horizontal contraint
            if (pos.x < minX) pos.x = minX;
            if (pos.x > maxX) pos.x = maxX;

            // vertical contraint
            if (pos.y < minY) pos.y = minY;
            if (pos.y > maxY) pos.y = maxY;

            // Update position
            shipTransform.position = pos;

        }        
    }
	
	void Update ()
    {
        SetGameBounds();
        
        //Teleport
        if (tether.line.enabled)
        {
            ///Get input
            int p1tp = Mathf.CeilToInt(Input.GetAxisRaw("P1Teleport"));
            int p2tp = Mathf.CeilToInt(Input.GetAxisRaw("P2Teleport"));
            if (p1tp == 1)
            {
                p2charge = 0f;
                p1charge += chargeSpeed * Time.deltaTime;
            }
            else if (p2tp == 1)
            {
                p1charge = 0f;
                p2charge += chargeSpeed * Time.deltaTime;
            }
            else if (p1charge >= fullyChargedValue)
            {
                //Teleport
                ///get direction
                float angle = ship1.transform.eulerAngles.z;
                Vector3 direction = new Vector3(Mathf.Sin(-Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
                Vector3 targetPos = ship1.transform.position + direction * teleportDistance;
                float shipSize = ship1.coll.radius;
                ///cast that point for collisions
                Collider2D hit = Physics2D.OverlapCircle(targetPos, shipSize - .2f);
                if(hit != null)
                {
                    targetPos = ship1.transform.position + direction * (Mathf.Abs(hit.Distance(ship1.coll).distance/2f));
                }
                ///teleport
                ship1.transform.position = targetPos;
                p1charge = 0f;
            }
            else if (p2charge >= fullyChargedValue)
            {
                //Teleport
                ///get direction
                float angle = ship2.transform.eulerAngles.z;
                Vector3 direction = new Vector3(Mathf.Sin(-Mathf.Deg2Rad * angle), Mathf.Cos(Mathf.Deg2Rad * angle));
                Vector3 targetPos = ship2.transform.position + direction * teleportDistance;
                float shipSize = ship2.coll.radius;
                ///cast that point for collisions
                Collider2D hit = Physics2D.OverlapCircle(targetPos, shipSize - .2f);
                if (hit != null)
                {
                    targetPos = ship2.transform.position + direction * (Mathf.Abs(hit.Distance(ship2.coll).distance / 2f));
                }
                ///teleport
                ship2.transform.position = targetPos;
                p2charge = 0f;
            }
            else
            {
                p1charge = 0f;
                p2charge = 0f;
            }
        }
        else
        {
            p1charge = 0f;
            p2charge = 0f;
        }
    }

    private void FixedUpdate()
    {
        p1movement = new Vector2(Input.GetAxisRaw("P1MovHor"), Input.GetAxisRaw("P1MovVer"));
        p2movement = new Vector2(Input.GetAxisRaw("P2MovHor"), Input.GetAxisRaw("P2MovVer"));

        p1rotation = new Vector2(Input.GetAxisRaw("P1RotHor"), Input.GetAxisRaw("P1RotVer"));
        p2rotation = new Vector2(Input.GetAxisRaw("P2RotHor"), Input.GetAxisRaw("P2RotVer"));

        ship1.rb.velocity = p1movement.normalized * moveSpeed;
        ship2.rb.velocity = p2movement.normalized * moveSpeed;

        if (p1rotation.x != 0f || p1rotation.y != 0f)
        {
            float angley = Mathf.Atan2(p1rotation.y, p1rotation.x) * Mathf.Rad2Deg;
            angley = (angley + 360f) % 360f;
            float angle = Mathf.LerpAngle(ship1.transform.rotation.eulerAngles.z, angley, rotateSpeed * Time.fixedDeltaTime);
            ship1.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        }
        if (p2rotation.x != 0f || p2rotation.y != 0f)
        {
            float angley = Mathf.Atan2(p2rotation.y, p2rotation.x) * Mathf.Rad2Deg;
            angley = (angley + 360f) % 360f;
            float angle = Mathf.LerpAngle(ship2.transform.rotation.eulerAngles.z, angley, rotateSpeed * Time.fixedDeltaTime);
            ship2.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        }
    }
}

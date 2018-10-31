using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : Movement
{

    public Grid grid; //  You can also use the Tilemap object
    public GameObject gameManager;

    private Vector2 startPos;
    private Vector2 endPos;
    private Vector2[] thePath;
    private Pathfinder pathfinderScript;
    private int currentStep;
    private bool moving;

    protected override void Start()
    {
        pathfinderScript = gameManager.GetComponent<Pathfinder>();
        thePath = new Vector2[0];
        base.Start();
    }

    public void Update()
    {
        if (thePath.Length > 0 && currentStep >= 0)
        {
            moving = true;
            if (gameObject.transform.position.x == thePath[currentStep].x && gameObject.transform.position.y == thePath[currentStep].y)
                currentStep--;
            else
                AttemptMove<Player>(thePath[currentStep]);
        }
        else
            moving = false;

        if (Input.GetMouseButtonUp(0)) //Left Click
        {
            if (moving)
            {
                AttemptMove<Player>(thePath[currentStep]);
                currentStep = -1;
                return;
            }
            
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int coordinate = grid.WorldToCell(mouseWorldPos);
            
            startPos = gameObject.transform.position;
            endPos.Set(coordinate.x +1,coordinate.y+1);
            
            thePath = pathfinderScript.GetPathList(startPos, endPos);
            
            currentStep = thePath.Length-1;
        }
    }

    protected override void AttemptMove<T>(Vector2 step)
    {
        /*
        if (gameObject.transform.position.Equals(thePath[currentStep]))
            currentStep--;

        if (currentStep >= 0)
        */

        base.AttemptMove<T>(step);
    }


    }

using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Snake : MonoBehaviour
{
    public int xSize, ySize;
    public GameObject block;
    private GameObject head;
    private GameObject food;
    public Material headMaterial, tailMaterial, foodMaterial;
    List<GameObject> tail;
    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private bool isAlive = true;

    [SerializeField] private float swipeThreshold = 50f;

    private Vector2 dir;
    private float passedTime, timeBetweenMovements;
    
    private void Start()
    {
        timeBetweenMovements = 0.2f;
        dir = Vector2.right;
        CreateGrid();
        CreatePlayer();
        SpawnFood();
        block.SetActive(false);
    }

    private Vector2 GetRandomPos()
    {
        return new Vector2(Random.Range(-xSize/2+1, xSize/2), Random.Range(-ySize/2+1, ySize/2));
    }

    private bool ContainedInSnake(Vector2 spawnPos)
    {
        bool isInHead = spawnPos.x == head.transform.position.x && spawnPos.y == head.transform.position.y;
        bool isInTail = false;
        foreach (var item in tail)
        {
            if(item.transform.position.x == spawnPos.x && item.transform.position.y == spawnPos.y)
            {
                isInTail = true; 
            }
        }
        return isInHead || isInTail;   
    }

    private void SpawnFood()
    {
        Vector2 spawnPos = GetRandomPos();
        while (ContainedInSnake(spawnPos))
        {
            spawnPos = GetRandomPos();
        }
        food = Instantiate(block);
        food.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);
        food.GetComponent<MeshRenderer>().material = foodMaterial;
        food.SetActive(true);
    }
    private void CreatePlayer()
    {
        head = Instantiate(block) as GameObject;
        head.GetComponent<MeshRenderer>().material = headMaterial;
        tail = new List<GameObject>();
    }

    private void CreateGrid()
    {
        for(int i = 0; i <= xSize; i++)
        {
            GameObject borderBottom = Instantiate(block) as GameObject;
            borderBottom.GetComponent<Transform>().position = new Vector3(i-xSize/2, -ySize/2, 0);

            GameObject borderTop = Instantiate(block) as GameObject;
            borderTop.GetComponent<Transform>().position = new Vector3(i - xSize / 2, ySize-ySize / 2, 0);
        }

        for(int i = 0; i <= ySize; i++)
        {
            GameObject borderRight = Instantiate(block) as GameObject;
            borderRight.GetComponent<Transform>().position = new Vector3(-xSize/2, i-(ySize/2), 0);

            GameObject borderLeft = Instantiate(block)as GameObject;
            borderLeft.GetComponent<Transform>().position = new Vector3(xSize-(xSize/2), i-(ySize/2), 0);
        }
    }

    private void DetectSwipe()
    {
        Vector2 swipeDelta = endTouchPos - startTouchPos;

        if (swipeDelta.magnitude > swipeThreshold)
        {           
            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                if (swipeDelta.x > 0)
                {
                    Debug.Log("Swipe Right");
                    dir = Vector2.right;
                }

                else
                {
                    Debug.Log("Swipe Left");
                    dir = Vector2.left;
                }
                    
            }
            else
            {
                if (swipeDelta.y > 0)
                {
                    Debug.Log("Swipe Up");
                    dir = Vector2.up;
                }

                else
                {
                    Debug.Log("Swipe Down");
                    dir = Vector2.down;
                }
                    
            }
        }
    }

    private void GameOver()
    {
        isAlive = false;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    startTouchPos = touch.position;
                    break;

                case TouchPhase.Ended:
                    endTouchPos = touch.position;
                    DetectSwipe();
                    break;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            startTouchPos = Input.mousePosition;
        }
            
        if (Input.GetMouseButtonUp(0))
        {
            endTouchPos = Input.mousePosition;
            DetectSwipe();
        }

        passedTime += Time.deltaTime;
        if(timeBetweenMovements < passedTime && isAlive)
        {
            passedTime = 0;

            //move
            Vector3 newPosition = head.GetComponent<Transform>().position + new Vector3(dir.x, dir.y, 0);

            //check collision border
            if (newPosition.x >= xSize/2 || newPosition.x <= -xSize/2 || newPosition.y >= ySize/2 || newPosition.y <= -ySize / 2)
            {
                GameOver();
            }

            //check collision with tail
            foreach (var item in tail)
            {
                if(item.transform.position == newPosition)
                {
                    GameOver();
                }
            }
            if(newPosition.x == food.transform.position.x && newPosition.y == food.transform.position.y)
            {
                GameObject newTile = Instantiate(block);
                newTile.SetActive(true);
                newTile.transform.position = food.transform.position;
                DestroyImmediate(food);
                head.GetComponent<MeshRenderer>().material = tailMaterial;
                tail.Add(head);
                head = newTile;
                head.GetComponent<MeshRenderer>().material = headMaterial;
                //when questions are added, incorrect answers will start a for each loop where it will run the code above for the equivalent of whatever the incorrect in a row streak is at
                SpawnFood();
            }
            else
            {
                if (tail.Count == 0)
                {
                    head.transform.position = newPosition;
                }
                else
                {
                    head.GetComponent<MeshRenderer>().material = tailMaterial;
                    tail.Add(head);
                    head = tail[0];
                    head.GetComponent<MeshRenderer>().material = headMaterial;
                    tail.RemoveAt(0);
                    head.transform.position = newPosition;
                }
            }
            
        }
    }
}

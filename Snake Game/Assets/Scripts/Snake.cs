using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Collections;

public class Snake : MonoBehaviour
{
    public int xSize, ySize;
    public GameObject block;
    //public GameObject borderBlock;
    private GameObject head;
    //public Sprite headSprite;
    private GameObject food;
    private GameObject cactus;
    List<GameObject> activeCacti;
    public Material headMaterial, tailMaterial, foodMaterial;
    public Sprite headSprite, tailSprite, foodSprite, borderSprite, cactusSprite, grownCactusSprite;
    List<GameObject> tail;
    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    private bool isAlive = true;
    private bool up = false;
    private bool down = false;
    private bool left = false;
    private bool right = true;
    private int headRotation = 0;
    public float cactusSpawnTime = 5f;
    public float cactusGrowTime = 5f;
    [SerializeField] private float swipeThreshold = 50f;

    private Vector2 dir;
    private float passedTime, timeBetweenMovements;
    void Awake()
    {
        if (activeCacti == null)
            activeCacti = new List<GameObject>();
    }

    private void Start()
    {
        timeBetweenMovements = 0.2f;
        dir = Vector2.right;
        CreateGrid();
        CreatePlayer();
        SpawnFood();
        StartCoroutine(CactusCoroutine());
        //SpawnCactus();
        block.SetActive(false);
    }

    IEnumerator CactusCoroutine()
    {
        yield return new WaitForSeconds(cactusSpawnTime);
        SpawnCactus();
    }

    IEnumerator CactusGrowCoroutine()
    {
        yield return new WaitForSeconds(cactusGrowTime);
        cactus.GetComponentInChildren<SpriteRenderer>().transform.localScale /= 1.5f;
        cactus.GetComponentInChildren<SpriteRenderer>().sprite = grownCactusSprite;
        if (cactus != null)
        {
            activeCacti.Add(cactus);
            StartCoroutine(CactusCoroutine());
        }        
    }

    private Vector2 GetRandomPos()
    {
        return new Vector2(UnityEngine.Random.Range(-xSize/2+1, xSize/2), UnityEngine.Random.Range(-ySize/2+1, ySize/2));
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

    private bool ContainedInFood(Vector2 spawnPos)
    {
        bool isInFood = spawnPos.x == food.transform.position.x && spawnPos.y == food.transform.position.y;
        
        
        return isInFood;
    }

    private bool ContainedInCactus(Vector2 spawnPos)
    {
        
        if (cactus == null)
        {           
            return false;
        }

        bool isInCactus = false;

        
        if (spawnPos.x == cactus.transform.position.x && spawnPos.y == cactus.transform.position.y)
        {
            isInCactus = true;
        }

        return isInCactus;
    }

    private bool ContainedInGrownCacti(Vector2 spawnPos)
    {
        if (activeCacti == null)
        {
            return false;
        }

        bool isInGrownCacti = false;

        foreach (var item in activeCacti)
        {
            if (item.transform.position.x == spawnPos.x && item.transform.position.y == spawnPos.y)
            {
                isInGrownCacti = true;
            }
        }
        return isInGrownCacti;
    }

    
    private void SpawnFood()
    {
        Vector2 spawnPos = GetRandomPos();
        while (ContainedInSnake(spawnPos) || ContainedInCactus(spawnPos) || ContainedInGrownCacti(spawnPos))
        {
            spawnPos = GetRandomPos();
        }
        food = Instantiate(block);
        food.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);
        food.GetComponentInChildren<SpriteRenderer>().sprite = foodSprite;
        food.GetComponent<MeshRenderer>().enabled = false;
        food.SetActive(true);
    }

    private void SpawnCactus()
    {       
        Vector2 spawnPos = GetRandomPos();
        while (ContainedInSnake(spawnPos) || ContainedInFood(spawnPos) || ContainedInGrownCacti(spawnPos))
        {
            spawnPos = GetRandomPos();
        }
        cactus = Instantiate(block);
        cactus.transform.position = new Vector3(spawnPos.x, spawnPos.y, 0);
        cactus.GetComponentInChildren<SpriteRenderer>().sprite = cactusSprite;
        cactus.GetComponentInChildren<SpriteRenderer>().transform.localScale *= 2f;
        cactus.GetComponent<MeshRenderer>().enabled = false;
        cactus.SetActive(true);
        StartCoroutine(CactusGrowCoroutine());
    }

    private void CreatePlayer()
    {
        head = Instantiate(block) as GameObject;
        head.GetComponentInChildren<SpriteRenderer>().sprite = headSprite;
        head.GetComponent<MeshRenderer>().enabled = false;
        //head.GetComponent<MeshRenderer>().material = headMaterial;
        tail = new List<GameObject>();
    }

    private void CreateGrid()
    {
        for(int i = 0; i <= xSize; i++)
        {
            GameObject borderBottom = Instantiate(block) as GameObject;
            borderBottom.GetComponentInChildren<SpriteRenderer>().sprite = borderSprite;
            borderBottom.GetComponent<MeshRenderer>().enabled = false;
            borderBottom.GetComponent<Transform>().position = new Vector3(i-xSize/2, -ySize/2, 0);

            GameObject borderTop = Instantiate(block) as GameObject;
            borderTop.GetComponentInChildren<SpriteRenderer>().sprite = borderSprite;
            borderTop.GetComponent<MeshRenderer>().enabled = false;
            borderTop.GetComponent<Transform>().position = new Vector3(i - xSize / 2, ySize-ySize / 2, 0);
        }

        for(int i = 0; i <= ySize; i++)
        {
            GameObject borderRight = Instantiate(block) as GameObject;
            borderRight.GetComponentInChildren<SpriteRenderer>().sprite = borderSprite;
            borderRight.GetComponent<MeshRenderer>().enabled = false;
            borderRight.GetComponent<Transform>().position = new Vector3(-xSize/2, i-(ySize/2), 0);

            GameObject borderLeft = Instantiate(block)as GameObject;
            borderLeft.GetComponentInChildren<SpriteRenderer>().sprite = borderSprite;
            borderLeft.GetComponent<MeshRenderer>().enabled = false;
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
                    right = true;
                    up = false;
                    down = false;
                    left = false;
                    head.GetComponentInChildren<Transform>().rotation = Quaternion.Euler(0, 0, 0);
                }

                else
                {
                    Debug.Log("Swipe Left");
                    dir = Vector2.left;
                    left = true;
                    down = false;
                    up = false;
                    right = false;
                    head.GetComponentInChildren<Transform>().rotation = Quaternion.Euler(0, 0, -180);
                }
                    
            }
            else
            {
                if (swipeDelta.y > 0)
                {
                    Debug.Log("Swipe Up");
                    dir = Vector2.up;
                    up = true;
                    down = false;
                    right=false;
                    left = false;
                    head.GetComponentInChildren<Transform>().rotation = Quaternion.Euler(0, 0, 90);
                }

                else
                {
                    Debug.Log("Swipe Down");
                    dir = Vector2.down;
                    down = true;
                    right = false;
                    left = false;
                    up=false;
                    head.GetComponentInChildren<Transform>().rotation = Quaternion.Euler(0, 0, -90);
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
        if (right == true)
        {
            headRotation = 0;
        }

        if (up == true)
        {
            headRotation = 90;
        }

        if (down == true)
        {
            headRotation = -90;
        }

        if (left == true)
        {
            headRotation = -180;
        }

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

            //check collision with cactus
            if (activeCacti != null)
            {
                foreach (var item in activeCacti)
                {
                    if (item == null)
                        continue; 

                    if (item.transform.position == newPosition)
                    {
                        GameOver();
                        break;
                    }
                }
            }

            if (newPosition.x == food.transform.position.x && newPosition.y == food.transform.position.y)
            {
                GameObject newTile = Instantiate(block);
                newTile.SetActive(true);
                newTile.transform.position = food.transform.position;
                DestroyImmediate(food);
                head.GetComponent<MeshRenderer>().enabled = true;
                head.GetComponent<MeshRenderer>().material = headMaterial;
                head.GetComponentInChildren<SpriteRenderer>().sprite = tailSprite;
                tail.Add(head);
                head = newTile;
                head.GetComponentInChildren<SpriteRenderer>().sprite = headSprite;
                head.GetComponentInChildren<Transform>().rotation = Quaternion.Euler(0, 0, headRotation);
                head.GetComponent<MeshRenderer>().enabled = false;
                //head.GetComponent<MeshRenderer>().material = headMaterial;
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
                    head.GetComponentInChildren<SpriteRenderer>().sprite = tailSprite;
                    head.GetComponent<MeshRenderer>().enabled = false;
                    tail.Add(head);
                    head = tail[0];
                    head.GetComponentInChildren<SpriteRenderer>().sprite = headSprite;
                    tail.RemoveAt(0);
                    head.GetComponentInChildren<Transform>().rotation = Quaternion.Euler(0, 0, headRotation);
                    head.transform.position = newPosition;                                                           
                }
            }
            
        }
    }
}

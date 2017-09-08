using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class MineCount
    {
        public static int number;


        public MineCount(int num)
        {
            number = num;
        }
    }
    public class GridCount
    {
        public int rows;
        public int columns;


        public GridCount(int ro, int col)

        {
            rows = ro;
            columns = col;
        }
    }

    public int columns = 10;
    public int rows = 10;
    public int number = 10;
    public GameObject[] clueTiles; //0 to 8, 9 elements
    public GameObject blankTile;
    public GameObject mineTile;
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    public Vector3 tileSize
    {
        get
        {
            
            return blankTile.GetComponent<SpriteRenderer>().bounds.size;
        }
    }

    // Use this for initialization
    void Start()
    {
        
    }

    void InitialiseList()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                gridPositions.Add(new Vector3(x*tileSize.x, y* tileSize.y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = 0; x < columns ; x++)
        {
            for (int y = 0; y < rows ; y++)
            {
                
               GameObject instance = Instantiate(blankTile, new Vector3(x*tileSize.x, y*tileSize.y, 0f), Quaternion.identity) as GameObject;
        
                instance.transform.SetParent(boardHolder);
            }
        }

        for (int i = 0; i < number; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject instance = Instantiate(mineTile, randomPosition, Quaternion.identity);

            instance.transform.SetParent(boardHolder);
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    public void SetupScene()
    {
        InitialiseList();
        BoardSetup();
        
        //LayoutMinesAtRandom();
    }


 
    // Update is called once per frame
    void Update()
    {
        //boardHolder = GameObject.Find("board").transform;
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // get first touch since touch count is greater than zero

            if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
            {
                // get the touch position from the screen touch to world point
                Vector3 touchedPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10));
                // lerp and set the position of the current object to that of the touch, but smoothly over time.
                boardHolder.position += Vector3.Lerp(transform.position, touchedPos, Time.deltaTime);

            }
        }
    }
}
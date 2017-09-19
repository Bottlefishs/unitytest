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
    //public class GridCount
    //{
    //    public int rows;
    //    public int columns;
    //
    //
    //    public GridCount(int ro, int col)
    //
    //    {
    //        rows = ro;
    //        columns = col;
    //    }
    //}

    public int columns;
    public int rows;
    public int mineNumber;
    public GameObject[] clueTiles; //0 to 8, 9 elements
    public GameObject blankTile;
    public GameObject mineTile;
    private Transform boardHolder;
    public List<Vector3> gridPositions = new List<Vector3>();
    public GameObject[,] topGridObjects;
    public GameObject[,] bottomGridObjects;
    public int[,] clueNumbers;

    public Vector3 tileSize
    {
        get
        {
            
            return blankTile.GetComponent<SpriteRenderer>().bounds.size;
        }
    }
    void Awake()//add GUI values in here
    {
        topGridObjects = new GameObject[columns, rows];
        bottomGridObjects = new GameObject[columns, rows];
        clueNumbers = new int[columns, rows];
    }
    // Use this for initialization
    void Start()
    {

    }

    void TopSetCoordinates(GameObject instance, int xPos, int yPos)
    {
        instance.GetComponent<Coordinates>().coordinates.x=xPos;
        instance.GetComponent<Coordinates>().coordinates.y = yPos;
        topGridObjects[xPos, yPos] = instance;
    }
    void BottomSetCoordinates(GameObject instance, int xPos, int yPos)
    {
        instance.GetComponent<Coordinates>().coordinates.x = xPos;
        instance.GetComponent<Coordinates>().coordinates.y = yPos;
        bottomGridObjects[xPos, yPos] = instance;
    }
    void AddOneAroundMine(int xPos, int yPos)
    {
        for (int x = xPos - 1; x <= xPos + 1; x++)  //does it 3 times i think       //not too sure what's going on here
        {
            if (x >= 0 && x < columns)
            {
                for (int y = yPos - 1; y <= yPos + 1; y++)
                {
                    if (y >= 0 && y < rows)
                    {
                        clueNumbers[x, y]++;
                    }

                }
            }
        }
    }


    void InitialiseList()
    {
        for (int x = 0; x < columns; x++)
        {

            for (int y = 0; y < rows; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f)); 
            }
        }
    }
    public void InitialiseList(float xPos, float yPos) //skipping blocks around the block that was hit
    {
        //List<Vector3> gridPositions = new List<Vector3>();
        List<Vector3> skipPositions = new List<Vector3>();
        //float maxX = boardHolder.position.x + columns * tileSize.x;
        float minX = boardHolder.position.x;
        //float maxY = boardHolder.position.y + rows * tileSize.y;
        float minY = boardHolder.position.y;
        int xIntPos = (int)Math.Round((xPos - minX)/tileSize.x);
        int yIntPos = (int)Math.Round((yPos - minY) / tileSize.y);
        for (int x = xIntPos -1; x<=xIntPos+1; x++)  //does it 3 times i think       //not too sure what's going on here
        {
            if (x>=0&& x<columns)
            {
                for (int y = yIntPos - 1; y <= yIntPos + 1; y++)
                {
                    if (y >= 0&& y <rows)//includes min and max
                    {
                        Vector3 current = new Vector3(x, y, 0f);
                        skipPositions.Add(current);
                        //Debug.Log(String.Format("x is {0} y is {1}",x,y) );
                    }
                }
            }
        }
        for (int x = 0; x < columns; x++)
        {

            for (int y = 0; y < rows; y++)
            {
                //Vector3 current = new Vector3(x * tileSize.x+boardHolder.position.x, y * tileSize.y + boardHolder.position.y, 0f);
                Vector3 current = new Vector3(x, y, 0f);
                if (skipPositions.Contains(current))
                {
                    gridPositions.Remove(current);
                }
            }
        }

    }
    void BoardSetup() //board is the blank blocks
    {
        Vector3 currentPosition;
        boardHolder = GameObject.Find("Board").transform;
        for (int i = 0; i < gridPositions.Count; i++)
        {
            currentPosition = gridPositions[i];
            GameObject instance = Instantiate(blankTile, new Vector3(currentPosition.x * tileSize.x, currentPosition.y * tileSize.y, 0f), Quaternion.identity) as GameObject;
            TopSetCoordinates(instance,(int)currentPosition.x, (int)currentPosition.y);
            instance.transform.SetParent(boardHolder);
        }
        Camera camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        boardHolder.position = camera.ScreenToWorldPoint(new Vector3(0f,0f,0f))+new Vector3(tileSize.x/2,tileSize.y/2,1f);
    }

    public void LayoutMines(GameObject blankBlock)// also lays out clues
    {
        Vector3 currentBoardPosition = boardHolder.position;
        for (int i = 0; i < mineNumber; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject instance = Instantiate(mineTile, new Vector3 (randomPosition.x*tileSize.x,randomPosition.y* tileSize.y,0f)+currentBoardPosition, Quaternion.identity); // add offset to boardholder position
            BottomSetCoordinates(instance, (int)randomPosition.x, (int)randomPosition.y);//stores game object in the array
            instance.transform.SetParent(topGridObjects[(int)randomPosition.x, (int)randomPosition.y].transform);
            AddOneAroundMine((int)randomPosition.x, (int)randomPosition.y);
        }
        for (int x = 0; x < clueNumbers.GetLength(0); x++)
        {
            for (int y = 0; y < clueNumbers.GetLength(1); y++)
            {
                if (bottomGridObjects[x, y]==null ||bottomGridObjects[x, y].tag != "Mine")
                    
                {
                    //Debug.Log("hello whats happening");
                    int clueNumber = clueNumbers[x, y];
                    GameObject instance = Instantiate(clueTiles[clueNumber], new Vector3(x * tileSize.x, y * tileSize.y, 0f) + currentBoardPosition, Quaternion.identity);
                    BottomSetCoordinates(instance, x, y);//stores game object in the array
                    instance.transform.SetParent(topGridObjects[x, y].transform);
                }
            }
        }


    }

    Vector3 RandomPosition() //gives you a random position and removes from the list to make sure it doesn't repeat
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Debug.Log("index" + randomIndex + "size:" + gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    public void SetupScene()
    {
        InitialiseList();
        //TODO make it so it only initialises on the first tap and have to 
        BoardSetup();
        

    }



    // Update is called once per frame
    void Update()
    {
    }
 
}
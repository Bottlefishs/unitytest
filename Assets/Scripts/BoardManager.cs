﻿using UnityEngine;
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
    public List<Vector2> totalZeroPosition = new List<Vector2>();
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

    }
    // Use this for initialization
    void Start()
    {

    }

    void TopSetCoordinates(GameObject instance, int xPos, int yPos)//stores gameobject into topgridobjects
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
    void AddOneAroundMine(int xPos, int yPos)//in clue numbers
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
    public void InitialiseList(GameObject block) //skipping blocks around the block that was hit
    {
        int xIntPos = (int)block.GetComponent<Coordinates>().coordinates.x;
        int yIntPos = (int)block.GetComponent<Coordinates>().coordinates.y;
        for (int x = xIntPos -1; x<=xIntPos+1; x++)  //does it 3 times i think       //not too sure what's going on here
        {
            if (x>=0&& x<columns)
            {
                for (int y = yIntPos - 1; y <= yIntPos + 1; y++)
                {
                    if (y >= 0&& y <rows)//includes min and max
                    {
                        Vector3 current = new Vector3(x, y, 0f);
                        //skipPositions.Add(current);
                        gridPositions.Remove(current);
                        //Debug.Log(String.Format("x is {0} y is {1}",x,y) );
                    }
                }
            }
        }
    }
    void BoardSetup() //board is the blank blocks
    {
        topGridObjects = new GameObject[columns, rows];
        bottomGridObjects = new GameObject[columns, rows];
        clueNumbers = new int[columns, rows];
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
            Debug.Log("called 10 times"+gridPositions.Count);
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
    public List<Vector2> ZeroPositionsAround(GameObject block)// where you clicked and the clues around the click pass in coordinates of touch
    {
        List<Vector2> zeroPositions = new List<Vector2>();
        int xIntPos = (int)block.GetComponent<Coordinates>().coordinates.x;
        int yIntPos = (int)block.GetComponent<Coordinates>().coordinates.y;
        for (int x = xIntPos - 1; x <= xIntPos + 1; x += 2)  //does it 3 times i think  
        {
            if (x >= 0 && x < columns)
            {
                if (clueNumbers[x, yIntPos] == 0 )
                {
                    zeroPositions.Add(new Vector2(x, yIntPos));
                }
            }
        }
        for (int y = yIntPos - 1; y <= yIntPos + 1; y += 2)
        {
            if (y >= 0 && y < rows)//includes min and max
            {
                if (clueNumbers[xIntPos, y] == 0 )
                {
                    zeroPositions.Add(new Vector2(xIntPos, y));
                }
            }
        }
        return zeroPositions;
    }
    public List<Vector2> ZeroPositionsAround(Vector2 currentZeroPosition)// TODO add these in the main loop // where you clicked and the clues around the click pass in coordinates of touch
    {                                                                                                                   //and pass in the last iterated vector 2
        List<Vector2> zeroPositions = new List<Vector2>();
        int xIntPos = (int)currentZeroPosition.x;
        int yIntPos = (int)currentZeroPosition.y;

        for (int x = xIntPos - 1; x <= xIntPos + 1; x++)  //doesnt include block clicked
        {
            if (x >= 0 && x < columns)
            {
                for (int y = yIntPos - 1; y <= yIntPos + 1; y++)
                {
                    if (y >= 0 && y < rows)//includes min and max
                    {
                        if (!(x== xIntPos&&y== yIntPos)&&clueNumbers[x, y] == 0)
                        {
                            zeroPositions.Add(new Vector2(x, y));
                            Debug.Log(x+"+"+y+"should do a bunch of times but depending on which block you're on");
                        }
                    }
                }

            }
        }
        return zeroPositions;
    }

    public void ZerosConnected(Vector2 blockPosition)
    {
            List<Vector2> zeros = ZeroPositionsAround(blockPosition);
            
            if (zeros.Count>0)
            {
                int count = zeros.Count;
                for (int i = 0; i< count;i++)
                {
                    Vector2 zero = zeros[i];
                    if(!totalZeroPosition.Contains(zero))
                    {
                    
                    totalZeroPosition.Add(zero);
                    topGridObjects[(int)zero.x,(int)zero.y].GetComponent<Renderer>().enabled = false;
                    Debug.Log("zeros around pressed" + (int)zero.x + " + " + (int)zero.y);
                    ZerosConnected(zero);

                    }
                }
            }

    }
    public void ShowCluesAroundZeros()
    {
        List<Vector2> tempZeroPositions = new List<Vector2>(totalZeroPosition);
        foreach (Vector2 zero in tempZeroPositions)
        {
            int currentX = (int)zero.x;

            int currentY = (int)zero.y;
            for (int x = currentX - 1; x <= currentX + 1; x++)  //doesnt include block clicked
            {
                if (x >= 0 && x < columns)
                {
                    for (int y = currentY - 1; y <= currentY + 1; y++)
                    {
                        if (y >= 0 && y < rows)//includes min and max
                        {
                            if (!(x == currentX && y == currentY) )
                            {
                                Vector2 temp = new Vector2(x, y);
                                if (!tempZeroPositions.Contains(temp))
                                {
                                    
                                    totalZeroPosition.Add(temp);
                                    
                                }
                            }
                        }
                    }

                }
            }
        }
    }

    public void WipeZeroList()
    {
        foreach (Vector2 zero in totalZeroPosition)
        {
            int x = (int)zero.x;

            int y = (int)zero.y;
            topGridObjects[x, y].GetComponent<Renderer>().enabled = false;
        }
        totalZeroPosition = new List<Vector2>();
    }
    public bool IsBlockHitZero(Vector2 blockPosition)
    {
        if (clueNumbers[(int)blockPosition.x, (int)blockPosition.y] == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsBlockHitMine(Vector2 blockPosition)
    {
        if (bottomGridObjects[(int)blockPosition.x, (int)blockPosition.y].tag == "Mine")
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void SetupScene()
    {
        InitialiseList();
        BoardSetup();
        

    }



    // Update is called once per frame
    void Update()
    {
    }
 
}
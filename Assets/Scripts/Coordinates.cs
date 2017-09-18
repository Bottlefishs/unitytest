using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Coordinates : MonoBehaviour {
    [Serializable]
    public class Coordinate
    {
        public int xCoordinates;
        public int yCoordinates;


        public Coordinate(int x, int y)
        {
            xCoordinates = x;
            yCoordinates = y;
        }
    }
    public int xCoordinates;
    public int yCoordinates;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

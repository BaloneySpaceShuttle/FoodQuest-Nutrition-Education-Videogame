using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class Gameboard : MonoBehaviour {
    
    // TODO Rememver to chance under rock to stairs
    // after rock is moved
    //
    // Change to Gem and then Stump after the Bush is moved
    // same for boomerange, bomb, etc..
    
    // Objects:
    // House, Door, Tree
    
    public string[,] gameObjects = new string[26, 19]; //accommodates space

	// Use this for initialization
	void Start () {
		// Add all game objects to array
        
        //AddYRowXRange(0, 0, 25, "Hill");    //startY, startX, maxX, name
       
        AddYRowXRange(0, 0, 25, "Hill");
        AddYRowXRange(18, 0, 25, "Hill");
        AddYRowXRange(3, 0, 7, "Hill");
        AddYRowXRange(4, 0, 7, "Hill");
        AddYRowXRange(5, 0, 6, "Hill");
        AddYRowXRange(14, 0, 4, "Hill");
        AddYRowXRange(15, 0, 4, "Hill");
        AddYRowXRange(9, 7, 9, "Hill");
        AddYRowXRange(10, 7, 9, "Hill");
        AddYRowXRange(14, 7, 9, "Hill");
        AddYRowXRange(15, 7, 11, "Hill");
        AddYRowXRange(13, 15, 22, "Hill");
        AddYRowXRange(12, 15, 22, "Hill");
        AddYRowXRange(5, 15, 22, "Hill");
        AddYRowXRange(4, 15, 22, "Hill");
        AddYRowXRange(3, 15, 22, "Hill");

        AddXColYRange(0, 0, 18, "Hill");
        AddXColYRange(25, 0, 18, "Hill");
        AddXColYRange(10, 1, 14, "Hill");
        AddXColYRange(11, 1, 14, "Hill");
        AddXColYRange(12, 1, 14, "Hill");
        
        AddXColYRange(16, 7, 9, "House");
        AddXColYRange(17, 8, 9, "House");
        AddXColYRange(18, 7, 9, "House");
        
        //NEED TO ADD TREES
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
   // Adds game object name to array using rows
    void AddYRowXRange(int yRow, int xStart, int xEnd, string gOName)
    {
        for (int i = xStart; i <= xEnd; i++)
        {
            gameObjects[i, yRow] = gOName;
        }
    }

    // Adds game object name to array using columns
    void AddXColYRange(int xColumn, int yStart, int yEnd, string gOName)
    {
        for (int i = yStart; i <= yEnd; i++)
        {
            gameObjects[xColumn, i] = gOName;
        }
    }

	public bool IsValidSpace(float x, float y, float horzMove, float vertMove){

		x = horzMove < 0 ? x + 1 : x;
		y = vertMove < 0 ? y + 1 : y;

		x = (float)Math.Floor(Convert.ToDouble(x));
		y = (float)Math.Floor(Convert.ToDouble(y));

		if (gameObjects [(int)x, (int)y] == null) {
			return true;
		} else {
			return false;
		}

	}
    
}

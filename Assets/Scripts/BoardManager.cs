using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class BoardManager : MonoBehaviour
{

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // map dimensions
    public int columns = 8;
    public int rows = 8;
    // min/max number of walls & food per level
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    // game objects
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;
    // head of hierarchy
    private Transform boardHolder;
    // grid positions
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitialiseList()
    {
        // clears positions
        gridPositions.Clear();
        // for loop values leave clear border around outside of level
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                // creates list of possible positions for units & items
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // generates base floor tiles & outer wall
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        // locates outer edge of map to build walls
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                // randomising sprites for floor tiles
                GameObject toInstantiate;

                // creates outer walls if edge; floor if not
                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }
                else
                {
                    toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);

            }
        }
    }

    // finds random position but prevents two items spawning in the same space
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    // puts object on a random tile
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    // calls functions, sets up board
    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);

        // makes more enemies at higher levels
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        //places exit in top right corner
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);


    }
}


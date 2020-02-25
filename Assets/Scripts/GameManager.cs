﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance;

    [Header("Grid Properties")]
    [Tooltip("TileSize represents width and height of the tile")]
    public int tileSize = 64;
    int verticalSize;
    int horizontalSize;

    int columns; //vertical
    int rows; //horizontal

    List<Point> pointsList = new List<Point>();

    [Header("Tiles Pool")]
    static int amountOfTiles = 100; //How many tiles should spawn for the pool
    Tile[] tilesPool = new Tile[amountOfTiles];
    public int tilesPoolFreeIndex = 0;

    [Header("Spawning Properties")]
    public GameObject pointToSpawn;
    public GameObject tileToSpawn;
    public RectTransform gamePanel;
    public GameObject pointParent;

    [Header("Tile fall properties")]
    [Tooltip("The speed that tiles fall at")]
    public float tileFallSpeed = 10.0f;
    public float tileFallMaxTime = 1.0f;
    public AnimationCurve tileFallCurve;

    [Header("RunTime Debugging properties")]
    [SerializeField] Tile firstHitObject;
    public List<GameObject> selectedObjects;
    Tile lastObject = null;

    bool isHeldDown = false;

    private void Start()
    {
        //Find out what size to create the grid
        verticalSize = (int)gamePanel.rect.height;
        horizontalSize = (int)gamePanel.rect.width;

        columns = verticalSize / tileSize;
        rows = horizontalSize / tileSize;

        InitializeGrid();
    }

    private void Update()
    {
        RespawnTiles();
        SelectObject();
    }

    void InitializeGrid()
    {
        //Spawn the points
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                pointsList.Add(SpawnPoints(x, y));
            }
        }

        //spawn the pool of tiles
        SpawnTiles();

        //place first points
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                //place tile at point x and y and pass in the correct position
                PlaceStartTiles(pointsList[(x * columns) + y]);
            }
        }

    }

    Point SpawnPoints(int x, int y)
    {
        GameObject point = Instantiate(pointToSpawn);
        point.transform.SetParent(pointParent.transform);
        point.name = ("X: " + x + " " + "Y: " + y);
        point.transform.localScale = Vector3.one;
        point.transform.localPosition = new Vector3(x * tileSize - (horizontalSize / 2) + (tileSize / 2), y * tileSize - (verticalSize / 2) + (tileSize / 2));
        point.GetComponent<Point>().x = x;
        point.GetComponent<Point>().y = y;
        return point.GetComponent<Point>();
    }

    void SpawnTiles()
    {
        for (int i = 0; i < tilesPool.Length; i++)
        {
            tilesPool[i] = Instantiate(tileToSpawn, Vector3.zero, Quaternion.identity).GetComponent<Tile>();
            tilesPool[i].transform.SetParent(gamePanel.transform);
            tilesPool[i].transform.localScale = Vector3.one;
            tilesPool[i].name = "Tile " + i.ToString();
            tilesPool[i].SetTileActive(false);
        }
    }

    void PlaceStartTiles(Point point)
    {
        if (tilesPoolFreeIndex < tilesPool.Length)
        {
            tilesPool[tilesPoolFreeIndex].SetTilePoint(point);
            tilesPool[tilesPoolFreeIndex].gridLocation.tile = tilesPool[tilesPoolFreeIndex];
            tilesPool[tilesPoolFreeIndex].gridLocation.isOccupied = true;
            tilesPool[tilesPoolFreeIndex].transform.localPosition = new Vector3(point.x * tileSize - (horizontalSize / 2) + (tileSize / 2), point.y * tileSize - (verticalSize / 2) + (tileSize / 2));
            tilesPool[tilesPoolFreeIndex].SetTileActive(true);
            tilesPoolFreeIndex++;
            if (tilesPoolFreeIndex >= tilesPool.Length)
                tilesPoolFreeIndex = 0;
        }
    }

    void SelectObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);
            if (hit.collider != null)
            {
                isHeldDown = true;
                firstHitObject = hit.collider.gameObject.GetComponent<Tile>();
                if (firstHitObject != null)
                {
                    firstHitObject.isConnected = true;
                    selectedObjects.Add(firstHitObject.gameObject);
                    firstHitObject.SetReactionAnimation(true);
                }
            }
        }
        if (Input.GetMouseButton(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);
            if (firstHitObject != null && isHeldDown == true && hit.collider != null)
            {
                Tile newObject = hit.collider.gameObject.GetComponent<Tile>();
                if (newObject != null && newObject.objectValue == firstHitObject.objectValue)
                {
                    if (lastObject == null)
                        lastObject = firstHitObject;
                    if (!selectedObjects.Contains(newObject.gameObject) && IsTileNextTo(lastObject, newObject))
                    {
                        newObject.isConnected = true;
                        newObject.SetReactionAnimation(true);
                        selectedObjects.Add(newObject.gameObject);
                        lastObject = selectedObjects[selectedObjects.Count - 1].GetComponent<Tile>();
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            //if player have selected 3 or more objects
            if (selectedObjects.Count >= 3)
            {
                for (int i = 0; i < selectedObjects.Count; i++)
                {
                    Tile tileInQuestion = selectedObjects[i].GetComponent<Tile>();
                    tileInQuestion.isConnected = false;
                    tileInQuestion.gridLocation.isOccupied = false;
                    tileInQuestion.SetReactionAnimation(false);
                    tileInQuestion.SetTileActive(false);
                }
                selectedObjects.Clear();
                TilesFall();
            }
            //if player have selected less than 3 objects
            else
            {
                for (int i = 0; i < selectedObjects.Count; i++)
                {
                    selectedObjects[i].GetComponent<Tile>().isConnected = false;
                    selectedObjects[i].GetComponent<Tile>().SetReactionAnimation(false);
                }
                selectedObjects.Clear();
            }

            selectedObjects.Clear();
            isHeldDown = false;
            lastObject = null;
            if (firstHitObject != null)
                firstHitObject = null;
        }
    }

    bool IsTileNextTo(Tile inLastObject, Tile inNewObject)
    {
        if (inNewObject.gridLocation.x == inLastObject.gridLocation.x + 1 && //is the new tile up to the right?
            inNewObject.gridLocation.y == inLastObject.gridLocation.y + 1 ||

            inNewObject.gridLocation.x == inLastObject.gridLocation.x + 1 && //right and down
            inNewObject.gridLocation.y == inLastObject.gridLocation.y - 1 ||

            inNewObject.gridLocation.x == inLastObject.gridLocation.x - 1 && //left and up
            inNewObject.gridLocation.y == inLastObject.gridLocation.y + 1 ||

            inNewObject.gridLocation.x == inLastObject.gridLocation.x - 1 && //left and down
            inNewObject.gridLocation.y == inLastObject.gridLocation.y - 1 ||

            inNewObject.gridLocation.x == inLastObject.gridLocation.x + 1 && //right and same row
            inNewObject.gridLocation.y == inLastObject.gridLocation.y ||

            inNewObject.gridLocation.x == inLastObject.gridLocation.x - 1 && //left and same row
            inNewObject.gridLocation.y == inLastObject.gridLocation.y ||

            inNewObject.gridLocation.x == inLastObject.gridLocation.x &&
            inNewObject.gridLocation.y == inLastObject.gridLocation.y + 1 || //same column and up

            inNewObject.gridLocation.x == inLastObject.gridLocation.x &&
            inNewObject.gridLocation.y == inLastObject.gridLocation.y - 1)   //same column and down
            return true;
        else
            return false;
    }

    void TilesFall()
    {
        for (int x = 0; x < columns; x++)
        {
            Tile tileToMove;
            int spacesToMove = 0;
            for (int y = 0; y < rows; y++)
            {
                if (!pointsList[(x * columns) + y].isOccupied)
                    spacesToMove++;
                else
                {
                    tileToMove = pointsList[(x * columns) + y].tile;
                    StartCoroutine(MoveTile(tileToMove, pointsList[(x * columns) + y - spacesToMove]));
                }
            }
        }
    }

    IEnumerator MoveTile(Tile tileToMove, Point newPoint)
    {
        //clear references of tile and point to say that point is not occupied and make sure that tile wont go back to the point it had before
        if(tileToMove.GetTilePoint() != null)
        {
            tileToMove.GetTilePoint().isOccupied = false;
            tileToMove.GetTilePoint().tile = null;
            tileToMove.SetTilePoint(null);
        }
        //set new references
        tileToMove.SetTilePoint(newPoint);
        newPoint.tile = tileToMove;
        newPoint.isOccupied = true;

        //for tiles to fall smoothly yet fast enought
        float timeToMove = 0;
        while(timeToMove < tileFallMaxTime)
        {
            Vector3 startPos = tileToMove.transform.localPosition;
            timeToMove += Time.deltaTime * tileFallSpeed;
            float curvePercent = tileFallCurve.Evaluate(timeToMove / tileFallMaxTime);
            tileToMove.transform.localPosition = Vector3.Lerp(startPos, new Vector3(newPoint.x * tileSize - (horizontalSize / 2) + (tileSize / 2), newPoint.y * tileSize - (verticalSize / 2) + (tileSize / 2)), curvePercent);
            yield return null;
        }
    }

    void RespawnTiles()
    {
        for (int x = 0; x < columns; x++)
        {
            //Check all the tiles at the top of the game
            if (!pointsList[(x * columns) + rows - 1].isOccupied)
            {
                Tile tileToSpawn = FindFirstDisabledTile();
                if(tileToSpawn != null)
                {
                    //set the tile point to one above the top row
                    tileToSpawn.transform.localPosition = new Vector3(pointsList[(x * columns) + rows - 1].x * tileSize - (horizontalSize / 2) + (tileSize / 2), (pointsList[(x * columns) + rows - 1].y + 1) * tileSize - (verticalSize / 2) + (tileSize / 2));
                    //set the tile point to the top point that it should fall down to
                    tileToSpawn.SetTilePoint(pointsList[(x * columns) + (rows - 1)]);
                    tileToSpawn.SetTileActive(true);
                    StartCoroutine(MoveTile(tileToSpawn, pointsList[(x * columns) + (rows - 1)]));
                    //Make tile fall properly into the right spot
                    TilesFall();
                }
            }
        }
    }

    Tile FindFirstDisabledTile()
    {
        Tile tileToReturn = null;
        for (int i = tilesPoolFreeIndex; i < tilesPool.Length; i++)
        {
            if (tilesPool[tilesPoolFreeIndex].GetTileActive())
            {
                tilesPoolFreeIndex++;
                if (tilesPoolFreeIndex >= tilesPool.Length)
                    tilesPoolFreeIndex = 0;

                continue;
            }
            else
            {
                tileToReturn = tilesPool[tilesPoolFreeIndex];
                tilesPoolFreeIndex++;
                if (tilesPoolFreeIndex >= tilesPool.Length)
                    tilesPoolFreeIndex = 0;

                return tileToReturn;
            }
        }

        return tileToReturn;
    }

    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            instance = new GameObject("DecalHandler").AddComponent<GameManager>();
        }
        return instance;
    }

    private void OnDestroy()
    {
        instance = null;
    }

}
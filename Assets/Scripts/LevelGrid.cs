using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGrid : MonoBehaviour
{
    int verticalSize;
    int horizontalSize;

    public int tileSize = 64;

    int columns; //this is vertical
    int rows; //this is horizontal

    public int[,] grid;

    public GameObject pointToSpawn;
    public GameObject tileToSpawn;
    public RectTransform gamePanel;
    public GameObject pointParent;

    private void Start()
    {
        verticalSize = (int)gamePanel.rect.height;
        horizontalSize = (int)gamePanel.rect.width;

        columns = verticalSize / tileSize;
        rows = horizontalSize / tileSize;

        InitializeGrid();
    }

    void InitializeGrid()
    {
        grid = new int[columns, rows];

        for (int i = 0; i < columns; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                SpawnTiles(SpawnPoints(i, j));
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

    void SpawnTiles(Point point)
    {
        GameObject tile = Instantiate(tileToSpawn);
        tile.transform.SetParent(gamePanel.transform);
        tile.transform.localScale = Vector3.one;
        tile.transform.localPosition = new Vector3(point.x * tileSize - (horizontalSize / 2) + (tileSize / 2), point.y * tileSize - (verticalSize / 2) + (tileSize / 2));
        tile.GetComponent<ConnectObject>().SetTilePoint(point);
    }

    void SelectObject()
    {
        //if(mouse.)
    }

}
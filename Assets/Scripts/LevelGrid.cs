using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGrid : MonoBehaviour
{
    public int verticalSize;
    public int horizontalSize;

    public int tileSize = 64;

    [SerializeField] int columns; //this is vertical
    [SerializeField] int rows; //this is horizontal

    public int[,] grid;

    public GameObject tileToSpawn;
    public RectTransform gamePanel;

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
                grid[i, j] = Random.Range(0, 10);
                SpawnTile(i, j, grid[i, j]);
            }
        }
    }

    void SpawnTile(int x, int y, int value)
    {
        GameObject tile = Instantiate(tileToSpawn);
        tile.transform.SetParent(gamePanel);
        tile.name = ("X: " + x + " " + "Y: " + y);
        tile.transform.localPosition = new Vector3(x * (tileSize / 2), y * (tileSize / 2));
    }

}
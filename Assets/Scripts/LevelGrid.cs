using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGrid : MonoBehaviour
{
    static LevelGrid instance;

    int verticalSize;
    int horizontalSize;

    public int tileSize = 64;

    int columns; //this is vertical
    int rows; //this is horizontal

    public int[,] grid;
    public List<Point> pointsList;

    static int amountOfTiles = 100;
    ConnectObject[] tilesPool = new ConnectObject[amountOfTiles];
    int tilesPoolFreeIndex = 0;


    public GameObject pointToSpawn;
    public GameObject tileToSpawn;
    public RectTransform gamePanel;
    public GameObject pointParent;

    [SerializeField] bool isHeldDown = false;
    [SerializeField] ConnectObject firstHitObject;
    [SerializeField] int selectedObjectValue;
    public List<GameObject> selectedObjects;

    private void Start()
    {
        verticalSize = (int)gamePanel.rect.height;
        horizontalSize = (int)gamePanel.rect.width;

        columns = verticalSize / tileSize;
        rows = horizontalSize / tileSize;

        InitializeGrid();
    }

    private void Update()
    {
        SelectObject();
    }

    void InitializeGrid()
    {
        grid = new int[columns, rows];

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                pointsList.Add(SpawnPoints(x,y));
            }
        }

        SpawnTiles();

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                //place tile at point i and j (x and y) and pass in pointList[j or HOW DO I FIND THE CORRECT POINT WHEN I HAVE i and j]?
                PlaceTile(pointsList[(x * columns) + y], x, y);
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
            tilesPool[i] = Instantiate(tileToSpawn, Vector3.zero, Quaternion.identity).GetComponent<ConnectObject>();
            tilesPool[i].transform.SetParent(gamePanel.transform);
            tilesPool[i].transform.localScale = Vector3.one;
            tilesPool[i].name = "Tile " + i.ToString();
            tilesPool[i].gameObject.SetActive(false);
        }
    }

    void PlaceTile(Point point, int x, int y)
    {
        if (tilesPoolFreeIndex < tilesPool.Length)
        {
            tilesPool[tilesPoolFreeIndex].transform.localPosition = new Vector3(x * tileSize - (horizontalSize / 2) + (tileSize / 2), y * tileSize - (verticalSize / 2) + (tileSize / 2));
            tilesPool[tilesPoolFreeIndex].gameObject.SetActive(true);
            tilesPool[tilesPoolFreeIndex].SetTilePoint(point);
            tilesPoolFreeIndex++;
            if (tilesPoolFreeIndex >= tilesPool.Length)
            {
                tilesPoolFreeIndex = 0;
            }
        }
    }

    void SelectObject()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);
            if(hit.collider != null)
            {
                isHeldDown = true;
                Debug.Log(hit.collider.gameObject);
                firstHitObject = hit.collider.gameObject.GetComponent<ConnectObject>();
                if(firstHitObject != null)
                {
                    firstHitObject.isConnected = true;
                    selectedObjects.Add(firstHitObject.gameObject);
                    selectedObjectValue = firstHitObject.objectValue;
                }
            }
        }
        if(Input.GetMouseButton(0))
        {
            if(firstHitObject != null && isHeldDown == true)
            {
                RaycastHit2D hit = Physics2D.Raycast(Input.mousePosition, Vector2.zero);
                if (hit.collider != null)
                {
                    ConnectObject newObject = hit.collider.gameObject.GetComponent<ConnectObject>();
                    if(newObject != null && firstHitObject != null && newObject.objectValue == firstHitObject.objectValue)
                    {
                        if(!selectedObjects.Contains(newObject.gameObject))
                        {
                            newObject.isConnected = true;
                            selectedObjects.Add(newObject.gameObject);
                        }
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (firstHitObject != null)
            {
                firstHitObject.isConnected = false;
                firstHitObject = null;
            }
            if (selectedObjects.Count >= 3)
            {
                for (int i = 0; i < selectedObjects.Count; i++)
                {
                    selectedObjects[i].GetComponent<ConnectObject>().isConnected = false;
                    selectedObjects[i].SetActive(false);
                }
            }
            for (int i = 0; i < selectedObjects.Count; i++)
            {
                selectedObjects[i].GetComponent<ConnectObject>().isConnected = false;
            }
            selectedObjects.Clear();
            isHeldDown = false;
        }
    }

    public static LevelGrid GetInstance()
    {
        if (instance == null)
        {
            instance = new GameObject("DecalHandler").AddComponent<LevelGrid>();
        }
        return instance;
    }

    private void OnDestroy()
    {
        instance = null;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectObject : MonoBehaviour
{
    public Image objectImage;
    public Sprite[] objectSprites;

    public Point gridLocation;
    public int objectValue = 0; //This represents what type of object it is, 0 = Circle, 1 = Cube, 2 = Diamond, 3 = Star, 4 == Triangle
    public bool isConnected = false;

    private void Start()
    {
        objectValue = Random.Range(0, 4);

        if(objectSprites.Length > 0)
        {
            objectImage.sprite = objectSprites[objectValue];
        }
    }

    public void SetTilePoint(Point point)
    {
        gridLocation = point;
    }
}

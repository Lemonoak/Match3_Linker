using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [Header("Visual properties")]
    [Tooltip("The image of the tile")]
    public Image objectImage;
    [Tooltip("The different sprites for each object")]
    public Sprite[] objectSprites;
    Animator anim;

    [HideInInspector] public Point gridLocation;
    [HideInInspector] public int objectValue = 0; //This represents what type of object it is, 0 = Circle, 1 = Cube, 2 = Diamond, 3 = Star, 4 == Triangle
    [HideInInspector] public bool isConnected = false;

    private void Start()
    {
        anim = GetComponent<Animator>();

        objectValue = Random.Range(0, objectSprites.Length);
        if (objectSprites.Length > 0)
            objectImage.sprite = objectSprites[objectValue];
    }

    public void SetTilePoint(Point point)
    {
        gridLocation = point;
    }

    public Point GetTilePoint()
    {
        if (gridLocation != null)
            return gridLocation;

        return null;
    }

    public void SetReactionAnimation(bool animationParam)
    {
        anim.SetBool("isConnected", animationParam);
    }
}

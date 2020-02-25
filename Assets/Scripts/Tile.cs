using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [Header("Visual properties")]
    [Tooltip("The image of the tile")]
    public Image tileImage;
    [Tooltip("The different sprites for each object")]
    public Sprite[] objectSprites;
    Animator anim;
    public Collider2D tileCollider;

    [HideInInspector] public Point gridLocation;
    [HideInInspector] public int objectValue = 0; //This represents what type of object it is, 0 = Circle, 1 = Cube, 2 = Diamond, 3 = Star, 4 == Triangle
    [HideInInspector] public bool isConnected = false;
    bool isActive = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        tileCollider = GetComponent<Collider2D>();

        objectValue = Random.Range(0, objectSprites.Length);
        if (objectSprites.Length > 0)
            tileImage.sprite = objectSprites[objectValue];
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

    public void Respawn(Point point)
    {
        SetTilePoint(point);
        //randomize tile object
        objectValue = Random.Range(0, objectSprites.Length);
        if (objectSprites.Length > 0)
            tileImage.sprite = objectSprites[objectValue];
        gameObject.SetActive(true);

    }

    public void SetTileActive(bool isTileActive)
    {
        isActive = isTileActive;
        tileCollider.enabled = isTileActive;
        tileImage.enabled = isTileActive;
    }

    public bool GetTileActive()
    {
        return isActive;
    }


    public void SetReactionAnimation(bool animationParam)
    {
        anim.SetBool("isConnected", animationParam);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
    public int x;
    public int y;
    public bool isOccupied = false;

    public Tile tile;

    //public Image sprite;


//#if (UNITY_EDITOR)
//    private void Update()
//    {
//        if(isOccupied)
//        {
//            sprite.color = Color.red;
//        }
//        else
//        {
//            sprite.color = Color.green;
//        }
//    }
//#endif
}

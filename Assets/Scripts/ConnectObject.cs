using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectObject : MonoBehaviour
{
    public Image objectSprite;

    public Point gridLocation;
    public int objectValue = 0; //This represents what type of object it is, 0 = nothing, 1 = blocked, 2 = object, 3 = second object and so on
}

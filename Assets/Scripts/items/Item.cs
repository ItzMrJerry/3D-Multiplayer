using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemInfo iteminfo;
    public GameObject itemGameobject;

    public abstract void Use();
}

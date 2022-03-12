using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public int id;

    private readonly int bombId = -1;

    public void OnDestory()
    {
        Destroy(gameObject);
    }

    public bool IsBomb() { return id == bombId; }
}
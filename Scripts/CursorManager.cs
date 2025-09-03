using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D cursor;
    private Vector2 hotSpot = Vector2.zero;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Cursor.SetCursor(cursor, hotSpot, CursorMode.Auto);
    }
}

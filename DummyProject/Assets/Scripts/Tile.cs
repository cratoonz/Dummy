using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Normal,
    Obstacle
}

public class Tile : MonoBehaviour
{
    public int xIndex;
    public int yIndex;
    

    private Board m_board;
    public TileType tileType = TileType.Normal;
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Init(int x, int y, Board board)
    {
        yIndex = y;
        xIndex = x;
        m_board = board;
    }

    private void OnMouseDown()
    {
        if (m_board != null)
        {
            m_board.ClickTile(this);
        }
    }

    private void OnMouseEnter()
    {
        if (m_board != null)
        {
            m_board.DragToTile(this);
        }
    }

    private void OnMouseUp()
    {
        if (m_board != null)
        {
            m_board.ReleaseTile();
        }
    }
}

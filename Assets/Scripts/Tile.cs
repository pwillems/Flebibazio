using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public int xIndex;
    public int yIndex;

    public Board m_board;
    bool tutorial;


    // Use this for initialization
    void Start () {
        tutorial = m_board.tutorial;
    }


    public void Init (int x, int y, Board board)
    {
        xIndex = x;
        yIndex = y;
        m_board = board;
    }

    void OnMouseUp()
    {
        /* Used if you want to have another interaction for the tutorial clicks
        if (!tutorial)
        {
            if (m_board != null)
            {
                m_board.ClickTile(this);
            }
        }
        else
        {
            m_tutorial.NextStep();
        }*/

        if (m_board != null)
        {
            m_board.ClickTile(this);
        }
    }
}

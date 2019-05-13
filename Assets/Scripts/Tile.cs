using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public int xIndex;
    public int yIndex;

    Board m_board;
    TutorialController m_tutorial;
    public bool tutorial = false;


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
        }
    }

    void OnMouseEnter()
    {
		/*
        if (m_board != null)
        {
            m_board.DragToTile(this);
        }
        */
    }

    /*void OnMouseUp()
    {
		/*
        if (m_board != null)
        {
            m_board.ReleaseTile();
        }

    }*/
}

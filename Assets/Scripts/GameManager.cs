using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int movesLeft = 30;
    public int scoreGoal = 10000;

    public ScreenFader screenFader;
    public Text levelNameText;
    public Text movesLeftText;

    Board m_board;

    bool m_isReadyToBegin = false;
    bool m_isGameOver = false;
    bool m_isWinner = false;
    bool m_isReadyToReload = false;

    // MessageWindow setup
    public MessageWindow messageWindow;

    // Start is called before the first frame update
    void Start()
    {
        m_board = GameObject.FindObjectOfType<Board>().GetComponent<Board>();

        Scene Scene = SceneManager.GetActiveScene();

        if(levelNameText != null)
        {
            levelNameText.text = Scene.name;
        }

        UpdateMoves();

        StartCoroutine("ExecuteGameLoop");
    }

    public void UpdateMoves()
    {
        if(movesLeftText != null)
        {
            movesLeft--;
            movesLeftText.text = movesLeft.ToString();
        }
    }

    IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine("StartGameRoutine");
        yield return StartCoroutine("PlayGameRoutine");
        yield return StartCoroutine("EndGameRoutine");
    }

    public void BeginGame()
    {
        m_isReadyToBegin = true;
    }

    IEnumerator StartGameRoutine()
    {
        if(messageWindow!= null)
        {
            messageWindow.GetComponent<RectXformMover>().MoveOn();
            messageWindow.ShowMessage(null, "Score goal\n" + scoreGoal.ToString(), "Start");
        }

        while (!m_isReadyToBegin)
        {
            yield return null;
            
        }

        if (screenFader != null)
        {
            screenFader.FadeOff();
        }

        yield return new WaitForSeconds(0.5f);
        if (m_board != null)
        {
            m_board.SetupBoard();
        }
    }
    IEnumerator PlayGameRoutine()
    {
        while (!m_isGameOver)
        {
            if(movesLeft == 0)
            {
                m_isGameOver = true;
                m_isWinner = false;
            }
            yield return null;
        }
    }
    IEnumerator EndGameRoutine()
    {
        m_isReadyToReload = false;

        if (screenFader != null)
        {
            screenFader.FadeOn();
        }
        if (m_isWinner)
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectXformMover>().MoveOn();
                messageWindow.ShowMessage(null, "YOU WIN", "YES");
            }
        }
        else
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectXformMover>().MoveOn();
                messageWindow.ShowMessage(null, "YOU LOST :(", "SHIT");
            }
        }
        while (!m_isReadyToReload)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void ReloadScene()
    {
        m_isReadyToReload = true;
    }
}

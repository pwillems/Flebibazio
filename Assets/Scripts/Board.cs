﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // Level settings
	public int width;
	public int height;
    public int turnIncrease = 1;
    public int rowIncrease = 5;
    public int level = 0;
    public float swapTime = 0.5f;
    public int borderSize;
    public int turn = 0;
    public float fadeTime = 0.5f;
    public float moveTime = 0.3f;
    public float fadeTimeGameStart = 0.1f;

    // Set 2D arrays
    Tile[,] m_allTiles;
    GamePiece[,] m_allGamePieces;
    int[,] m_levelBuilder;
    Tile m_clickTile;
    Tile m_targetTile;

    // Use stepnumer to chain animations, each one can be delayed 0.3s*m_stepnumber
    int m_stepNumber;

    // Set rules
    public int difficulty;
	public bool ruleCheckTop = true;
	public bool ruleCheckBottom = true;
	public bool ruleCheckLeft = true;
	public bool ruleCheckRight = true;
    public bool ruleCheckColumn = true;
    public bool ruleCheckRow = true;

    // Refered scripts
    private AnimationQueue animationQueue;
    public GameObject tilePrefab;
    private GameObject score;
    public GameObject[] gamePiecePrefabs;
    public GameObject nextShape;
    public GameManager gameManager;

    // CSV Array containing all the levels + Boolean to check if we are in the tutorial
    public TextAsset[] levelsCSV;
    private char lineSeperater = '\n'; // It defines line seperate character
    private char fieldSeperator = ','; // It defines field seperate chracter
    public bool tutorial = false;
    
	// Use this for initialization
	void Start ()
	{
        animationQueue = GetComponent<AnimationQueue>();
        score = GameObject.Find("ScoreManager");
		m_allTiles = new Tile[width, height];
		m_allGamePieces = new GamePiece[width, height];
        m_levelBuilder = new int[width, height];
        SetupTiles();
        SetupCamera();
    }

    public void SetupBoard()
    {
        if (tutorial)
        {
            // Tutorial time, let's do this! 
        }
        else
        {
            // Not Tutorial time, let's start a level! 
            if (level == 0)
            {
                // Endless mode! 
                FillRandom();
            }
            if (level == 1)
            {
                // Load in level from CSV file
                LoadLevel(levelsCSV[0]);
            }
        }

        // Setup next play piece
        Color tempColor = gamePiecePrefabs[1].GetComponent<SpriteRenderer>().color;
        nextShape.GetComponent<Image>().color = new Color(tempColor.r, tempColor.g, tempColor.b, 1);
    }

	/* Debugging purposes:
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
            Debug.Log(m_allGamePieces);
			// Print the grid types for debugging, fix those errors!! 
			for (int x = 0; x < width; x++) {
				int[] tempRow = new int[width];
				for (int y = 0; y < height; y++) {
					if (m_allGamePieces [x, y] == null) {
						tempRow[y] = 3;
					} else {
						tempRow[y] = m_allGamePieces [x, y].type;
					}
				}		
				Debug.Log (tempRow[0] + " " + tempRow[1] + " " + tempRow[2] + " " + tempRow[3] + " " + tempRow[4] + " " + tempRow[5]);
			}

		}
	}*/

    void LoadLevel(TextAsset levelData)
    {
        // Read CSV data and fill levelBuilder 2d array with the grid
        string[] records = levelData.text.Split(lineSeperater);
        for (int i = 0; i < records.Length; i++)
        {
            string[] fields = records[i].Split(fieldSeperator);
            for (int j = 0; j < fields.Length; j++)
            {
                m_levelBuilder[i, j] = int.Parse(fields[j]);
            }
        }

        // Now build the level and set the 2d arrays used
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (m_levelBuilder[i, j] == 1 || m_levelBuilder[i, j] == 2)
                {
                    GameObject newGamePiece = Instantiate(gamePiecePrefabs[m_levelBuilder[i, j] - 1], Vector3.zero, Quaternion.identity) as GameObject;
                    if (newGamePiece != null)
                    {
                        newGamePiece.GetComponent<GamePiece>().Init(this);
                        PlaceGamePiece(newGamePiece.GetComponent<GamePiece>(), i, j, 1, 0, fadeTime);
                        newGamePiece.transform.parent = transform;
                    }
                }
            }
        }
    }

    // 
	void SetupTiles ()
	{
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				GameObject tile = Instantiate (tilePrefab, new Vector3 (i, j, 0), Quaternion.identity) as GameObject;

				tile.name = "Tile (" + i + "," + j + ")";

				m_allTiles [i, j] = tile.GetComponent<Tile> ();

				tile.transform.parent = transform;

				m_allTiles [i, j].Init (i, j, this);
			}
		}
	}

	void SetupCamera ()
	{
        // Take care of automatic zooming of the camera, so it doesn't matter how big the field is
		Camera.main.transform.position = new Vector3 ((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10);

		float aspectRatio = (float)Screen.width / (float)Screen.height;

		float verticalSize = (float)height / 2f + (float)borderSize;
		float horizontalSize = ((float)width / 2f + (float)borderSize) / aspectRatio;

		Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
	}
    
    void NextTurn()
    {
        // Quickly done, needs some updating if we are using more shapes
        if(turn == 0)
        {
            // Setup next play piece
            Color tempColor = gamePiecePrefabs[0].GetComponent<SpriteRenderer>().color;
            nextShape.GetComponent<Image>().color = new Color(tempColor.r, tempColor.g, tempColor.b, 1);
            turn = 1;
        }

        else if (turn == 1)
        {
            // Setup next play piece
            Color tempColor = gamePiecePrefabs[1].GetComponent<SpriteRenderer>().color;
            nextShape.GetComponent<Image>().color = new Color(tempColor.r, tempColor.g, tempColor.b, 1);
            turn = 0;
        }

        if(gameManager != null)
        {
            gameManager.UpdateMoves();
        }
    }

	GameObject GetRandomGamePiece ()
	{
		// Get random gamePiece
		int randomIdx = Random.Range (0, gamePiecePrefabs.Length);

		if (gamePiecePrefabs [randomIdx] == null) {
			Debug.LogWarning ("Board" + randomIdx + "does not contain valid GamePiece prefab");
		}

		return gamePiecePrefabs [randomIdx];
	}

	public void PlaceGamePiece (GamePiece gamePiece, int x, int y, int shape, int newLine, float timeToFade)
	{
		if (gamePiece == null) {
			Debug.LogWarning ("Invalid GamePiece");
			return;
		}

		// Check if the piece needs to move into the field from outside of the board
		if (newLine == 1) {
			// fall in from top
			gamePiece.transform.position = new Vector3 (x, y+1, 0);
			gamePiece.transform.rotation = Quaternion.identity;
            animationQueue.addMoveAnimation(gamePiece, moveTime, new Vector3(x, y, 0));
		} else if (newLine == 2) {
			// fall in from right
			gamePiece.transform.position = new Vector3 (x+1, y, 0);
			gamePiece.transform.rotation = Quaternion.identity;
            animationQueue.addMoveAnimation(gamePiece, moveTime, new Vector3(x, y, 0));
		} else {
			// just place it
			gamePiece.transform.position = new Vector3 (x, y, 0);
			gamePiece.transform.rotation = Quaternion.identity;
		}

		if (IsWithinBounds (x, y)) {
			//Debug.Log ("Piece at position " + x + ", " + y + " placed");
			m_allGamePieces [x, y] = gamePiece;
		}

		// Set the coordinates in the shape
		gamePiece.SetCoord (x, y, shape);
        //TODO: WIP
        animationQueue.addFadeAnimation(gamePiece, timeToFade, 1);
        //gamePiece.FadeIn(fadeTime);
    }

	bool IsWithinBounds (int x, int y)
	{
		return (x >= 0 && x < width && y >= 0 && y < height);
	}

	void FillRandom ()
	{
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				PlaceRandomPiece (i, j, 0, fadeTimeGameStart);
			}
		}
	}

	void PlaceRandomPiece(int i, int j, int newLine, float timeToFade) 
	{
        // Get a random piece of which 50% is null, 50% is divided over the shapes
		int randomNumber = Random.Range (0, 100);

		if (randomNumber < 50) {
			m_allGamePieces [i, j] = null;
		} else {

			int index;
			int counter;

			GameObject tempRandom = GetRandomGamePiece ();

			for (counter = 0; counter < gamePiecePrefabs.Length; counter++) {
				if (gamePiecePrefabs [counter] == tempRandom) {
					index = counter;
					break;
				}
			}

			GameObject randomPiece = Instantiate (tempRandom, Vector3.zero, Quaternion.identity) as GameObject;

			if (randomPiece != null) {
				randomPiece.GetComponent<GamePiece> ().Init (this);
				PlaceGamePiece (randomPiece.GetComponent<GamePiece> (), i, j, counter, newLine, timeToFade);
				randomPiece.transform.parent = transform;
			}
		}
	}

	public void ClickTile (Tile tile)
	{

		if (m_clickTile == null) {
			int y = tile.yIndex;
			int x = tile.xIndex;

			if (m_allGamePieces [x, y] == null) {

				// Check which turn it is
				if (turn % 2 != 1) {
					// turn is even
					//Debug.Log ("Empty clicked tile: " + tile.name + ", turn number is " + turn + ", shape is 1 = Blue");

					GameObject newGamePiece = Instantiate (gamePiecePrefabs [1], Vector3.zero, Quaternion.identity) as GameObject;

					if (newGamePiece != null) {
						newGamePiece.GetComponent<GamePiece> ().Init (this);
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, y, 1, 0, fadeTime);
						newGamePiece.transform.parent = transform;
					}

					// Now check sides (maybe filling it)
					checkSides (x, y, 1);

				} else {
					// turn is odd
					//Debug.Log ("Empty clicked tile: " + tile.name + ", turn number is " + turn + ", shape is 0 = Yellow");

					GameObject newGamePiece = Instantiate (gamePiecePrefabs [0], Vector3.zero, Quaternion.identity) as GameObject;

					if (newGamePiece != null) {
						newGamePiece.GetComponent<GamePiece> ().Init (this);
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, y, 0, 0, fadeTime);
						newGamePiece.transform.parent = transform;
					}

					// Now check sides (maybe filling it)
					checkSides (x, y, 0);
				}
			}

            // Update the score
            ScoreManager scoreManager = (ScoreManager)score.GetComponent(typeof(ScoreManager));
            scoreManager.AddScore(turnIncrease);

			// Now check if the playingfield is full
			isFull();

            // Next turn :) 
            NextTurn();
		}

	}

	// Needs some work to check if the field is full
	public void isFull(){
		int fullCount = 0;
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				fullCount++;
			}
		}
		if (fullCount == height * width) {
			//Debug.Log ("Game over, playing field full!");
            // TODO, add score to scoreboard and return to menu
		}
	}

	public void checkFullRow(){
		for (int y = 0; y < height; y++) {

			int startShape = 325;

			if (m_allGamePieces [0, y] != null) {
				startShape = m_allGamePieces [0, y].type;
			}

			for (int x = 1; x < width; x++) {
				if (m_allGamePieces[x,y] != null && m_allGamePieces [x, y].type == startShape) {
					// Cool this is the same, now check if its the last one
					if(x == (width-1)){
						// We got a full row!! 
						Debug.Log("Full row " + y);
						removeRow(y);
                        // checkSides ();
                        break;
					}
				} else {
					break;
				}
			}
		}
	}

	public void checkFullColumn() {
		for (int x = 0; x < width; x++) {

			int startShape = 325;

			if (m_allGamePieces [x, 0] != null) {
				startShape = m_allGamePieces [x, 0].type;
			}

			for (int y = 1; y < height; y++) {
				if (m_allGamePieces [x, y] != null && m_allGamePieces [x, y].type == startShape) {
					// So at least shape 2 is the same as the startshape
					if (y == (height-1)) {
						// We got a winner folks! 
						Debug.Log("Full column " + x);
						removeColumn (x);
						break;
					}
				} else {
					break;
				}
			}
		}
	}

	public void removeRow(int row){
		// First delete the row and set the grid to null for this row.
		for (int x = 0; x < width; x++) {
			// Old code, doesn't use animationQueue
            //m_allGamePieces [x, row].FadeOut (fadeTime);
            animationQueue.addFadeAnimationBatch(m_allGamePieces[x, row], fadeTime, 3, width);
            m_allGamePieces[x, row] = null;
        }

		// Then move the next row
		if (row != height - 1) {
			for (int y = row+1; y < height; y++) {
                int tempRowCounter = 0;
                for (int x = 0; x < width; x++) {
                    // First count the amount of pieces per row we need to move
                    if (m_allGamePieces[x, y] != null)
                    {
                        tempRowCounter++;
                    }
				}
                for(int x = 0; x < width; x++)
                {
                    // Then actually move the moves per group (use tempRowCounter as groupsize)
                    if (m_allGamePieces[x, y] != null)
                    {
                        // Try moving row by row
                        animationQueue.addMoveAnimationBatch(m_allGamePieces[x, y], moveTime, new Vector3(x, (y - 1), 0), tempRowCounter);

                        // Old code for moving piece by piece
                        // animationQueue.addMoveAnimation(m_allGamePieces[x, y], moveTime, new Vector3(x, (y - 1), 0));

                        // Now update the matrix
                        m_allGamePieces[x, (y - 1)] = m_allGamePieces[x, y];
                        m_allGamePieces[x, (y - 1)].xIndex = x; // This is probably not needed
                        m_allGamePieces[x, (y - 1)].yIndex = y - 1;

                        // Set the previous one to null
                        m_allGamePieces[x, y] = null;
                    }
                }
			}
		}

        // Now add a new row
        for (int x = 0; x < width; x++) {
            PlaceRandomPiece (x, height - 1, 0, fadeTime);
		}

        // Now increase the score
        // Update the score
        ScoreManager scoreManager = (ScoreManager)score.GetComponent(typeof(ScoreManager));
        scoreManager.AddScore(rowIncrease);

    }

	public void removeColumn(int column){

        Debug.Log("Remove Column " + column);

		// First delete the column
		for (int y = 0; y < height; y++) {
            animationQueue.addFadeAnimationBatch(m_allGamePieces[column, y], fadeTime, 3, height);
			m_allGamePieces [column, y] = null;
		}

		// Now move the rest
		if (column != width - 1) {
			for (int x = column + 1; x < width; x++) {
                int tempRowCounter = 0;
				for (int y = 0; y < height; y++)
                {
					if (m_allGamePieces[x, y] != null)
					{
						tempRowCounter++;
					}
                }
				for (int y = 0; y < height; y++) {
					Debug.Log ("All game pieces x, y = " + x + ", " + y);
					if (m_allGamePieces [x, y] != null)
                    {
                        // Try moving columnm by column
						Debug.Log("Old pos = " + x + ", " + y + "  -   New pos = " + (x-1) + ", " + y);
						animationQueue.addMoveAnimationBatch(m_allGamePieces[x, y], moveTime, new Vector3((x - 1), y, 0), tempRowCounter);

                        // Old code for moving piece by piece
                        // animationQueue.addMoveAnimation(m_allGamePieces[x, y], moveTime, new Vector3((x - 1), y, 0));

                        // Now update the matrix
                        m_allGamePieces[(x - 1), y] = m_allGamePieces [x, y];
                        m_allGamePieces[(x - 1), y].xIndex = x;
                        m_allGamePieces[(x - 1), y].yIndex = y;

                        // Set the previous one to null
						m_allGamePieces[x, y] = null;
					}
				}
			}
		}

		// Now add a new column
		for (int y = 0; y < height; y++) {
			PlaceRandomPiece (width-1, y, 1, fadeTime);
		}

        // Now increase the score
        // Update the score
        ScoreManager scoreManager = (ScoreManager)score.GetComponent(typeof(ScoreManager));
        scoreManager.AddScore(rowIncrease);
    }

	public void checkSides (int x, int y, int shape)
	{
		// Check all the sides, reset stepnumber to 0
		m_stepNumber = 0;

		if (x < width - 1 && ruleCheckRight == true) {
			checkRight (x, y, shape);
		}
		if (x != 0 && ruleCheckLeft == true) {
			checkLeft (x, y, shape);
		}
		if (y < height - 1 && ruleCheckTop == true) {
			checkTop (x, y, shape);
		}
		if (y != 0 && ruleCheckBottom == true) {
			checkBottom (x, y, shape);
		}

        if (ruleCheckColumn)
        {
            checkFullColumn();
        }
        if (ruleCheckRow)
        {
            checkFullRow();
        }
	}

	void checkRight (int x, int y, int shape)
	{
		if (m_allGamePieces [x + 1, y] != null && m_allGamePieces [x + 1, y].type == shape) {
			
			// Right is the same shape
			for (int i = x + 2; i < width; i++) {

				// Now check all the shapes to the right
				if (m_allGamePieces [i, y] == null) {

					// Empty spot found, fill with new shape
					GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

					if (newGamePiece != null) {
						newGamePiece.GetComponent<GamePiece> ().Init (this);
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), i, y, shape, 0, fadeTime);
						newGamePiece.transform.parent = transform;
					}

				} 
				if (m_allGamePieces [i, y].type == shape) {

					// Do nothing, go to the next shape
					
				} 
				if (m_allGamePieces [i, y].type != shape) {
					
					// Wrong shape detected, change it, then step out of the loo
					// Fade old piece
                    animationQueue.addFadeAnimation(m_allGamePieces[i, y], fadeTime, 2);
					m_allGamePieces [i, y] = null;

					GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

					if (newGamePiece != null) {
						newGamePiece.GetComponent<GamePiece> ().Init (this);
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), i, y, shape, 0, fadeTime);
						newGamePiece.transform.parent = transform;
					}

					break;
				}
			}
		}
	}

	void checkLeft (int x, int y, int shape)
	{
		if (m_allGamePieces [x - 1, y] != null && m_allGamePieces [x - 1, y].type == shape) {

			// Left is the same shape
			for (int i = x - 2; i >= 0; i--) {

				// Now check all the shapes to the left
				if (m_allGamePieces [i, y] == null) {

					// Empty spot found, fill with new shape
					GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

					if (newGamePiece != null) {
						//Debug.Log("New piece " + i + " " + y + " " + shape);
						newGamePiece.GetComponent<GamePiece> ().Init (this);
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), i, y, shape, 0, fadeTime);
						newGamePiece.transform.parent = transform;
					}

				} 
				if (m_allGamePieces [i, y].type == shape) {
					// Do nothing, go to the next shape
				} 
				if (m_allGamePieces [i, y].type != shape) {

					// Wrong shape detected, change it, then step out of the loop
					// Fade old piece and set to null
                    animationQueue.addFadeAnimation(m_allGamePieces[i, y], fadeTime, 2);
					m_allGamePieces [i, y] = null;

					// Now place the new piece
					GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

					if (newGamePiece != null) {
						newGamePiece.GetComponent<GamePiece> ().Init (this);
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), i, y, shape, 0, fadeTime);
						newGamePiece.transform.parent = transform;
					}

					break;

				}
			}
		}
	}

	void checkTop (int x, int y, int shape) {
		if (m_allGamePieces [x, y + 1] != null && m_allGamePieces [x, y + 1].type == shape) {
			// Top is the same shape
			for (int i = y + 2; i < height; i++) {
				if (m_allGamePieces [x, i] == null) {
					// Empty spot found, fill with new shape
					GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

					if (newGamePiece != null) {
						//Debug.Log("New piece " + i + " " + y + " " + shape);
						newGamePiece.GetComponent<GamePiece> ().Init (this);
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, i, shape, 0, fadeTime);
						newGamePiece.transform.parent = transform;
					}
				}
				if (m_allGamePieces [x, i].type == shape) {
					// Do nothing, go to the next shape
				}
				if (m_allGamePieces [x, i].type != shape) {

					// Wrong shape detected, change it, then step out of the loop
					// Fade old piece and set to null
                    animationQueue.addFadeAnimation(m_allGamePieces[x, i], fadeTime, 2);
                    m_allGamePieces [x, i] = null;

					// Now place the new piece
					GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

					if (newGamePiece != null) {
						newGamePiece.GetComponent<GamePiece> ().Init (this);
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, i, shape, 0, fadeTime);
						newGamePiece.transform.parent = transform;
					}

					break;

				}
			}
		}
	}

	void checkBottom (int x, int y, int shape) {
		if (m_allGamePieces [x, y - 1] != null && m_allGamePieces [x, y - 1].type == shape) {
			// Bottom is the same shape
				for (int i = y - 2; i >= 0; i--) {
					if (m_allGamePieces [x, i] == null) {
						// Empty spot found, fill with new shape
						GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

						if (newGamePiece != null) {
							//Debug.Log("New piece " + i + " " + y + " " + shape);
							newGamePiece.GetComponent<GamePiece> ().Init (this);
							PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, i, shape, 0, fadeTime);
							newGamePiece.transform.parent = transform;
						}
					}
					if (m_allGamePieces [x, i].type == shape) {
						// Do nothing, go to the next shape
					}
					if (m_allGamePieces [x, i].type != shape) {

                        // Wrong shape detected, change it, then step out of the loop
                        // Fade old piece and set to null
                        animationQueue.addFadeAnimation(m_allGamePieces[x, i], fadeTime, 2);
                        m_allGamePieces [x, i] = null;

						// Now place the new piece
						GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

						if (newGamePiece != null) {
							newGamePiece.GetComponent<GamePiece> ().Init (this);
							PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, i, shape, 0, fadeTime);
							newGamePiece.transform.parent = transform;
						}

						break;

					}
				}
			}
	}
}
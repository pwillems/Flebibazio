using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{

	public int width;
	public int height;
	public GameObject tilePrefab;
	public int difficulty;
	public bool ruleCheckTop = true;
	public bool ruleCheckBottom = true;
	public bool ruleCheckLeft = true;
	public bool ruleCheckRight = true;

    private AnimationQueue animationQueue;

	public float swapTime = 0.5f;

	public int borderSize;
	public GameObject[] gamePiecePrefabs;

	public int turn = 0;

	public float fadeTime = 0.5f;
	public float moveTime = 1f;

	// Set 2D arrays
	Tile[,] m_allTiles;
	GamePiece[,] m_allGamePieces;

	Tile m_clickTile;
	Tile m_targetTile;

	// Use stepnumer to chain animations, each one can be delayed 0.3s*m_stepnumber
	int m_stepNumber;
    
	// Use this for initialization
	void Start ()
	{
        animationQueue = GetComponent<AnimationQueue>();
		m_allTiles = new Tile[width, height];
		m_allGamePieces = new GamePiece[width, height];
		SetupTiles ();
		SetupCamera ();
		FillRandom ();
	}

	// Debugging purposes:
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
		{
			
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
	}

	// Update is called once per frame
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
		Camera.main.transform.position = new Vector3 ((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10);

		float aspectRatio = (float)Screen.width / (float)Screen.height;

		float verticalSize = (float)height / 2f + (float)borderSize;
		float horizontalSize = ((float)width / 2f + (float)borderSize) / aspectRatio;

		Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
	}


	GameObject GetRandomGamePiece ()
	{
		// Get random sprite
		int randomIdx = Random.Range (0, gamePiecePrefabs.Length);

		if (gamePiecePrefabs [randomIdx] == null) {
			Debug.LogWarning ("Board" + randomIdx + "does not contain valid GamePiece prefab");
		}

		return gamePiecePrefabs [randomIdx];
	}

	public void PlaceGamePiece (GamePiece gamePiece, int x, int y, int shape, int newLine)
	{
		if (gamePiece == null) {
			Debug.LogWarning ("Invalid GamePiece");
			return;
		}

		// Check if the piece needs to move into the field
		if (newLine == 1) {
			// fall in from top
			gamePiece.transform.position = new Vector3 (x, y+1, 0);
			gamePiece.transform.rotation = Quaternion.identity;
			gamePiece.Move (x, y, moveTime);

		} else if (newLine == 2) {
			// fall in from right
			gamePiece.transform.position = new Vector3 (x+1, y, 0);
			gamePiece.transform.rotation = Quaternion.identity;
			gamePiece.Move (x, y, moveTime);

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
        animationQueue.addAnimation(gamePiece, fadeTime);
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
				PlaceRandomPiece (i, j, 0);
			}
		}
	}

	void PlaceRandomPiece(int i, int j, int newLine) 
	{
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
				PlaceGamePiece (randomPiece.GetComponent<GamePiece> (), i, j, counter, newLine);
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
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, y, 1, 0);
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
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, y, 0, 0);
						newGamePiece.transform.parent = transform;
					}

					// Now check sides (maybe filling it)
					checkSides (x, y, 0);
				}
			}

			// Now check if the playingfield is full
			isFull();

			// Next turn :) 
			turn++;
		}

	}

	// Needs some work
	public void isFull(){
		int fullCount = 0;
		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				fullCount++;
			}
		}
		if (fullCount == height * width) {
			//Debug.Log ("Game over, playing field full!");
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
					if (y == (height - 1)) {
						// We got a winner folks! 
						Debug.Log("Full column " + x);
						removeColumn (x);
					}
				}
			}
		}
	}

	public void removeRow(int row){
		
		// First delete the row and set the grid to null for this row
		for (int x = 0; x < width; x++) {
			m_allGamePieces [x, row].FadeOut (fadeTime);
			m_allGamePieces [x, row] = null;
		}

		// Then move the next row
		if (row != height - 1) {
			for (int y = row+1; y < height; y++) {
				for (int x = 0; x < width; x++) {
					if (m_allGamePieces [x, y] != null) {
						// TODO: Might be going wrong here. 
						m_allGamePieces [x, y].Move (x, (y - 1), moveTime);

						// The type in the movingpiece is correct
						// Debug.Log (movingPiece.type);

						m_allGamePieces [x, (y - 1)] = m_allGamePieces[x,y];
						m_allGamePieces [x, (y - 1)].xIndex = x; // This is probably not needed
						m_allGamePieces [x, (y - 1)].yIndex = y-1;

						// Set the previous one to null
						m_allGamePieces [x, y] = null;

					}
				}
			}
		}

		// Now add a new row
		for (int x = 0; x < width; x++) {
			PlaceRandomPiece (x, height - 1, 0);
		}

	}

	public void removeColumn(int column){
		// First delete the column
		for (int y = 0; y < height; y++) {
			m_allGamePieces [column, y].FadeOut (fadeTime);
			m_allGamePieces [column, y] = null;
		}

		// Now move the rest
		if (column != width - 1) {
			for (int x = column + 1; x < width; x++) {
				for (int y = 0; y < height; y++) {
					if (m_allGamePieces [x, y] != null) {
						m_allGamePieces [x, y].Move ((x - 1), y, moveTime);
						m_allGamePieces [(x - 1), y] = m_allGamePieces [x, y];
						m_allGamePieces = null;
					}
				}
			}
		}

		// Now add a new column
		for (int y = 0; y < height; y++) {
			PlaceRandomPiece (width-1, y, 1);
		}
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

		checkFullRow ();
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
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), i, y, shape, 0);
						newGamePiece.transform.parent = transform;
					}

				} 
				if (m_allGamePieces [i, y].type == shape) {

					// Do nothing, go to the next shape
					
				} 
				if (m_allGamePieces [i, y].type != shape) {
					
					// Wrong shape detected, change it, then step out of the loo
					// Fade old piece
					GamePiece oldPiece = m_allGamePieces [i, y];
					oldPiece.FadeOut (fadeTime);
					m_allGamePieces [i, y] = null;

					GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

					if (newGamePiece != null) {
						newGamePiece.GetComponent<GamePiece> ().Init (this);
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), i, y, shape, 0);
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
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), i, y, shape, 0);
						newGamePiece.transform.parent = transform;
					}

				} 
				if (m_allGamePieces [i, y].type == shape) {
					// Do nothing, go to the next shape
				} 
				if (m_allGamePieces [i, y].type != shape) {

					// Wrong shape detected, change it, then step out of the loop
					// Fade old piece and set to null
					GamePiece oldPiece = m_allGamePieces [i, y];
					oldPiece.FadeOut (fadeTime);
					m_allGamePieces [i, y] = null;

					// Now place the new piece
					GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

					if (newGamePiece != null) {
						newGamePiece.GetComponent<GamePiece> ().Init (this);
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), i, y, shape, 0);
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
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, i, shape, 0);
						newGamePiece.transform.parent = transform;
					}
				}
				if (m_allGamePieces [x, i].type == shape) {
					// Do nothing, go to the next shape
				}
				if (m_allGamePieces [x, i].type != shape) {

					// Wrong shape detected, change it, then step out of the loop
					// Fade old piece and set to null
					GamePiece oldPiece = m_allGamePieces [x, i];
					oldPiece.FadeOut (fadeTime);
					m_allGamePieces [x, i] = null;

					// Now place the new piece
					GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

					if (newGamePiece != null) {
						newGamePiece.GetComponent<GamePiece> ().Init (this);
						PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, i, shape, 0);
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
							PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, i, shape, 0);
							newGamePiece.transform.parent = transform;
						}
					}
					if (m_allGamePieces [x, i].type == shape) {
						// Do nothing, go to the next shape
					}
					if (m_allGamePieces [x, i].type != shape) {

						// Wrong shape detected, change it, then step out of the loop
						// Fade old piece and set to null
						GamePiece oldPiece = m_allGamePieces [x, i];
						oldPiece.FadeOut (fadeTime);
						m_allGamePieces [x, i] = null;

						// Now place the new piece
						GameObject newGamePiece = Instantiate (gamePiecePrefabs [shape], Vector3.zero, Quaternion.identity) as GameObject;

						if (newGamePiece != null) {
							newGamePiece.GetComponent<GamePiece> ().Init (this);
							PlaceGamePiece (newGamePiece.GetComponent<GamePiece> (), x, i, shape, 0);
							newGamePiece.transform.parent = transform;
						}

						break;

					}
				}
			}
	}

	/* 
	 * --Old code--
	 * 

	public void DragToTile (Tile tile)
	{
        if (m_clickTile != null)
        {
            m_targetTile = tile;
        }
	}

	public void ReleaseTile ()
	{
        if(m_clickTile != null && m_targetTile != null)
        {
            SwitchTile(m_clickTile, m_targetTile);
        }

        m_clickTile = null;
        m_targetTile = null;
	}

	public void SwitchTile (Tile clickedTile, Tile targetTile)
	{
        GamePiece clickedPiece = m_allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
        GamePiece targetPiece = m_allGamePieces[targetTile.xIndex, targetTile.yIndex];

        clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
        targetPiece.Move(clickedPiece.xIndex, clickedPiece.yIndex, swapTime);
	}

	bool IsNextTo (Tile start, Tile end)
	{
		if (Mathf.Abs (start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex) {
			return true;
		}
		if (Mathf.Abs (start.yIndex = end.yIndex) == 1 && start.xIndex == end.xIndex) {
			return true;
		}
		return false;
	}
	
	*/

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayingField : MonoBehaviour {

    public GameObject shape;

    public Sprite rectangle;
    public Sprite triangle;
    public Sprite empty;

    static int fieldWidth = 6;
    static int fieldHeight = 6;

    // Set the turn and score
    int turn;
    float score;

    public int[,] matrix = new int[fieldHeight, fieldWidth];
    GameObject[,] playingField = new GameObject[fieldHeight, fieldWidth];

    // Use this for initialization
    void Start () {

        // Reset score and turn
        turn = 0;
        score = 0.0f;

		// Fill the playing field and draw the shapes
		FillMatrix ();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the user pressed the mouseButton
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("Pressed left click, casting ray.");
            CastRay();
        }
        // Draw the shapes (update them if needed)
		UpdateShapes();
    }

	void FillMatrix() {

		// Generate the matrix and the shapes in the playingfield
		for (int x = 0; x < fieldWidth; x++)
		{
			for (int y = 0; y < fieldHeight; y++)
			{
				matrix[x, y] = 0;

				// Create temp floats for the vector2 position
				float x_float = x;
				float y_float = y;
				playingField[x, y] = (GameObject)Instantiate(shape, new Vector2(x_float*0.55f, y_float*0.55f), Quaternion.identity);
			}
		}

		// Fill the matrix with the shapes
		for (int x = 0; x < fieldWidth; x++)
		{
			for (int y = 0; y < fieldHeight; y++)
			{
				// Fill the field with random generated shapes
				int tempValue = Random.Range(0, 100);
				if (tempValue < 50)
				{
					matrix[x, y] = 0;
				}
				else if (tempValue >= 50 && tempValue < 75)
				{
					matrix[x, y] = 1;
				}
				else
				{
					matrix[x, y] = 2;
				}                
			}
		}
	}

    // Process the mouseClick
    void CastRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
        if (hit)
        {
            //Debug.Log(hit.collider.gameObject.name);

            // Check if an empty spot is selected
            if (hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite == empty)
            {

                // Get the position of the object to adapt the matrix
                int tempPositionX = Mathf.RoundToInt(hit.collider.gameObject.transform.position.x / 0.55f);
                int tempPositionY = Mathf.RoundToInt(hit.collider.gameObject.transform.position.y / 0.55f);

                // Debug.Log("Position: " + tempPositionX + " " + tempPositionY + " Clicked");

                // Now change the matrix depending on the placed shape
                if ((turn % 2) == 0)
                {
                    // Place the new shape and fill the rows and columns accordingly
                    matrix[tempPositionX, tempPositionY] = 1;
                    CheckSides(tempPositionX, tempPositionY, 1);
                }
                else
                {
                    // Place the new shape and fill the rows and columns accordingly
                    matrix[tempPositionX, tempPositionY] = 2;
                    CheckSides(tempPositionX, tempPositionY, 2);
                }

                turn = turn + 1;
            }
            
        }
    }

	void UpdateShapes() {
		
		// Update the shapes according to changes in the matrix
		for (int x = 0; x < fieldWidth; x++)
		{
			for (int y = 0; y < fieldHeight; y++)
			{
				switch (matrix[x, y])
				{
				case 0:
					playingField[x, y].GetComponent<SpriteRenderer>().sprite = empty;
					break;
				case 1:
					playingField[x, y].GetComponent<SpriteRenderer>().sprite = rectangle;
					break;
				case 2:
					playingField[x, y].GetComponent<SpriteRenderer>().sprite = triangle;
					break;
				}
			}
		}

	}

    void CheckSides(int xInput, int yInput, int shapeCheck)
    {
        CheckRight(xInput, yInput, shapeCheck);
        CheckLeft(xInput, yInput, shapeCheck);
        CheckTop(xInput, yInput, shapeCheck);
        CheckBottom(xInput, yInput, shapeCheck);
    }

    void CheckRight(int xInput, int yInput, int shapeCheck)
    {
        // Check to the right
        if (xInput + 1 < fieldWidth && matrix[(xInput + 1), yInput] == shapeCheck)
        {
            // Same shape to the right detected
            bool check = true;
            int whileLoop = 1;

            while (check)
            {
                // Maak temp variables that correspond with the place in the matrix
                int x = xInput + whileLoop;
                int y = yInput;

                // Check if the right is the same or empty
                if ((x < fieldWidth) && (matrix[x, y] == shapeCheck || matrix[x, y] == 0))
                {
                    matrix[x, y] = shapeCheck;
                }
                else
                {
                    // Check if it has stopped because of end of the field or other shape
                    if (x < fieldWidth)
                    {
                        matrix[x, y] = shapeCheck;
                    }
                    check = false;
                }
                whileLoop++;
            }
            whileLoop = 0;
        }
        // Done checking to the right
    }

    void CheckLeft(int xInput, int yInput, int shapeCheck)
    {
        // Check to the left
        if (xInput - 1 >= 0 && matrix[(xInput - 1), yInput] == shapeCheck)
        {
            // Same shape to the left detected
            bool check = true;
            int whileLoop = 1;

            while (check)
            {
                // Maak temp variables that correspond with the place in the matrix
                int x = xInput - whileLoop;
                int y = yInput;

                // Check if the right is the same or empty
                if ((x >= 0) && (matrix[x, y] == shapeCheck || matrix[x, y] == 0))
                {
                    matrix[x, y] = shapeCheck;
                }
                else
                {
                    // Check if it has stopped because of end of the field or other shape
                    if (x < fieldWidth)
                    {
                        matrix[x, y] = shapeCheck;
                    }
                    check = false;
                }
                whileLoop++;
            }
            whileLoop = 0;
        }
        // Done checking to the left
    }

    void CheckTop(int xInput, int yInput, int shapeCheck)
    {
        // Check to the top
        if (yInput + 1 < fieldWidth && matrix[(xInput), yInput + 1] == shapeCheck)
        {
            // Same shape to the top detected
            bool check = true;
            int whileLoop = 1;

            while (check)
            {
                // Maak temp variables that correspond with the place in the matrix
                int x = xInput;
                int y = yInput + whileLoop;

                // Check if the right is the same or empty
                if ((y < fieldWidth) && (matrix[x, y] == shapeCheck || matrix[x, y] == 0))
                {
                    matrix[x, y] = shapeCheck;
                }
                else
                {
                    // Check if it has stopped because of end of the field or other shape
                    if (y < fieldWidth)
                    {
                        matrix[x, y] = shapeCheck;
                    }
                    check = false;
                }
                whileLoop++;
            }
            whileLoop = 0;
        }
        // Done checking to the top
    }

    void CheckBottom(int xInput, int yInput, int shapeCheck)
    {
        // Check to the bottom
        if (yInput - 1 >= 0 && matrix[(xInput), yInput + 1] == shapeCheck)
        {
            // Same shape to the bottom detected
            bool check = true;
            int whileLoop = 1;

            while (check)
            {
                // Maak temp variables that correspond with the place in the matrix
                int x = xInput;
                int y = yInput - whileLoop;

                // Check if the right is the same or empty
                if ((y >= 0) && (matrix[x, y] == shapeCheck || matrix[x, y] == 0))
                {
                    matrix[x, y] = shapeCheck;
                }
                else
                {
                    // Check if it has stopped because of end of the field or other shape
                    if (y < fieldWidth)
                    {
                        matrix[x, y] = shapeCheck;
                    }
                    check = false;
                }
                whileLoop++;
            }
            whileLoop = 0;
        }
        // Done checking to the bottom
    }

}

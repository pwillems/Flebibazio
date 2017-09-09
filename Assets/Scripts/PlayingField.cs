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

    // Update is called once per frame
    void Update()
    {

        // scoreText.text = "Score: " + turn;

        // Check if the user pressed the mouseButton
        if (Input.GetMouseButtonDown(0))
        {
            // Debug.Log("Pressed left click, casting ray.");
            CastRay();
        }

        // Draw the shapes (update them if needed)
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

                Debug.Log("Position: " + tempPositionX + " " + tempPositionY + " Clicked");

                // Now change the matrix depending on the placed shape
                if ((turn % 2) == 0)
                {
                    // For shape 1
                    matrix[tempPositionX, tempPositionY] = 1;

                    // Check to the right
                    if(tempPositionX < fieldWidth && matrix[tempPositionX+1, tempPositionY] == 1)
                    {
                        Debug.Log("Same icon to the RIGHT Detected!");

                        bool check = true;
                        int whileLoop = 0;

                        while(check)
                        {
                            Debug.Log("Next position: " + tempPositionX + whileLoop + " " + tempPositionY);
                            Debug.Log((tempPositionX + whileLoop < fieldWidth) + " " + (tempPositionX + whileLoop));

                            // Check if the right is the same or empty
                            if (tempPositionX+whileLoop < fieldWidth && (tempPositionX + whileLoop == 1 || tempPositionX + whileLoop == 0))
                            {
                                Debug.Log("Right is good");
                                matrix[tempPositionX+whileLoop, tempPositionY] = 1;
                            }
                            else
                            {
                                if(tempPositionX + whileLoop < fieldWidth)
                                {
                                    matrix[tempPositionX + whileLoop, tempPositionY] = 1;
                                }
                                check = false;
                            }
                            whileLoop++;
                        }

                        whileLoop = 0;
                    }
                }
                else
                {
                    // For shape 2
                    matrix[tempPositionX, tempPositionY] = 2;
                }

                turn = turn + 1;
            }
            
        }
    }

}

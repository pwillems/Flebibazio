using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour {

    public int xIndex;
    public int yIndex;
    public int type;

    bool m_isMoving = false;
	bool m_isFading = false;

    Board m_board;

    public InterpType interpolation = InterpType.SmootherStep;

    public enum InterpType
    {
        Linear,
        EaseOut,
        EasIn,
        SmoothStep,
        SmootherStep
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move((int)transform.position.x + 1, (int)transform.position.y, 0.5f);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move((int)transform.position.x - 1, (int)transform.position.y, 0.5f);
        }
    }

	public void SetCoord(int x, int y, int tempType)
    {
        xIndex = x;
        yIndex = y;
		type = tempType;
    }

    public void Init(Board board)
    {
        m_board = board;
    }//

    public void Move(int destX, int destY, float timeToMove)
    {
        if (!m_isMoving)
        {
            StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), timeToMove));
        }
    }

	public void FadeIn(float timeToFade){
		if (!m_isFading) {
			StartCoroutine (FadeInRoutine (timeToFade));
		}
	}

	public void FadeOut(float timeToFade){
		if (!m_isFading) {
            // TODO: This needs a check or something
		    StartCoroutine (FadeOutRoutine (timeToFade));
		}
	}

	IEnumerator FadeInRoutine (float fadeTime){
		// Get current spriteColor
		Color spriteColor = this.GetComponent<SpriteRenderer> ().color;

		bool fadeComplete = false;
		float elapsedTime = 0f;
		float alpha = spriteColor.a;

		m_isFading = true;

		while (!fadeComplete) {

			if (alpha > .99f) {
				// Fading is done
				fadeComplete = true;
				break;
			}

			elapsedTime += Time.deltaTime;

			//Fade from 0 to 1
			alpha = Mathf.Lerp(0, 1, elapsedTime / fadeTime);

			this.GetComponent<SpriteRenderer> ().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);

			yield return null;
		}

        m_isFading = false;

    }

	IEnumerator FadeOutRoutine (float timeToFade)
	{

        Debug.Log("SD " + xIndex + ", " + yIndex);

        Color spriteColor = this.GetComponent<SpriteRenderer> ().color; // Get current spriteColor
        bool fadeComplete = false;
		float elapsedTime = 0f;
		float alpha = spriteColor.a;

		m_isFading = true;

		while (!fadeComplete) {

			if (alpha < 0.01f) {
				// Fading is done
				fadeComplete = true;
				break;
			}

			elapsedTime += Time.deltaTime;

			//Fade from 1 to 0
			alpha = Mathf.Lerp(1, 0, elapsedTime / timeToFade);
			this.GetComponent<SpriteRenderer> ().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);
			yield return null;

		}

		if (fadeComplete == true) {
            // Now destroy
            Debug.Log("D " + xIndex + ", " + yIndex);
			Destroy(this.gameObject);
        }

        m_isFading = false;

    }

    IEnumerator MoveRoutine (Vector3 destination, float timeToMove)
    {
        Vector3 startPosition = transform.position;
        bool reachedDestination = false;
        float elapsedTime = 0f;

        m_isMoving = true;

        while (!reachedDestination)
        {
            // If we are close enough to destination
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                reachedDestination = true;
                break;
            }
            // track the total running time
            elapsedTime += Time.deltaTime;

            // calculate the Lerp value
            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            // Interpolation, for smoother movement (smoothstep)
            switch (interpolation)
            {
                case InterpType.Linear:
                    break;
                case InterpType.EasIn:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case InterpType.EaseOut:
                    t = 1 - Mathf.Cos(t * Mathf.PI * 0.05f);
                    break;
                case InterpType.SmoothStep:
                    t = t * t * (3 - 2 * t);
                    break;
                case InterpType.SmootherStep:
                    t = t * t * t * (t * (t * 6 - 15) + 10);
                    break;
            }
            
            // move the game piece
            transform.position = Vector3.Lerp(startPosition, destination, t);

            // wait until next frame
            yield return null;
        }

        m_isMoving = false;
    }
}

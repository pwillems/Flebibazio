using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour, ITileAnimation
{

    public int xIndex;
    public int yIndex;
    public int type;

    private GameObject sounds;

    bool m_isMoving = false;
    bool m_isFading = false;

    Board m_board;

    public InterpType interpolation = InterpType.SmootherStep;

    public AudioClip placementSound;

    public enum InterpType
    {
        Linear,
        EaseOut,
        EasIn,
        SmoothStep,
        SmootherStep
    }

    // Use this for initialization
    void Start()
    {
        sounds = GameObject.Find("SoundManager");
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    public void Move(int destX, int destY, float timeToMove)
    {
        if (!m_isMoving)
        {
            StartCoroutine(MoveRoutineTest(new Vector3(destX, destY, 0), timeToMove));
        }
    }

    public void FadeIn(float timeToFade)
    {
        if (!m_isFading)
        {
            StartCoroutine(FadeInRoutineTest(timeToFade));
            // Debug.Log("Start fading in position " + xIndex + ", " + yIndex);
            if(sounds != null)
            {
                SoundManager soundManager = (SoundManager)sounds.GetComponent(typeof(SoundManager));
                soundManager.PlayClipAtPoint(placementSound, Vector3.zero, soundManager.fxVolume);
            }
        }
    }

    public void FadeOut(float timeToFade)
    {
        // Debug.Log ("Fading " + xIndex + ", " + yIndex);
        StartCoroutine(FadeOutRoutineTest(timeToFade)); // Not correct, needs to be in the if(!misFading), but this does work
        if (!m_isFading)
        {
            // TODO: This needs a check or something
            // Debug.Log ("Actual fading of " + xIndex + ", " + yIndex);
            StartCoroutine(FadeOutRoutineTest(timeToFade));
        }
    }

    IEnumerator FadeInRoutine(float fadeTime, Action callback)
    {
        // Get current spriteColor
        Color spriteColor = this.GetComponent<SpriteRenderer>().color;

        bool fadeComplete = false;
        float elapsedTime = 0f;
        float alpha = spriteColor.a;

        m_isFading = true;

        while (!fadeComplete)
        {

            if (alpha > .99f)
            {
                // Fading is done
                fadeComplete = true;
                break;
            }

            elapsedTime += Time.deltaTime;

            //Fade from 0 to 1
            alpha = Mathf.Lerp(0, 1, elapsedTime / fadeTime);

            this.GetComponent<SpriteRenderer>().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);

            yield return null;

        }
        callback();
    }

    IEnumerator FadeInRoutineTest(float fadeTime)
    {
        // Get current spriteColor
        Color spriteColor = this.GetComponent<SpriteRenderer>().color;

        bool fadeComplete = false;
        float elapsedTime = 0f;
        float alpha = spriteColor.a;

        m_isFading = true;

        while (!fadeComplete)
        {

            if (alpha > .99f)
            {
                // Fading is done
                fadeComplete = true;
                break;
            }

            elapsedTime += Time.deltaTime;

            //Fade from 0 to 1
            alpha = Mathf.Lerp(0, 1, elapsedTime / fadeTime);

            this.GetComponent<SpriteRenderer>().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);

            yield return null;

        }
    }

    IEnumerator FadeOutRoutine(float timeToFade, Action callback)
    {
        // Get current spriteColor
        Color spriteColor = this.GetComponent<SpriteRenderer>().color;

        bool fadeComplete = false;
        float elapsedTime = 0f;
        float alpha = spriteColor.a;

        //Debug.Log ("Now fading, current alpha " + alpha + " current rgb " + spriteColor.r + " " + spriteColor.g + " " + spriteColor.b);

        m_isFading = true;

        while (!fadeComplete)
        {

            if (alpha < 0.01f)
            {
                // Fading is done
                //Debug.Log("Fading done");
                fadeComplete = true;
                break;
            }

            elapsedTime += Time.deltaTime;

            //Fade from 1 to 0
            alpha = Mathf.Lerp(1, 0, elapsedTime / timeToFade);

            this.GetComponent<SpriteRenderer>().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);

            yield return null;

        }

        if (fadeComplete == true)
        {
            Destroy(this.gameObject);
        }
        callback();
    }

    IEnumerator FadeOutRoutineTest(float timeToFade)
    {
        // Get current spriteColor
        Color spriteColor = this.GetComponent<SpriteRenderer>().color;

        bool fadeComplete = false;
        float elapsedTime = 0f;
        float alpha = spriteColor.a;

        //Debug.Log ("Now fading, current alpha " + alpha + " current rgb " + spriteColor.r + " " + spriteColor.g + " " + spriteColor.b);

        m_isFading = true;

        while (!fadeComplete)
        {

            if (alpha < 0.01f)
            {
                // Fading is done
                fadeComplete = true;
                break;
            }

            elapsedTime += Time.deltaTime;

            //Fade from 1 to 0
            alpha = Mathf.Lerp(1, 0, elapsedTime / timeToFade);

            this.GetComponent<SpriteRenderer>().color = new Color(spriteColor.r, spriteColor.g, spriteColor.b, alpha);

            yield return null;

        }

        if (fadeComplete == true)
        {
            Destroy(this.gameObject);
        }

    }

    IEnumerator MoveRoutine(Vector3 destination, float timeToMove, Action callback)
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
        callback();
    }

    IEnumerator MoveRoutineTest(Vector3 destination, float timeToMove)
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

    // Used in the AnimationQueue
    public void fadeIn(float fadeTime, Action callback)
    {
        StartCoroutine(FadeInRoutine(fadeTime, callback));
    }
    public void fadeOut(float fadeTime, Action callback)
    {
        StartCoroutine(FadeOutRoutine(fadeTime, callback));
    }
    public void movePiece(Vector3 pieceDestination, float moveTime, Action callback)
    {
        StartCoroutine(MoveRoutine(pieceDestination, moveTime, callback));
    }
}
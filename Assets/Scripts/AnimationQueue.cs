using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationQueue : MonoBehaviour {

    private List<ITileAnimation> animationList = new List<ITileAnimation>();
    private List<float> animationTimes = new List<float>();
    private List<int> tileAnimationType = new List<int>();
    private List<Vector3> destinationList = new List<Vector3>();
    private bool canRun = true;

    // Types of animations:
    // Move = 0, FadeIn = 1, FadeOut = 2

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (canRun && animationList.Count > 0 && animationTimes.Count > 0 && tileAnimationType.Count > 0)
        {
            canRun = false;
            if (tileAnimationType[0]==1) {
                // The animation should be a fade in
                animationList[0].fadeIn(animationTimes[0], () => { //Dit is een callback System.Action, check de docs daar maar op.
                    canRun = true;
                });
            }
            else if (tileAnimationType[0] == 2)
            {
                // The animation should be a fade out
                animationList[0].fadeOut(animationTimes[0], () => { 
                    canRun = true;
                });
            }
            else if (tileAnimationType[0] == 0)
            {
                // The animation should be a move, TODO
                animationList[0].movePiece(destinationList[0], animationTimes[0], () => { //Dit is een callback System.Action, check de docs daar maar op.
                    canRun = true;
                });
            }
            animationList.RemoveAt(0);
            animationTimes.RemoveAt(0);
            tileAnimationType.RemoveAt(0);
            destinationList.RemoveAt(0);
        }
    }

    // Add a fade animation to the queue
    public void addFadeAnimation(ITileAnimation tileAnimation, float fadeTime, int animationType)
    {
        animationTimes.Add(fadeTime);
        animationList.Add(tileAnimation);
        tileAnimationType.Add(animationType);
        destinationList.Add(new Vector3(0,0,0));
    }

    // Add a move animation to the queue
    public void addMoveAnimation(ITileAnimation tileAnimation, float movingTime, Vector3 pieceDestination)
    { 
        animationTimes.Add(movingTime);
        animationList.Add(tileAnimation);
        destinationList.Add(pieceDestination);
        tileAnimationType.Add(0);
    }
}

public interface ITileAnimation
{
    void fadeIn(float fadeInTime, System.Action callback);
    void fadeOut(float fadeInTime, System.Action callback);
    void movePiece(Vector3 destination, float moveTime, System.Action callback);
}
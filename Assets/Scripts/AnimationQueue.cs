using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationQueue : MonoBehaviour {

    private List<ITileAnimation> animationList = new List<ITileAnimation>();
    private List<float> animationTimes = new List<float>();
    private List<int> tileAnimationType = new List<int>();
    private List<int> rowSize = new List<int>();
    private List<Vector3> destinationList = new List<Vector3>();
    private bool canRun = true;
    private int rowSizeTemp = 0;
    private int counter = 0;

    // Types of animations:
    // Move = 0, FadeIn = 1, FadeOut = 2

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (canRun && animationList.Count > 0 && animationTimes.Count > 0 && tileAnimationType.Count > 0)
        {
            rowSizeTemp = 0;
            counter = 0;
            canRun = false;
            if (tileAnimationType[0] == 0)
            {
                // The animation should be a move, TODO
                animationList[0].movePiece(destinationList[0], animationTimes[0], () => { //This is a callback System.Action
                    canRun = true;
                });
                removeLists();
            }
            else if (tileAnimationType[0]==1) {
                // The animation should be a fade in
                animationList[0].fadeIn(animationTimes[0], () => { 
                    canRun = true;
                });
                removeLists();
            }
            else if (tileAnimationType[0] == 2)
            {
                // The animation should be a fade out
                animationList[0].fadeOut(animationTimes[0], () => { 
                    canRun = true;
                });
                removeLists();
            }
            // TODO: This doesn't feel like the optimal solution. 
            else if (tileAnimationType[0] == 3)
            {
                rowSizeTemp = rowSize[0];
                // The whole row should fade
                for (int i = 0; i < rowSizeTemp; i++) {
                    // The animation should be a fade out
                    animationList[i].fadeOut(animationTimes[i], () => {
                        // Count the total amount of fades and see if we have reached the last one by comparing it to the total row size
                        counter++;
                        if (counter == rowSizeTemp) {
                            // This is the last fade, so afterwards set canRun on true
                            canRun = true;
                        }
                    });
                }
                for (int i = 0; i < rowSizeTemp; i++)
                {
                    removeLists();
                }
            }
            // TODO: This doesn't feel like the optimal solution. 
            else if (tileAnimationType[0] == 4)
            {
                rowSizeTemp = rowSize[0];

                // The whole row should move
                for (int i = 0; i < rowSizeTemp; i++)
                {
                    // The animation should be a fade out
                    animationList[i].movePiece(destinationList[i], animationTimes[i], () => {
                        // Count the total amount of fades and see if we have reached the last one by comparing it to the total row size
                        counter++;
                        if (counter == rowSizeTemp)
                        {
                            // This is the last fade, so afterwards set canRun on true
                            canRun = true;
                        }
                    });
                }
                for (int i = 0; i < rowSizeTemp; i++)
                {
                    removeLists();
                }
            }
        }
    }

    // Remove the top one from all animation lists
    public void removeLists()
    {
        animationList.RemoveAt(0);
        animationTimes.RemoveAt(0);
        tileAnimationType.RemoveAt(0);
        destinationList.RemoveAt(0);
        rowSize.RemoveAt(0);
    }

    // Add a fade animation to the queue
    public void addFadeAnimation(ITileAnimation tileAnimation, float fadeTime, int animationType)
    {
        animationTimes.Add(fadeTime);
        rowSize.Add(0);
        animationList.Add(tileAnimation);
        tileAnimationType.Add(animationType);
        destinationList.Add(new Vector3(0,0,0));
    }

    // Add a move animation to the queue
    public void addMoveAnimation(ITileAnimation tileAnimation, float movingTime, Vector3 pieceDestination)
    { 
        animationTimes.Add(movingTime);
        rowSize.Add(0);
        animationList.Add(tileAnimation);
        destinationList.Add(pieceDestination);
        tileAnimationType.Add(0);
    }
    // Add a row fade animation to the queue
    // TODO: This doesn't feel like the optimal solution. 
    public void addFadeAnimationBatch(ITileAnimation tileAnimation, float fadeTime, int animationType, int fieldSize)
    {
        animationTimes.Add(fadeTime);
        rowSize.Add(fieldSize);
        animationList.Add(tileAnimation);
        tileAnimationType.Add(animationType);
        destinationList.Add(new Vector3(0, 0, 0));
    }
    // Add a row move animation to the queue
    // TODO: This doesn't feel like the optimal solution. 
    public void addMoveAnimationBatch(ITileAnimation tileAnimation, float movingTime, Vector3 pieceDestination, int fieldSize)
    {
        animationTimes.Add(movingTime);
        rowSize.Add(fieldSize);
        animationList.Add(tileAnimation);
        destinationList.Add(pieceDestination);
        tileAnimationType.Add(4);
    }
}

public interface ITileAnimation
{
    void fadeIn(float fadeInTime, System.Action callback);
    void fadeOut(float fadeInTime, System.Action callback);
    void movePiece(Vector3 destination, float moveTime, System.Action callback);
}
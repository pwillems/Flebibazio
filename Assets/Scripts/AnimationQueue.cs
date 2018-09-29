using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationQueue : MonoBehaviour {

    private List<ITileAnimation> animationList = new List<ITileAnimation>();
    private List<float> animationTimes = new List<float>();
    private bool canRun = true;
   

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (canRun && animationList.Count > 0 && animationTimes.Count > 0)
        {
            canRun = false;
            animationList[0].fadeIn(animationTimes[0], () => { //Dit is een callback System.Action, check de docs daar maar op.
                canRun = true;
            });
            animationList.RemoveAt(0);
            animationTimes.RemoveAt(0);
        }
    }

    public void addAnimation(ITileAnimation tileAnimation, float fadeTime)
    {
        animationTimes.Add(fadeTime);
        animationList.Add(tileAnimation);
    }
}

public interface ITileAnimation
{
    void fadeIn(float fadeInTime, System.Action callback);
}
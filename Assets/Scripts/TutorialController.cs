using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{

    public Text tutorialTextHolder;
    public string opener = "Welcome to testing Pim's Shape Game, click on a tile to continue";
    public string[] steps;

    public int stepCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Set the welcome text
        tutorialTextHolder.text = opener;
    }

    public void NextStep()
    {
        Debug.Log("NExt step");
        // Show the next explanation, then move a step up
        tutorialTextHolder.text = steps[stepCounter];
        stepCounter++;
    }
}

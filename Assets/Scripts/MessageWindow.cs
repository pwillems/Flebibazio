using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectXformMover))]
public class MessageWindow : MonoBehaviour
{
    public Image messageIcon;
    public Text messageText;
    public Text buttonText;

    public void ShowMessage(Sprite sprite = null, string message = "", string buttonMsg = "Start")
    {
        if(messageIcon!= null)
        {
            messageIcon.sprite = sprite;
        }

        if(message!= null)
        {
            messageText.text = message;
        }

        if(buttonMsg != null)
        {
            buttonText.text = buttonMsg;
        }
    }

}

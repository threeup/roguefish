using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum ButtonType
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    ROTATE_LEFT,
    ROTATE_RIGHT,
    ACTION_ONE,
    ACTION_TWO,
}

public class UIButton : MonoBehaviour
{
    public ButtonType btype;
    public Text textObject;
    public string textString;

    public void Initialize()
    {
        textObject.text = textString;
    }
}
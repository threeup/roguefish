using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIButtonSet : UIEntity {


    public static UIButtonSet Instance;

    public List<UIButton> buttons;
    
    public override void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("UIButtonSet Already exists");
        }
        Instance = this;
    }


    public override void Initialize()
    {
        buttons.AddRange(GetComponentsInChildren<UIButton>() as UIButton[]);
        
        foreach(UIButton button in buttons)
        {
            button.Initialize();
        }
        Vector2 canvasAnchor = (container.transform.parent as RectTransform).anchoredPosition;
        Vector2 containerAnchor = (container.transform as RectTransform).anchoredPosition;
        containerRect = (container.transform as RectTransform).rect;
		containerRect.x += containerAnchor.x + canvasAnchor.x;
		containerRect.y += containerAnchor.y + canvasAnchor.y;
        base.Initialize();

    }

    public void OnPress(UIButton btn)
    {
        switch(btn.btype)
        {
            case ButtonType.UP:
                break;
            case ButtonType.DOWN:
                break;
            case ButtonType.LEFT:
                break;
            case ButtonType.RIGHT:
                break;
            case ButtonType.ROTATE_LEFT:
                break;
            case ButtonType.ROTATE_RIGHT:
                break;
            case ButtonType.ACTION_ONE:
                GameMgr.Instance.ModifyScore(5);
                break;
            case ButtonType.ACTION_TWO:
                GameMgr.Instance.FinalizeScore();
                break;
        }
        Debug.Log("Button"+btn.btype);
        return;
    }
}

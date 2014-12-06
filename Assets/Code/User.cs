
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class ContentMgr
{
    private static int nextContentUID = 0;
    public static int GetContentUID()
    {
        nextContentUID++;
        return nextContentUID;
    }

}

public class User : MonoBehaviour
{
    public Actor actor;
    private List<UIProperties> commandUI;
    private bool commandDirty = false;

    public void Initialize()
    {
        actor = null;
        commandUI = new List<UIProperties>();
        commandDirty = true;
        FillDefault();

    }

    public void FillDefault()
    {
        
    }

    public List<UIProperties> GetCommandUI()
    {
        if (commandDirty)
        {
            commandUI.Clear();
            commandUI.Add(new UIProperties("SHOOT", 0));
            commandUI.Add(new UIProperties("MOVE", 1));
            commandUI.Add(new UIProperties("WAIT", 2));
            commandDirty = false;
        }
        return commandUI;
    }
}

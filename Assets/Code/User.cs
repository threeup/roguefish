
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class User : MonoBehaviour
{
    public BoatActor boat;
    public List<NormalActor> actorList;
    private List<UIProperties> commandUI;
    private bool commandDirty = false;

    public void Initialize()
    {
        boat = null;
        actorList = new List<NormalActor>();
        commandUI = new List<UIProperties>();
        commandDirty = true;

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

    public void ProcessInput(int count, Vector2[] current, Vector2[] source)
    {
        if (boat != null)
        {
            if (count > 0)
            {
                Vector2 fieldPos = World.Instance.ScreenToField(current[0]);
                boat.ProcessInput(fieldPos);
            }
            else
            {
                boat.ProcessInput(null);
            }
        }
    }
}

using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using System.Linq;

public class World : MonoBehaviour {

    public static World Instance;
    public GeneralMachine machine;

    public Vector2 centerPos = Vector2.zero;
    public int currentWidth = 0;
    public int currentHeight = 0;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("Maker Already exists");
        }
        Instance = this;
    }


    public void Initialize()
    {
        machine = new GeneralMachine();
        machine.Initialize(this);
        machine.AddEnterListener(OnNotReady);
        machine.AddEnterListener(OnReady);
        machine.AddEnterListener(OnActive);
        machine.AddEnterListener(OnBusy);
        
        machine.SetState(GeneralState.READY);
    }

    public void OnNotReady(object owner)
    {

    }


    public void OnReady(object owner)
    {
        machine.SetState(GeneralState.ACTIVE);   
    }

    public void OnActive(object owner)
    {
        

    }

    public void OnBusy(object owner)
    {

    }


	public Actor GetActorByActorUID(int uid)
    {
        return null;
    }
}

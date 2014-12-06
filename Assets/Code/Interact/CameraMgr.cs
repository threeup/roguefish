using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;

public enum CameraState
{
    NOTREADY,
    READY,
    BUSY,
}

[System.Serializable]
public class CameraMachine : StateMachine<CameraState>
{
}

public class CameraMgr : MonoBehaviour {

    public static CameraMgr Instance;
    public CameraMachine machine;

    private bool isInit = false;
    private bool moveReady = false;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("CameraMgr Already exists");
        }
        Instance = this;
    }


    public void Initialize()
    {
        machine = new CameraMachine();
        machine.Initialize(this);
        machine.AddEnterListener(OnNotReady);
        machine.AddEnterListener(OnReady);
        machine.AddUpdateListener(UpdateReady);
        machine.AddEnterListener(OnBusy);
        machine.AddUpdateListener(UpdateBusy);
        
        machine.SetState(CameraState.BUSY);
        isInit = true;
    }
    
    public void OnNotReady(object owner)
    {
        moveReady = false;
    }


    public void OnReady(object owner)
    {
        moveReady = true;
    }

    public void OnActive(object owner)
    {
        
    }


    public void OnBusy(object owner)
    {
        
    }

    void Update() 
    {
        if (!isInit)
        {
            return;
        }
        machine.MachineUpdate(Time.deltaTime);
    }

    void UpdateReady(float deltaTime)
    {

    }



    void UpdateBusy(float deltaTime)
    {
        
    }


    public bool IsBusy()
    {
        return machine.IsState(CameraState.BUSY);
    }

}

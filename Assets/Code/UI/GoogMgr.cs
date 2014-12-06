using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;

public enum GoogState
{
    NOTREADY,
    READY,
    WAITING,
    ACTIVE,
    BUSY,
    FAIL,
    DISABLED,
}

public enum GoogJob
{
    NONE,
    AUTH,
    LEADERBOARD,
}

[System.Serializable]
public class GoogMachine : StateMachine<GoogState>
{
}

public class GoogMgr {

    public static GoogMgr Instance;
    public GoogMachine machine;

    public GoogJob waitingFor = GoogJob.NONE; 

    public string leaderboardID = "CgkI-vTL3rsWEAIQAQ";

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("GoogManger Already exists");
        }
        Instance = this;
    }


    public void Initialize()
    {
        machine = new GoogMachine();
        machine.Initialize(this);
        machine.AddEnterListener(OnNotReady);
        machine.AddEnterListener(OnReady);
        machine.AddEnterListener(OnWaiting);
        machine.AddEnterListener(OnActive);
        machine.AddEnterListener(OnBusy);
        machine.AddEnterListener(OnFail);
        machine.AddEnterListener(OnDisabled);
		machine.AddChangeListener(OnChange);
        
        machine.SetState(GoogState.READY);
    }
    public void Authenticate()
    {
        waitingFor = GoogJob.AUTH;
        machine.SetState(GoogState.WAITING);
    }

    public void ReportScore(int score)
    {
        if (!machine.IsState(GoogState.ACTIVE))
        {
            return;
        }
        machine.SetState(GoogState.BUSY);
        Social.ReportScore (score, leaderboardID, success => {
            Debug.Log("ReportScore="+success);
            machine.SetState(GoogState.ACTIVE);
        });
    }

	public void OnChange(int val	)
	{
		Dbg.Instance.SetLabel(0, "Goog "+((GoogState)machine.GetActiveState()).ToString());
	}

    public void OnNotReady(object owner)
    {

    }


    public void OnReady(object owner)
    {
        // Select the Google Play Games platform as our social platform implementation
        PlayGamesPlatform.Activate();
    }

    public void OnWaiting(object owner)
    {
        if (waitingFor == GoogJob.AUTH)
        {
            if (Social.localUser.authenticated) {
                machine.SetState(GoogState.ACTIVE);
            } else {
                Social.localUser.Authenticate((bool success) => {
                    machine.SetState(success ? GoogState.ACTIVE : GoogState.FAIL);
                });
            }
        }
    }

    public void OnActive(object owner)
    {

    }

    public void OnBusy(object owner)
    {

    }

    public void OnFail(object owner)
    {

    }

    public void OnDisabled(object owner)
    {
        ((PlayGamesPlatform) Social.Active).SignOut();
    }


    public bool IsActive()
    {
        return machine != null && machine.IsState(GoogState.ACTIVE);
    }

    public void PostLeaderboard(int val)
    {
         if (Social.localUser.authenticated)
         {
            waitingFor = GoogJob.LEADERBOARD;
            machine.SetState(GoogState.WAITING);
            Social.ReportScore(val, leaderboardID, (bool success) => {
                if (success)
                {
                ((PlayGamesPlatform)Social.Active).ShowLeaderboardUI(leaderboardID);
                }
                else
                {
                //Debug.Log("Login failed for some reason");
                }
            });
         }
    }
}

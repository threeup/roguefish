
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class GameMgr : MonoBehaviour {

    public static GameMgr Instance;
    
    public World world; //wiredup

    public GoogMgr googMgr;
    public Dbg dbg; //wiredup
    public UserMgr userMgr; //wiredup
    public InputMgr inputMgr; //wiredup
    public CameraMgr cameraMgr; //wiredup
    public UIMgr uiMgr; //wiredup

	public delegate void InitDelegate();
    private float timer = 0.0f;
    private bool isStart = false;
    private bool isInit = false;
	private List<InitDelegate> inits;
	private int initIdx = 0;
	private InitDelegate nextInit;

    public int score;


	public void Start()
	{
        if (Instance != null)
        {
            Debug.LogError("GameMgr Already exists");
        }
        Instance = this;

		inits = new List<InitDelegate>();
		inits.Add(InitDebug);
        inits.Add(InitUser);
        inits.Add(InitWorld);
		inits.Add(InitCamera);
        inits.Add(InitGoog);
		inits.Add(InitInput);
        inits.Add(InitUI);
		isStart = true;
	}

    public void Update()
    {
		if (isStart && !isInit)
    	{
    		timer++;
    		if (timer > 0.1f)
    		{
				inits[initIdx]();
				initIdx++;
				timer = 0;
    		}
			if (initIdx >= inits.Count)
			{
				isInit = true;
			}
    	}
    }

    public void InitGoog()
    {
        bool useGoog = false;
        if (useGoog && Application.platform == RuntimePlatform.Android)
        {
            googMgr = new GoogMgr();
            googMgr.Initialize();
            googMgr.Authenticate();
        }
	}

	public void InitDebug()
	{
		if (dbg != null)
		{
        	dbg.Initialize();
		}
    }

    public void InitInput()
    {
    	inputMgr.Initialize();
    }

    public void InitCamera()
    {
    	cameraMgr.Initialize();
    }

    public void InitUser()
    {
        userMgr.Initialize();
    }

    public void InitWorld()
    {
        world.Initialize();
    }

    public void InitUI()
    {
        uiMgr.Initialize();
    }

    public void ModifyScore(int val)
    {
        score += val;
    }

    public void FinalizeScore()
    {
        if (googMgr != null && googMgr.IsActive())
        {
            Debug.Log("post"+score);
            googMgr.PostLeaderboard(score);
        }
        else
        {
            Debug.Log("Cant post");
        }
    }
}

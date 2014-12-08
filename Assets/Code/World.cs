using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;
using System.Linq;

public class World : MonoBehaviour {

    public List<Entity> entities;
    public Canvas playField;
    public Canvas PlayField { get { return playField; } }
    public static World Instance;
    public GeneralMachine machine;

    public int level = 0;
    public int whaleCount = 0;

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
        entities = new List<Entity>();

        machine = new GeneralMachine();
        machine.Initialize(this);
        machine.AddEnterListener(OnNotReady);
        machine.AddEnterListener(OnReady);
        machine.AddEnterListener(OnActive);
        machine.AddEnterListener(OnBusy);
        
        machine.SetState(GeneralState.READY);

        Utilities.SortChildren(playField.gameObject);
    }

    public void OnNotReady(object owner)
    {

    }


    public void OnReady(object owner)
    {
        AdvanceLevel();

        machine.SetState(GeneralState.ACTIVE);   
    }

    private Actor MakeBoat()
    {
        Actor actor = FactoryEntity.Instance.GetBoatActor(Constants.BoatData);
        actor.name = "Boat";
        actor.WarpTo(Vector3.up*156f);
        
        actor.transform.SetParent(playField.transform, false);
        actor.TurnOn();
        Ability hook = new Ability(Constants.HookData, "HOOK");
        actor.abilities.Add(hook);
        
        GameObject vitalObj = new GameObject("VitalsBoat");
        vitalObj.transform.SetParent(playField.transform, false);
        Vitals vitals = vitalObj.AddComponent<Vitals>();
        vitals.Initialize(actor);
        return actor;
    }

    private Actor MakeCloud()
    {
        Actor actor = FactoryEntity.Instance.GetBoatActor(Constants.CloudData);
        actor.name = "Cloud";
        actor.WarpTo(Vector3.up*256f);
        actor.transform.SetParent(playField.transform, false);
        actor.maxWeapons = 9;
        actor.TurnOn();
        Ability rain = new Ability(Constants.RainData, "RAIN");
        actor.abilities.Add(rain);

        Ability lightning = new Ability(Constants.LightningData, "LIGHTNING");
        actor.abilities.Add(lightning);

        AIAgent cloudAI = actor.gameObject.AddComponent<AIAgent>();
        cloudAI.Initialize(actor, AIAgent.AgentType.ATTACK);

        actor.collider.enabled = false;
        return actor;
    }

    private Actor MakeFish(int i, EntityProperties fishData)
    {
        Actor actor = FactoryEntity.Instance.GetNormalActor(fishData);
        actor.name = "Fish"+i;
        Vector2 pos = Vector2.zero;
        pos.x = (i%2 == 0 ? 1f : -1f)*612f;
        pos.y = UnityEngine.Random.Range(-300f, 110f);
        actor.WarpTo(pos);
        pos.x = -pos.x;
        actor.GoalTo(pos);
        actor.transform.SetParent(playField.transform, false);
        actor.TurnOn();
        AIAgent fishAI = actor.gameObject.AddComponent<AIAgent>();
        fishAI.Initialize(actor, AIAgent.AgentType.WANDER);
        return actor;
    }

    private Actor MakeButton(string name, ImageProperties imgProp, Vector2 pos)
    {
        EntityProperties eprop = Constants.FishData;
        eprop.hp = 1;
        eprop.imgProp = imgProp;
        Actor actor = FactoryEntity.Instance.GetNormalActor(eprop);
        actor.name = "Button"+name;
        actor.WarpTo(pos);
        actor.transform.SetParent(playField.transform, false);
        actor.TurnOn();
        return actor;
    }


    public void OnActive(object owner)
    {
        

    }

    public void OnBusy(object owner)
    {

    }

    public void Register(Entity entity)
    {
        entities.Add(entity);
    }
    public void Deregister(Entity entity)
    {
        entities.Remove(entity);   
    }

	public Actor GetActorByActorUID(int uid)
    {
        return null;
    }

    public Vector2 ScreenToField(Vector2 pos)
    {
        float scaleX = 1024f/playField.pixelRect.width;
        float scaleY = 768f/playField.pixelRect.height;
        float centerX = pos.x-playField.pixelRect.width/2f;
        float centerY = pos.y-playField.pixelRect.height/2f;
        return new Vector2(centerX*scaleX, centerY*scaleY);
    }

    public void Update()
    {
        BoatActor userBoat = UserMgr.Instance.GetBoat(1);
        if (level > 1 && userBoat != null && userBoat.HP <= 0 && whaleCount < 2)
        {
            userBoat.HP = 2;
            Actor fish = MakeFish(999, Constants.BigWhaleData);
            UserMgr.Instance.AssignActor(2, fish);
            whaleCount++;
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            level = 0;
            AdvanceLevel();
        }
    }

    public void ParentToField(Transform transform)
    {
        transform.SetParent(playField.transform, false);
        Utilities.SortChildren(playField.gameObject);
    }

    public void AdvanceLevel()
    {
        level++;
        whaleCount = 0;
        UserMgr.Instance.Purge(0);
        if (level == 1)
        {
            UserMgr.Instance.Purge(1);
            UserMgr.Instance.Purge(2);
        
            Actor boat = MakeBoat();
            UserMgr.Instance.AssignActor(1, boat);

            Actor start = MakeButton("Start", Constants.StartImgData, new Vector2(-100,0));
            start.OnAttach = GoStart;
            UserMgr.Instance.AssignActor(0, start);
            Actor exit = MakeButton("Exit", Constants.QuitImgData, new Vector2(100,0));
            exit.OnAttach = GoExit;
            UserMgr.Instance.AssignActor(0, exit);
        }
        if (level >= 2)
        {
            BoatActor userBoat = UserMgr.Instance.GetBoat(1);
            Actor cloud = MakeCloud();
            UserMgr.Instance.AssignActor(0, cloud);

            int maxFish = level*10;
            if (userBoat != null)
            {
                userBoat.healthDecayRate = 10f/level;
                userBoat.progressRemaining = 4*level;
            }
            int i=0;
            
            int type1 = (int)Mathf.Round(maxFish*0.4f);
            int type2 = type1 + (int)Mathf.Round(maxFish*0.2f);
            int type3 = type2 + (int)Mathf.Round(maxFish*0.2f);
            int type4 = type3 + (int)Mathf.Round(maxFish*0.2f);
            for(; i<type1; ++i)
            {
                Actor fish = MakeFish(i, Constants.FishData);
                UserMgr.Instance.AssignActor(2, fish);
            }
            for(; i<type2; ++i)
            {
                Actor fish = MakeFish(i, Constants.AngelFishData);
                UserMgr.Instance.AssignActor(2, fish);
            }
            for(; i<type3; ++i)
            {
                Actor fish = MakeFish(i, Constants.BootData);
                UserMgr.Instance.AssignActor(2, fish);
            }
            for(; i<type4; ++i)
            {
                Actor fish = MakeFish(i, Constants.TurtleData);
                UserMgr.Instance.AssignActor(2, fish);
            }
        }
    }

    void GoStart(Entity parent)
    {
        if (parent != null)
        {
            BoatActor boat = UserMgr.Instance.GetBoat(1);
            if (boat != null)
            {
                boat.progressRemaining = 0;
                boat.CheckAdvance();
            }
        }
    }

    void GoExit(Entity parent)
    {
        if (parent != null)
        {
            Debug.Log("Exit");
        }
    }

}

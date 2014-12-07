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
    }

    public void OnNotReady(object owner)
    {

    }


    public void OnReady(object owner)
    {

        Actor boat = MakeBoat();
        UserMgr.Instance.AssignActor(0, boat);

        AdvanceLevel();

        machine.SetState(GeneralState.ACTIVE);   
    }

    private Actor MakeBoat()
    {
        EntityProperties eprop = new EntityProperties();
        eprop.HP = 10;
        eprop.imgProp = new ImageProperties(Constants.BoatRect);
        Actor actor = FactoryEntity.Instance.GetBoatActor(eprop);
        actor.name = "Boat";
        actor.WarpTo(Vector3.up*256f);
        actor.transform.localScale = Vector3.one*2f;
        actor.transform.SetParent(playField.transform, false);
        return actor;
    }

    private Actor MakeFish(int i)
    {
        EntityProperties eprop = new EntityProperties();
        eprop.HP = 10;
        eprop.imgProp = new ImageProperties(Constants.FishRect);
        Actor actor = FactoryEntity.Instance.GetGenericActor(eprop);
        actor.name = "Fish"+i;
        Vector2 pos = Vector2.zero;
        pos.x = (i%2 == 0 ? 1f : -1f)*512f;
        pos.y = UnityEngine.Random.Range(-300f, 300f);
        actor.WarpTo(pos);
        pos.x = -pos.x;
        actor.GoalTo(pos);
        actor.transform.localScale = Vector3.one*0.75f;
        actor.transform.SetParent(playField.transform, false);

        AIAgent fishAI = actor.gameObject.AddComponent<AIAgent>();
        fishAI.Initialize(actor);
        return actor;
    }

    private Actor MakeButton(string name, float posX)
    {
        EntityProperties eprop = new EntityProperties();
        eprop.HP = 1;
        eprop.imgProp = new ImageProperties(Constants.ButtonRect);
        Actor buttonActor = FactoryEntity.Instance.GetGenericActor(eprop);
        actor.name = "Button"+name;
        Vector2 pos = Vector2.zero;
        pos.x = posX;
        pos.y = -200f;
        actor.WarpTo(pos);
        actor.transform.localScale = Vector3.one*0.75f;
        actor.transform.SetParent(playField.transform, false);

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
        float deltaTime = Time.deltaTime;
        foreach(Entity entity in entities)
        {
            entity.UpdateEntity(deltaTime);
        }
    }

    public void ParentToField(Transform transform)
    {
        transform.SetParent(playField.transform, false);
        Utilities.SortChildren(playField.gameObject);
    }

    void AdvanceLevel()
    {
        level++;
        if (level == 1)
        {
            Actor start = MakeButton("Start", -100);
            UserMgr.Instance.AssignActor(0, start);
            Actor exit = MakeButton("Exit", 150);
            UserMgr.Instance.AssignActor(0, exit);
        }
        if (level == 2)
        {
            for(int i=0; i<20; ++i)
            {
                Actor fish = MakeFish(i);
                UserMgr.Instance.AssignActor(2, fish);
            }
        }
    }
}

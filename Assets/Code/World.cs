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

        for(int i=0; i<20; ++i)
        {
            Actor fish = MakeFish(i);
            UserMgr.Instance.AssignActor(1, fish);
        }
        
        machine.SetState(GeneralState.ACTIVE);   
    }

    private Actor MakeBoat()
    {
        EntityProperties boatProp = new EntityProperties();
        boatProp.HP = 10;
        boatProp.imgProp = new ImageProperties(Constants.BoatRect);
        Actor boatActor = FactoryEntity.Instance.GetBoatActor(boatProp);
        boatActor.name = "Boat";
        boatActor.WarpTo(Vector3.up*256f);
        boatActor.transform.localScale = Vector3.one*2f;
        boatActor.transform.SetParent(playField.transform, false);
        return boatActor;
    }

    private Actor MakeFish(int i)
    {
        EntityProperties fishProp = new EntityProperties();
        fishProp.HP = 10;
        fishProp.imgProp = new ImageProperties(Constants.FishRect);
        Actor fishActor = FactoryEntity.Instance.GetFishActor(fishProp);
        fishActor.name = "Fish"+i;
        Vector2 pos = Vector2.zero;
        pos.x = (i%2 == 0 ? 1f : -1f)*512f;
        pos.y = UnityEngine.Random.Range(-300f, 300f);
        fishActor.WarpTo(pos);
        pos.x = -pos.x;
        fishActor.GoalTo(pos);
        fishActor.transform.localScale = Vector3.one*0.75f;
        fishActor.transform.SetParent(playField.transform, false);

        AIAgent fishAI = fishActor.gameObject.AddComponent<AIAgent>();
        fishAI.Initialize(fishActor);
        return fishActor;
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
}

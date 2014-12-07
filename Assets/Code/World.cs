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
        EntityProperties boatProp = new EntityProperties();
        boatProp.HP = 10;
        boatProp.imgProp = new ImageProperties(Constants.BoatRect);
        Actor boatActor = FactoryEntity.Instance.GetActor(boatProp);
        boatActor.name = "Boat";
        boatActor.WarpTo(Vector3.up*256f);
        boatActor.transform.localScale = Vector3.one*2f;
        boatActor.transform.SetParent(playField.transform, false);
        

        UserMgr.Instance.AssignActor(0, boatActor);
        
        machine.SetState(GeneralState.ACTIVE);   
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

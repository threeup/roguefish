using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Actor : Entity
{
    public int actorUID = -1;
    public string actorName;
    public AIAgent ai = null;
    public int HP = 1;
    public int AP = 1;
    public int RP = 1;
    public Item CurrentItem = null;
    public ActorInventory inventory = null;
    public ThreatList threatList = null;

    protected override void Initialize()
    {
        actorUID = Boss.GetActorUID();
        base.Initialize();
    }

    public override void Reset()
    {
        base.Reset();
    }

    public override void DestroySelf()
    {
        base.DestroySelf();
    }

    public override void UpdateEntity(float deltaTime)
    {
        if (ai != null)
        {
            ai.UpdateAgent(deltaTime);
        }
        base.UpdateEntity(deltaTime);
    }


    public void SetupActor(EntityProperties prop)
    {
        HP = prop.HP;        
    }



    public bool IsAlive()
    {
        return HP > 0;
    }

    public bool CanExecuteTask()
    {
        return IsAlive();
    }

    public virtual void ProcessInput(Vector2? click)
    {

    }

}

public class ActorInventory : MonoBehaviour
{
    

}
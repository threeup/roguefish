using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Actor : Entity
{
    public Vitals vitals;

    public int actorUID = -1;
    public string actorName;
    public AIAgent ai = null;
    public int HP = 1;
    public int AP = 1;
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
        if (vitals != null)
        {
            vitals.UpdateVitals();
        }
        if (ai != null)
        {
            ai.UpdateAgent(deltaTime);
        }
        base.UpdateEntity(deltaTime);
    }


    public void SetupProp(EntityProperties prop)
    {
        HP = prop.hp;    
        AP = prop.ap;    
        base.SetupProp(prop);
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

    void OnCollisionStay(Collision collisionInfo) {
        foreach (ContactPoint contact in collisionInfo.contacts) {
            Debug.DrawRay(contact.point, contact.normal*10f, Color.white);
        }
        if (collider.enabled && collisionInfo.collider.enabled)
        {
            CollideWith(collisionInfo.gameObject);
        }
    }

    protected virtual void CollideWith(GameObject other)
    {
        Actor otherActor = other.GetComponent<Actor>();
        if (otherActor != null)
        {
            //Debug.Log(this.name+" bump "+otherActor.name);
        }
        else
        {
            //Debug.Log(this.name+" hit by "+other.name);   
        }
    }

}

public class ActorInventory : MonoBehaviour
{
    

}
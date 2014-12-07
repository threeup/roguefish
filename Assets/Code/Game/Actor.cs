using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ActorAI
{
    public bool isNPC = false;
}

public class Actor : Entity
{
    public int actorUID = -1;
    public string actorName;
    public ActorAI ai = null;
    public int HP = 1;
    public int AP = 1;
    public int RP = 1;
    public Item CurrentItem = null;
    public ActorInventory inventory = null;
    public ThreatList threatList = null;
    public Weapon hook;
    private bool buttonDown;

    protected override void Initialize()
    {
        actorUID = Boss.GetActorUID();
        base.Initialize();
    }

    public override void Reset()
    {
        base.Reset();
    }

    public override void UpdateEntity(float deltaTime)
    {
        base.UpdateEntity(deltaTime);
        if (hook != null)
        {
            hook.desiredPos.x = this.currentPos.x;
            if (buttonDown)
            {
                hook.desiredPos.y = -1000f;
            }
            else
            {
                hook.desiredPos.y = this.currentPos.y;
            }
        }
    }


    public void SetupActor(EntityProperties prop)
    {
        HP = prop.HP;        
    }

    public override void DestroySelf()
    {
        Reset();
        FactoryEntity.Instance.PoolActor(this);
    }

    public bool IsAlive()
    {
        return HP > 0;
    }

    public bool CanExecuteTask()
    {
        return IsAlive();
    }

    public void ProcessInput(Vector2? click)
    {
        buttonDown = click.HasValue;
        if (buttonDown)
        {
            this.desiredPos.x = click.Value.x;
            if (hook == null)
            {
                CreateWeapon();
            }
        }
    }

    public bool CreateWeapon()
    {
        EntityProperties hookProp = new EntityProperties();
        hookProp.HP = 10;
        hookProp.imgProp = new ImageProperties(Constants.HookRect);
        hook = FactoryEntity.Instance.GetWeapon(hookProp);
        hook.name = "Hook";
        hook.weaponName = "Hook";
        hook.transform.position = Vector3.up*192f;
        hook.transform.localScale = Vector3.one*1f;
        hook.transform.SetParent(World.Instance.PlayField.transform, false);
        hook.owner = this;
        return true;
    }

}

public class ActorInventory : MonoBehaviour
{
    

}
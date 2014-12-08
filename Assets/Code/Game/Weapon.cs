using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Weapon : Entity
{
    private List<Entity> attackList;
    public int weaponUID = -1;
    public string weaponName;
    public Actor owner;
    public int HP = 1;
    public int AP = 1;
    public int HPDMG = 1;
    public int APDMG = 1;

    private float attackTimer = 0f;
    public float attackTimerMax = 5f;

    protected override void Initialize()
    {
        attackList = new List<Entity>();
        weaponUID = Boss.GetActorUID();//change later
        base.Initialize();
    }

    public override void UpdateEntity(float deltaTime)
    {
        base.UpdateEntity(deltaTime);
        attackTimer -= deltaTime;
        if (attackTimer < 0f)
        {
            Attack();
            attackTimer = attackTimerMax;
        }
    }

    public override void DestroySelf()
    {
        foreach(Actor actor in attackList)
        {
            actor.hasTimeToLive = true;
            actor.timeToLive = 0f;
        }
        Debug.Log("weapon destroyed");
        TurnOff();
        FactoryEntity.Instance.PoolWeapon(this);
    }

    public bool IsAlive()
    {
        return true;
    }

    void Attack()
    {
        foreach(Actor actor in attackList)
        {
            actor.HP -= HPDMG;
        }
    }


    public void AddAttack(Actor actor)
    {
        if (actor.propType == PropType.BOAT)
        {
            actor.TakeDamage(1);
            hasTimeToLive = true;
            timeToLive = 0f;
            return;
        }
        if (this.propType == PropType.HOOK)
        {
            if (attackList.Contains(actor))
            {
                return;
            }
            attackList.Add(actor);
            actor.AttachTo(this);
        }

    }

    public void RemoveAttack(Actor actor)
    {
        if (!attackList.Contains(actor))
        {
            return;
        }
        attackList.Remove(actor);
        actor.AttachTo(null);
    }

    public void Purge()
    {
        foreach(Actor actor in attackList)
        {
            actor.ImmuneTimer = 0.75f;
            actor.AttachTo(null);   
        }
        attackList.Clear();
    }

    public Actor Pop()
    {
        if (attackList.Count > 0)
        {
            Actor actor = attackList[0] as Actor; 
            attackList.RemoveAt(0);
            if (actor != null)
            {
                if (actor.propType == PropType.FISH || actor.propType == PropType.TURT)
                {
                    actor.BecomeDead();
                }
                if (actor.propType == PropType.BOAT)
                {
                    actor.TakeDamage(1);
                }
                return actor;
            }
        }
        return null;
    }
}
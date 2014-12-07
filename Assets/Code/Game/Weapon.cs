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
    public int RP = 1;

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

    public override void Reset()
    {
        base.Reset();
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
        Reset();
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


    public void AddAttack(FishActor actor)
    {
        if (attackList.Contains(actor))
        {
            return;
        }
        attackList.Add(actor);
        actor.AttachTo(this);
    }

    public void Purge()
    {
        foreach(Actor actor in attackList)
        {
            actor.AttachTo(null);   
        }
        attackList.Clear();
    }
}
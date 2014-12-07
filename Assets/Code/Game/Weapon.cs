using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Weapon : Entity
{
    public int weaponUID = -1;
    public string weaponName;
    public Actor owner;
    public int HP = 1;
    public int AP = 1;
    public int RP = 1;

    protected override void Initialize()
    {
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

}
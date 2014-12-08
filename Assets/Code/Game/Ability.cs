
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Ability
{
    public int ammo = 0;
    public int maxammo = 1;

    public EntityProperties eprop;
    public string ename;

    public Ability(EntityProperties eprop, string ename)
    {
        this.eprop = eprop;
        this.ename = ename;
    }

    public Weapon Execute(Actor actor)
    {
        Weapon weap = FactoryEntity.Instance.GetWeapon(eprop);
        weap.name = ename;
        weap.weaponName = ename;
        Vector2 startPos = actor.currentPos;
        startPos.x += UnityEngine.Random.Range(-1f,1f)*actor.transform.localScale.x*32f;
        weap.WarpTo(startPos);
        weap.transform.localScale = Vector3.one*1f;
        World.Instance.ParentToField(weap.transform);
        weap.owner = actor;
        weap.TurnOn();
        return weap;
    }

    public bool CanUseAbility(Actor actor)
    {
        return ammo > 0;
    }

    public void ReloadAmmo()
    {
        ammo = maxammo;
    }
}
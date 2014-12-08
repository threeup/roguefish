
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
        weap.WarpTo(actor.currentPos);
        weap.transform.localScale = Vector3.one*1f;
        World.Instance.ParentToField(weap.transform);
        weap.owner = actor;
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
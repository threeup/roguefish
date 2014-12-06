
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Item : MonoBehaviour
{
    public int range = 0;
    public int ammo = 0;
    public int maxammo = 0;
    public List<Ability> abilityList;
    public Ability activeAbility;

    public bool CanUseItem(Actor actor)
    {
        return true;
    }

    public void ReloadAmmo()
    {

    }

    public bool CanTargetTile()
    {
        return true;
    }
    public bool CanTargetSelf()
    {
        return true;
    }
    public bool CanTargetActor()
    {
        return true;
    }
    public bool AffectsFriendly()
    {
        return true;
    }
    public bool AffectsEnemy()
    {
        return true;
    }
}

public class Ability : MonoBehaviour
{
    public Item item;
}
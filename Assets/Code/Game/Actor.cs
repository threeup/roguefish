using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public struct ActorProperties
{
    public Material mat;
}

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
    public Tile CurrentTile = null;

    public override void Reset()
    {
        base.Reset();
    }

    public void UpdateActor(float deltaTime)
    {
        UpdateEntity(deltaTime);
    }

    public void SetPhysics(ActorProperties prop)
    {
        
    }

    public override void DestroySelf()
    {
        Reset();
        FactoryEntity.Instance.PoolActor(this);
    }

    public bool IsAlive()
    {
        return true;
    }

    public bool IsWaitingTask()
    {
        return false;
    }

    public bool IsWaitingMotion()
    {
        return false;
    }

    public bool CanExecuteTask()
    {
        return IsAlive();
    }

    public void AdvanceActorTurn()
    {

    }

    public bool IsAffiliatedWith(Actor other)
    {
        return false;
    }
    public bool IsTeamWith(Actor other)
    {
        return false;
    }

    public bool LookAt(Vector3 pos)
    {
        return true;
    }

    public void BuildPathToTile(Vector3 pos)
    {

    }

    public bool WalkPath()
    {
        return true;
    }

    public bool UseItem(Tile tile, Item item, Ability ability)
    {
        return false;
    }
    public bool UseItem(Actor other, Item item, Ability ability)
    {
        return false;   
    }
}

public class ActorInventory : MonoBehaviour
{
    public List<Item> weaponList;
    public Item selectedWeapon;

    public void SelectWeapon(Item weapon)
    {
        selectedWeapon = weapon;
    }

}
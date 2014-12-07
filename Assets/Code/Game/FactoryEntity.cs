
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FactoryEntity : MonoBehaviour {

    public static FactoryEntity Instance;

    public Actor actorPrefab;
    public Queue<Actor> actorPool;
    public Weapon weaponPrefab;
    public Queue<Weapon> weaponPool;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("FactoryEntity Already exists");
        }
        Instance = this;
        actorPool = new Queue<Actor>();
        weaponPool = new Queue<Weapon>();
    }


    public Actor GetActor(EntityProperties prop)
    {
        Actor result = GetActor();
        result.SetImage(prop.imgProp);
        result.SetPhysics(prop.projProp);
        return result;
    }

    public Actor GetActor()
    {
        if (actorPool.Count > 0) {
            return actorPool.Dequeue();
        } else {
            return Instantiate(actorPrefab) as Actor;
        }
    }

    public void PoolActor(Actor inp)
    {
        actorPool.Enqueue(inp);
    }



    public Weapon GetWeapon(EntityProperties prop)
    {
        Weapon result = GetWeapon();
        result.SetImage(prop.imgProp);
        result.SetPhysics(prop.projProp);
        return result;
    }

    public Weapon GetWeapon()
    {
        if (weaponPool.Count > 0) {
            return weaponPool.Dequeue();
        } else {
            return Instantiate(weaponPrefab) as Weapon;
        }
    }

    public void PoolWeapon(Weapon inp)
    {
        weaponPool.Enqueue(inp);
    }
}

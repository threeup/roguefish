
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FactoryEntity : MonoBehaviour {

    public static FactoryEntity Instance;

    public Actor genericPrefab;
    public Actor boatPrefab;
    public Queue<Actor> genericPool;
    public Queue<Actor> boatPool;
    public Weapon weaponPrefab;
    public Queue<Weapon> weaponPool;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("FactoryEntity Already exists");
        }
        Instance = this;
        genericPool = new Queue<Actor>();
        boatPool = new Queue<Actor>();
        weaponPool = new Queue<Weapon>();
    }


    public Actor GetGenericActor(EntityProperties prop)
    {
        Actor result = GetGenericActor();
        result.SetImage(prop.imgProp);
        result.SetPhysics(prop.projProp);
        return result;
    }

    public Actor GetGenericActor()
    {
        if (genericPool.Count > 0) {
            return genericPool.Dequeue();
        } else {
            return Instantiate(genericPrefab) as Actor;
        }
    }

    public void PoolActor(FishActor inp)
    {
        genericPool.Enqueue(inp);
    }


    public Actor GetBoatActor(EntityProperties prop)
    {
        Actor result = GetBoatActor();
        result.SetImage(prop.imgProp);
        result.SetPhysics(prop.projProp);
        return result;
    }

    public Actor GetBoatActor()
    {
        if (boatPool.Count > 0) {
            return boatPool.Dequeue();
        } else {
            return Instantiate(boatPrefab) as Actor;
        }
    }

    public void PoolActor(BoatActor inp)
    {
        boatPool.Enqueue(inp);
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

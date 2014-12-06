
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FactoryEntity : MonoBehaviour {

    public static FactoryEntity Instance;
    public Entity floorPrefab;
    public Queue<Entity> floorPool;


    public Entity entityPrefab;
    public Queue<Entity> entityPool;
    public Actor actorPrefab;
    public Queue<Actor> actorPool;

    public Shader entityShaderOpaque;
    public Shader entityShaderTrnsp;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("FactoryEntity Already exists");
        }
        Instance = this;
        floorPool = new Queue<Entity>();
        entityPool = new Queue<Entity>();
        actorPool = new Queue<Actor>();
    }

    public Entity GetFloor()
    {
        if (floorPool.Count > 0) {
            return floorPool.Dequeue();
        } else {
            return Instantiate(floorPrefab) as Entity;
        }
    }

    public Actor GetActor(ActorProperties prop)
    {
        Actor result = GetActor();
        result.SetMaterial(prop.mat);
        result.SetPhysics(prop);
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

    public void PoolActor(Actor act)
    {
        actorPool.Enqueue(act);
    }

    public Entity GetProjectile(ProjectileProperties prop)
    {
        Entity result = GetEntity();
        result.SetMaterial(prop.mat);
        result.SetPhysics(prop);
        return result;
    }

    public Entity GetEntity()
    {
        if (entityPool.Count > 0) {
            return entityPool.Dequeue();
        } else {
            return Instantiate(entityPrefab) as Entity;
        }
    }

    public void PoolEntity(Entity ent)
    {
        entityPool.Enqueue(ent);
    }

    
}

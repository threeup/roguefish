
using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public struct ImageProperties
{
    public Rect rect;

    public ImageProperties(Rect rect)
    {
        this.rect = rect;
    }
}


public enum CommandType
{
    HOOKDOWN,
    HOOKUP,
    STRAFELEFT,
    STRAFERIGHT,
}

[System.Serializable]
public struct CommandProperties
{
    public string name;
    public CommandType type;
    public Material mat;

}

[System.Serializable]
public enum PropType
{
    BOAT,
    FISH,
    TURT,
    BOOT,
    WHAL,
    HOOK,
    FREE,
}


[System.Serializable]
public struct EntityProperties
{
    public ImageProperties imgProp; 
    public PropType propType;
    public int hp;
    public int ap;
    public int rp;
    public float rpregen;
    public float mass;
    public float scale;
    public Vector2 highVel;
    public Vector2 lowVel;
    public float angularSpeed;
    public float ttl;

    public EntityProperties(ImageProperties imgProp, PropType propType, int hp, int ap, int rp, float rpregen, float mass, float scale, Vector2 highVel, Vector2 lowVel, float angularSpeed, float ttl)
    {
        this.imgProp = imgProp;        
        this.propType = propType;        
        this.hp = hp;
        this.ap = ap;
        this.rp = rp;
        this.mass = mass;        
        this.scale = scale;        
        this.highVel = highVel;        
        this.lowVel = lowVel;        
        this.angularSpeed = angularSpeed;        
        this.rpregen = rpregen;        
        this.ttl = ttl;
    }
}

/*
public class Vars : MonoBehaviour {

    public static Vars Instance;
    
    public CommandProperties[] commandProperties;
    public Dictionary<CommandType, CommandProperties> commandDict;

    public ProjectileProperties[] projectileProperties;
    public Dictionary<ProjectileType, ProjectileProperties> projectileDict;
    
    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("FactoryEntity Already exists");
        }
        Instance = this;
        commandDict = new Dictionary<CommandType, CommandProperties>();
        for(int i=0; i<commandProperties.Length; ++i)
        {
            commandDict.Add((CommandType)i, commandProperties[i]);
        }

        projectileDict = new Dictionary<ProjectileType, ProjectileProperties>();
        for(int i=0; i<projectileProperties.Length; ++i)
        {
            projectileDict.Add((ProjectileType)i, projectileProperties[i]);
        }
    }

    
}
*/
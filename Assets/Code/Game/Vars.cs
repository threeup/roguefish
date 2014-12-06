
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CommandType
{
    SHOOT,
    WAIT,
    FORWARD,
    ROTATELEFT,
    ROTATERIGHT,
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

public enum ProjectileType
{
    HOOK,
    RAIN,
    LIGHTNING,
}


[System.Serializable]
public struct ProjectileProperties
{
    public string name;
    public ProjectileType type;
    public Material mat;
    public float mass;
    public float radius;
    public Vector2 vel;
}


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

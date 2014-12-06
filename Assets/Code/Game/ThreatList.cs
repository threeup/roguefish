//#define VERBOSE
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum PerceptionLevel
{
    INVALID,
    FRIENDLY,
    NEUTRAL,
    ENEMY,
    SELF,
}

[System.Serializable]
public class ThreatRecord
{
    
}
[System.Serializable]
public class ThreatList
{       
    public Actor concernedActor;
    public List<ThreatRecord> enemies = new List<ThreatRecord>();
       

}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;



public class Boss : MonoBehaviour
{

    private static int nextActorUID = 0;
    public static int GetActorUID()
    {
        nextActorUID++;
        return nextActorUID;
    }


    public static Boss Instance;

    public void Awake()
    {
        Instance = this;
    }

}

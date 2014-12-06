
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Boss : MonoBehaviour
{
    public static Boss Instance;

    public void Awake()
    {
        Instance = this;
    }

}

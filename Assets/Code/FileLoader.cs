
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class FileLoader : MonoBehaviour
{
    public static FileLoader Instance;

    public void Awake()
    {
        Instance = this;
    }

    public bool BytesExists(string path)
    {
        return false;
    }

    public byte[] LoadBytes(string path)
    {
        return null;
    }
}

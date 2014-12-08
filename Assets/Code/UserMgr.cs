
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class UserMgr : MonoBehaviour {

    public static UserMgr Instance;
    public List<User> users;
    private User localUser;
    private User worldUser;
    private User aiUser;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("UserMgr Already exists");
        }
        Instance = this;
    }

    public void Initialize()
    {
        worldUser = AddUser(0);
        localUser = AddUser(1);
        aiUser = AddUser(2);


    }

    public User AddUser(int idx)
    {
        GameObject go = new GameObject();
        go.name = "User"+idx;
        User user = go.AddComponent<User>();
        user.Initialize();
        users.Add(user);
        return user;
    }

    public void AssignActor(int idx, Actor actor)
    {
        if (idx < users.Count)
        {
            users[idx].boat = actor as BoatActor;
            NormalActor norm = actor as NormalActor;
            if (norm != null)
            {
                users[idx].actorList.Add(norm);
            }
        }
    }

    public void Purge(int idx)
    {
        if (idx < users.Count)
        {
            foreach(NormalActor norm in users[idx].actorList)
            {
                norm.BecomeDead();
            }
        }
    }

    public void Input(int count, Vector2[] current, Vector2[] source)
    {
        localUser.ProcessInput(count, current, source);
    } 

    public BoatActor GetBoat(int idx)
    {
        if (idx < users.Count)
        {
            return users[idx].boat;
        }
        return null;
    }
}

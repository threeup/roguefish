
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SocialPlatforms;

public class UserMgr : MonoBehaviour {

    public static UserMgr Instance;
    public List<User> users;
    private User localUser;
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
        localUser = AddUser(0);
        aiUser = AddUser(1);


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
            users[idx].fish = actor as FishActor;
        }
    }

    public void Input(int count, Vector2[] current, Vector2[] source)
    {
        localUser.ProcessInput(count, current, source);
    } 
}

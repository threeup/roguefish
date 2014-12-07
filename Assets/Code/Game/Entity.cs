
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class Entity : MonoBehaviour
{
    public Vector2 currentPos;
    public Vector2 desiredPos;
    public RectTransform thisTransform;
    public GameObject go;
    public bool hasTimeToLive;
    public float timeToLive;
    public Image img = null;

    public float currentSpeedX = 1f;
    public float currentSpeedY = 1f;
    public float maxSpeedX = 1f;
    public float maxSpeedY = 1f;
    protected Vector2 velocity = Vector2.zero;

    void Awake()
    {
        go = this.gameObject;
        thisTransform = go.transform as RectTransform;
        Initialize();
    }

    protected virtual void Initialize()
    {
        currentSpeedX = maxSpeedX;
        currentSpeedY = maxSpeedY;
        World.Instance.Register(this);
    }

    public virtual void Reset()
    {
        hasTimeToLive = false;
        World.Instance.Deregister(this);
    }

    public virtual void UpdateEntity(float deltaTime)
    {
        if (hasTimeToLive)
        {
            timeToLive -= deltaTime;
            if (timeToLive <= 0f)
            {
                DestroySelf();
            }
        }
        Vector2 diff = (desiredPos-currentPos);
        float sqrDiff = diff.sqrMagnitude;
        if (sqrDiff > 0.01f)
        {
            velocity = diff;
            velocity.x = Mathf.Clamp(velocity.x, -maxSpeedX, maxSpeedX) * deltaTime*5;
            velocity.y = Mathf.Clamp(velocity.y, -maxSpeedY, maxSpeedY) * deltaTime*5;
            SetPos(currentPos+velocity);
        }
    }

    public void SetPhysics(ProjectileProperties prop)
    {

    }

    private void SetPos(Vector2 pos)
    {
        thisTransform.anchoredPosition = pos;
        this.currentPos = pos;
    }

    public void WarpTo(Vector2 pos)
    {
        SetPos(pos);
        GoalTo(pos);
    }

    public void GoalTo(Vector2 pos)
    {
        this.desiredPos = pos;   
    }

    public void SetImage(ImageProperties imgProp)
    {
        img = FactoryEmoji.Instance.GetEmoji(imgProp);
        img.transform.SetParent(this.transform, true);
    }

    public virtual void DestroySelf()
    {
        Reset();
        Debug.LogWarning("Cant destroy generic");
    }
}


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public delegate void EntityEvent();

public class Entity : MonoBehaviour
{
    public Vector2 currentPos;
    public Vector2 desiredPos;
    public RectTransform thisTransform;
    public GameObject go;
    public bool hasTimeToLive;
    public float timeToLive = 0f;
    public Image img = null;

    public float currentSpeedX = 1f;
    public float currentSpeedY = 1f;
    public float maxSpeedX = 1f;
    public float maxSpeedY = 1f;
    public Vector2 velocity = Vector2.zero;


    public bool isValid = false;
    public EntityEvent OnAttach;

    public Entity attachedParent = null;

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
        isValid = true;
    }

    public virtual void Reset()
    {
        if (attachedParent != null)
        {
            Weapon weapon = attachedParent as Weapon;
            if (weapon != null)
            {
                weapon.RemoveAttack(this as Actor);
            }
        }
        collider.enabled = false;
        img.enabled = false;
        img.transform.SetParent(null, true);
        FactoryEmoji.Instance.PoolImage(img);

        hasTimeToLive = false;
        this.name = this.name+"X";
        this.enabled = false;
        World.Instance.Deregister(this);
        isValid = false;
    }

    void Update()
    {
        UpdateEntity(Time.deltaTime);
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
        if (attachedParent != null)
        {
            SetPos(attachedParent.currentPos);
            velocity = diff;
        }
        else
        {
            
            float sqrDiff = diff.sqrMagnitude;
            if (sqrDiff > 0.01f)
            {
                velocity = diff;
                velocity.x = Mathf.Clamp(velocity.x, -maxSpeedX, maxSpeedX) * deltaTime;
                velocity.y = Mathf.Clamp(velocity.y, -maxSpeedY, maxSpeedY) * deltaTime;
                SetPos(currentPos+velocity);
            }
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

    public void AttachTo(Entity other)
    {
        attachedParent = other;
        if (OnAttach != null)
        {
            OnAttach();
        }
    }

    public void BecomeDead()
    {
        hasTimeToLive = true;
        timeToLive = 0f;
        collider.enabled = false;
    }
}

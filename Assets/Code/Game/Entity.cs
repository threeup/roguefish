﻿
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public delegate void EntityEvent(Entity parent);

public class Entity : MonoBehaviour
{
    public PropType propType;

    public Vector2 currentPos;
    public Vector2 desiredPos;
    public RectTransform thisTransform;
    public GameObject go;
    public bool hasTimeToLive;
    public float timeToLive = 0f;
    public Image img = null;

    
    public Vector2 lowVel = Vector2.zero;
    public Vector2 highVel = Vector2.zero;
    private Vector2 velocity = Vector2.zero;
    public Vector2 Velocity { get { return velocity; } }
    public float angularSpeed = 1f;

    public int RP = 1;
    public float regenRate = 1f;
    private float regenTimer = 0f;
    public int lowThresh = 2;
    public int highThresh = 8;
    private bool lowMode = false;

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
        TurnOn();
    }

    public virtual void TurnOn()
    {
        collider.enabled = true;
        if (img != null)
        {
            img.enabled = true;
        }
        this.enabled = true;
        World.Instance.Register(this);
        isValid = true;
    }

    public virtual void TurnOff()
    {
        if (attachedParent != null)
        {
            Weapon weapon = attachedParent as Weapon;
            if (weapon != null && weapon.propType == PropType.HOOK)
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
        float deltaTime = Time.deltaTime;
        regenTimer -= deltaTime;
        if (regenTimer < 0f)
        {
            if (lowMode)
            {
                this.RP = Mathf.Min(RP+2, 10);
            }
            else
            {
                this.RP = Mathf.Max(RP-1, 0);   
            }
            regenTimer = regenRate;
        }
        UpdateEntity(deltaTime);
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
            Vector2 attachDiff = (attachedParent.currentPos-currentPos);
            velocity = attachDiff;
            velocity.x = Mathf.Clamp(velocity.x, -5000, 5000) * deltaTime;
            velocity.y = Mathf.Clamp(velocity.y, -5000, 5000) * deltaTime;
            Vector2 nextPos = currentPos+velocity;
            Vector2 average = nextPos*0.9f+attachedParent.currentPos*0.1f;
            SetPos(average);
            velocity = diff;
        }
        else
        {
            Vector2 currentVel = Vector2.zero;
            if (lowMode && RP >= highThresh)
            {
                lowMode = false;
            }
            if (!lowMode && RP <= lowThresh)
            {
                lowMode = true;
            }
            currentVel = lowMode ? lowVel : highVel;
            float sqrDiff = diff.sqrMagnitude;
            if (sqrDiff > 0.01f)
            {
                velocity = diff;
                velocity.x = Mathf.Clamp(velocity.x, -currentVel.x, currentVel.x) * deltaTime;
                velocity.y = Mathf.Clamp(velocity.y, -currentVel.y, currentVel.y) * deltaTime;
                SetPos(currentPos+velocity);
            }
        }
    }

    public virtual void SetupProp(EntityProperties prop)
    {
        transform.localScale = Vector3.one*prop.scale;
        propType = prop.propType;
        RP = prop.rp;    
        regenRate = prop.rpregen;
        lowVel = prop.lowVel;
        highVel = prop.highVel;
        angularSpeed = prop.angularSpeed; 
        if (prop.ttl > 0.01f)
        {
            hasTimeToLive = true;
            timeToLive = prop.ttl;
        }
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
        img.transform.localPosition = Vector3.zero;
        img.transform.localScale = Vector3.one;
        img.transform.rotation = Quaternion.identity;
        img.transform.SetParent(this.transform, false);
    }

    public virtual void DestroySelf()
    {
        TurnOff();
        Debug.LogWarning("Cant destroy generic");
    }

    public void AttachTo(Entity other)
    {
        attachedParent = other;
        if (OnAttach != null)
        {
            OnAttach(other);
        }
    }

    public void BecomeDead()
    {
        hasTimeToLive = true;
        timeToLive = 0f;
        collider.enabled = false;
    }
}

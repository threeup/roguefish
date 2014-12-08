using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Actor : Entity
{
    public Vitals vitals;

    public int actorUID = -1;
    public string actorName;
    public AIAgent ai = null;
    public int HP = 1;
    public int AP = 1;
    public List<Ability> abilities;
    public List<Weapon> activeWeapons;
    public int maxWeapons = 1;
    public ThreatList threatList = null;
    public bool attackQueued = false;
    public float attackRate = 0.75f;
    public float attackTimer = 0f;

    public int progressRemaining = 0;

    private float immuneTimer = -1f;
    public float ImmuneTimer { 
        get { return immuneTimer; } 
        set { immuneTimer = value; } 
    }

    protected override void Initialize()
    {
        actorUID = Boss.GetActorUID();
        abilities = new List<Ability>();
        base.Initialize();
    }

    public override void TurnOff()
    {
        base.TurnOff();
    }

    public override void DestroySelf()
    {
        base.DestroySelf();
    }

    public override void UpdateEntity(float deltaTime)
    {
        if (immuneTimer > 0f)
        {
            immuneTimer -= deltaTime;
        }
        if (attackQueued)
        {
            attackTimer -= deltaTime;
            if (attackTimer < 0f)
            {
                ProcessAttack();
                attackTimer = attackRate;
            }
        }
        if (vitals != null)
        {
            vitals.UpdateVitals();
        }
        if (ai != null)
        {
            ai.UpdateAgent(deltaTime);
        }
        base.UpdateEntity(deltaTime);
    }


    public override void SetupProp(EntityProperties prop)
    {
        HP = prop.hp;    
        AP = prop.ap;    
        base.SetupProp(prop);
    }



    public bool IsAlive()
    {
        return HP > 0;
    }

    public bool CanExecuteTask()
    {
        return IsAlive();
    }

    public virtual void ProcessInput(Vector2? click)
    {

    }

    void OnCollisionStay(Collision collisionInfo) {
        foreach (ContactPoint contact in collisionInfo.contacts) {
            Debug.DrawRay(contact.point, contact.normal*10f, Color.white);
        }
        if (collider.enabled && collisionInfo.collider.enabled)
        {
            CollideWith(collisionInfo.gameObject);
        }
    }

    protected virtual void CollideWith(GameObject other)
    {
        Actor otherActor = other.GetComponent<Actor>();
        if (otherActor != null)
        {
            //Debug.Log(this.name+" bump "+otherActor.name);
        }
        else
        {
            //Debug.Log(this.name+" hit by "+other.name);   
        }
    }

    public void ProcessAttack()
    {
        if (abilities.Count > 0)
        {
            Ability ability = abilities[ UnityEngine.Random.Range(0, abilities.Count) ];
            if (!ability.CanUseAbility(this))
            {
                ability.ReloadAmmo();
            }
            if (ability.CanUseAbility(this))
            {
                Weapon activeWeapon = ability.Execute(this);
                activeWeapons.Add(activeWeapon);
                if (activeWeapons.Count >= maxWeapons)
                {
                    attackQueued = false;
                }
            }
        }        
    }

    public void RemoveWeapon(Weapon weapon)
    {
        if (activeWeapons.Contains(weapon))
        {
            activeWeapons.Remove(weapon);
        }
    }

    public void TakeDamage(int val)
    {
        this.HP -= val;
    }

}

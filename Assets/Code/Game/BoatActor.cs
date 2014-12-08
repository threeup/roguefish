using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoatActor : Actor
{
    private bool buttonDown;

    private float healthDecayTimer = 10f;
    public float healthDecayRate = 100f;


    public override void UpdateEntity(float deltaTime)
    {
        healthDecayTimer -= deltaTime;
        if (healthDecayTimer < 0f)
        {
            TakeDamage(1);
            healthDecayTimer = healthDecayRate;
        }
        base.UpdateEntity(deltaTime);
        foreach(Weapon activeWeapon in activeWeapons)
        {
            if (activeWeapon.propType == PropType.HOOK)
            {
                activeWeapon.desiredPos.x = this.currentPos.x;
                if (buttonDown)
                {
                    activeWeapon.desiredPos.y = -1000f;
                }
                else
                {
                    activeWeapon.desiredPos.y = this.currentPos.y;
                }
            }
            else
            {
                activeWeapon.desiredPos.x = this.currentPos.x;
                activeWeapon.desiredPos.y = -1000f;
            }
        }

        if (currentPos.x > 430)
        {
            CheckAdvance();
        }
    }

    public override void DestroySelf()
    {
        Debug.Log("Destroy Boat");
        TurnOff();
        FactoryEntity.Instance.PoolActor(this);
    }

    public override void ProcessInput(Vector2? click)
    {
        buttonDown = click.HasValue;
        if (buttonDown)
        {
            this.desiredPos.x = click.Value.x;
            if (activeWeapons.Count == 0)
            {
                CreateWeapon();
            }
        }
    }

    public bool CreateWeapon()
    {
        attackQueued = true;
        return true;
    }


    protected override void CollideWith(GameObject other)
    {
        if (!isValid)
        {
            return;
        }
        Actor otherActor = other.GetComponent<Actor>();
        if (otherActor != null)
        {
            //Debug.Log(this.name+" bump "+otherActor.name);
            return;
        }
        Weapon otherWeapon = other.GetComponent<Weapon>();
        if (otherWeapon)
        {
            if (otherWeapon.owner == this && activeWeapons.Count > 0)
            {
                Weapon activeWeapon = activeWeapons[0];
                Actor hookedActor = activeWeapon.Pop();
                if (hookedActor != null)
                {
                    if (hookedActor.propType == PropType.FISH)
                    {
                        this.HP = Mathf.Min(this.HP+1,10);
                        this.progressRemaining -= 1;
                    }
                    Debug.Log(this+"Caught"+hookedActor);
                }
                else
                {
                    //destroy activeWeapon?
                }
            }
            else
            {
                this.AP -= otherWeapon.APDMG;
                if (this.AP < 0)
                {
                    otherWeapon.AddAttack(this);
                }
            }
        }
    }

    public void CheckAdvance()
    {
        if (progressRemaining <= 0)
        {
            World.Instance.AdvanceLevel();
        }
    }



}
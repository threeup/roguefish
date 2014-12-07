using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BoatActor : Actor
{
    public Weapon hook;
    private bool buttonDown;

    public override void UpdateEntity(float deltaTime)
    {
        base.UpdateEntity(deltaTime);
        if (hook != null)
        {
            hook.desiredPos.x = this.currentPos.x;
            if (buttonDown)
            {
                hook.desiredPos.y = -1000f;
            }
            else
            {
                hook.desiredPos.y = this.currentPos.y;
            }
        }
    }

    public override void DestroySelf()
    {
        Reset();
        FactoryEntity.Instance.PoolActor(this);
    }

    public override void ProcessInput(Vector2? click)
    {
        buttonDown = click.HasValue;
        if (buttonDown)
        {
            this.desiredPos.x = click.Value.x;
            if (hook == null)
            {
                CreateWeapon();
            }
        }
    }

    public bool CreateWeapon()
    {
        EntityProperties hookProp = new EntityProperties();
        hookProp.HP = 10;
        hookProp.imgProp = new ImageProperties(Constants.HookRect);
        hook = FactoryEntity.Instance.GetWeapon(hookProp);
        hook.name = "Hook";
        hook.weaponName = "Hook";
        hook.WarpTo(this.currentPos);
        hook.transform.localScale = Vector3.one*1f;
        World.Instance.ParentToField(hook.transform);
        hook.owner = this;
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
            if (otherWeapon == hook)
            {
                Actor hookedActor = hook.Pop();
                if (hookedActor != null)
                {
                    Debug.Log("Caught"+hookedActor);
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

}
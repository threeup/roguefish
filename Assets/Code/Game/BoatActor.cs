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
        hook.transform.SetParent(World.Instance.PlayField.transform, false);
        hook.owner = this;
        return true;
    }

}
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NormalActor : Actor
{
    private bool buttonDown;


    public override void UpdateEntity(float deltaTime)
    {

        base.UpdateEntity(deltaTime);
        UpdateOrientation();
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
            this.desiredPos.y = click.Value.y;
        }
    }

    void UpdateOrientation()
    {
        Vector2 dir = Velocity.normalized;
        Vector3 eulers = Vector3.zero;
        if (Mathf.Abs(Velocity.x) > 0.1f)
        {
            if (dir.x > 0f)
            {
                eulers.y = 180f;
            }
            else
            {
                eulers.y = 0f;   
            }
            eulers.z = -dir.y*180f/Mathf.PI;
            (img.transform as RectTransform).eulerAngles = eulers;
        }        
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
        if (otherWeapon && this.ImmuneTimer < 0f)
        {
            this.AP -= otherWeapon.APDMG;
            if (this.AP < 0)
            {
                if (propType == PropType.BOOT)
                {
                    otherWeapon.Purge();
                }
                else
                {
                    otherWeapon.AddAttack(this);
                }
            }
        }
    }
}
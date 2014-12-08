using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class AIAgent : MonoBehaviour
{
    public delegate bool ActionUpdate(float deltaTime, ActionData adata, Actor actor);
    public enum ActionType
    {
        MOVE,
        WAIT,
    }
    public class ActionData
    {
        public ActionType atype;
        public Vector2 pos;
        public float val;
        public ActionUpdate Start;
        public ActionUpdate Tick;
        public ActionData(ActionType atype, Vector2 pos, float val, ActionUpdate start, ActionUpdate tick)
        {
            this.atype = atype;
            this.pos = pos;
            this.val = val;
            this.Start = start;
            this.Tick = tick;
        }

        public bool DoStart(Actor actor)
        {
            if (Start != null)
            {
                return Start(0f, this, actor);
            }
            return false;
        }

        public bool DoTick(float deltaTime, Actor actor)
        {
            if (Tick != null)
            {
                return Tick(deltaTime, this, actor);
            }
            return false;
        }
    }

    public bool isNPC = false;
    public Actor actor;

    private ActionData currentAction;

    private float evalTimer = 0f;
    public float evalTimerMax = 1f;

    private float fastTimer = 0f;
    public float fastTimerMax = 0.25f;

    public string debugString;

    public void Initialize(Actor actor)
    {
        this.actor = actor;
        actor.ai = this;
    }

    public void UpdateAgent(float deltaTime)
    {
        evalTimer -= deltaTime;
        if (evalTimer < 0f)
        {
            if (currentAction == null)
            {
                if (actor.IsAlive())
                {
                    int roll = UnityEngine.Random.Range(0,4);
                    switch(roll)
                    {
                        default:
                        case 0: currentAction = CreateRandomAcross(actor); break;
                        case 1: 
                        case 2: currentAction = CreateDrift(actor); break;
                        case 3: currentAction = CreateRandomWander(actor); break;
                    }
                }
                else
                {
                    currentAction = CreateDrift(actor);
                }
                debugString = actor.name+" "+currentAction.atype;
            }
            currentAction.DoStart(actor);
            evalTimer = evalTimerMax;
        }
        fastTimer -= deltaTime;
        if (fastTimer < 0f)
        {
            if (currentAction != null)
            {
                bool finished = currentAction.DoTick(fastTimerMax, actor);
                if (finished)
                {
                    currentAction = null;
                }
            }
            fastTimer = fastTimerMax;
        }
    }

    public static ActionData CreateDrift(Actor actor)
    {
        Vector2 next = actor.currentPos + actor.Velocity.normalized*20f;
        float time = UnityEngine.Random.Range(0.3f,1f);
        return new ActionData(ActionType.WAIT, next, time, SetupWander, DriftUpdate);
    }


    public static ActionData CreateRandomWander(Actor actor)
    {
        Vector2 next = Vector2.zero;
        next.x = Mathf.Clamp(actor.currentPos.x + UnityEngine.Random.Range(-20f,20f), -570f,570f);
        next.y = Mathf.Clamp(actor.currentPos.y + UnityEngine.Random.Range(-20f,20f), -370f,110f);

        return new ActionData(ActionType.MOVE, next, 0f, SetupWander, MovePositionUpdate);
    }

    public static ActionData CreateRandomAcross(Actor actor)
    {
        Vector2 next = Vector2.zero;
        float travelX = UnityEngine.Random.Range(400,1100f);
        travelX *= (actor.currentPos.x > 0 ? -1f : 1f);
        next.x = actor.currentPos.x + travelX;
        next.y = UnityEngine.Random.Range(-370f,110f);
        return new ActionData(ActionType.MOVE, next, 0f, SetupNormal, MovePositionUpdate);
    }   

    public static ActionData CreateRandomAnywhere(Actor actor)
    {
        Vector2 next = Vector2.zero;
        float min = 100f;
        do
        {
            next.x = UnityEngine.Random.Range(-570f,570f);
            next.y = UnityEngine.Random.Range(-370f,110f);
        } while ((actor.currentPos - next).sqrMagnitude < min*min);
        return new ActionData(ActionType.MOVE, next, 0f, SetupNormal, MovePositionUpdate);
    }

    public static bool SetupWander(float deltaTime, ActionData adata, Actor actor)
    {
        actor.lowThresh = 6;

        actor.GoalTo(adata.pos);
        return true;
    }

    public static bool SetupNormal(float deltaTime, ActionData adata, Actor actor)
    {
        actor.lowThresh = 0;

        actor.GoalTo(adata.pos);
        return true;
    }

    public static bool MovePositionUpdate(float deltaTime, ActionData adata, Actor actor)
    {
        float min = 1f;
        return (actor.currentPos - adata.pos).sqrMagnitude < min*min;
    }

    public static bool DriftUpdate(float deltaTime, ActionData adata, Actor actor)
    {
        adata.val -= deltaTime;
        return adata.val < 0f;
    }
}
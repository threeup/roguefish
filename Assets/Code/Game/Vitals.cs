using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Vitals : MonoBehaviour
{
    public Actor actor;

    private int lastHealthIndex = 0;
    public Image health;
    public Image house;


    public void Initialize(Actor actor)
    {
        this.actor = actor;
        actor.vitals = this;
        health = FactoryEmoji.Instance.GetEmoji(Constants.MoonImgData5);
        health.transform.SetParent(this.transform, false);
        health.transform.localScale = 1.4f*Vector3.one;
        health.transform.localPosition = new Vector3(-309f, 225f, 0f);
        health.name = "Health";
        house = FactoryEmoji.Instance.GetEmoji(Constants.HouseImgData);
        house.transform.SetParent(this.transform, false);
        house.transform.localPosition = new Vector3(459f, 205f, 0f);
        house.name = "House";
        house = FactoryEmoji.Instance.GetEmoji(Constants.HouseImgData);
        house.transform.SetParent(this.transform, false);
        house.transform.localPosition = new Vector3(459f, 155f, 0f);
        house.name = "Land";
    }

    public void UpdateVitals()
    {
        int healthIndex = (int)Mathf.Round(actor.HP*0.5f);
        if (healthIndex != lastHealthIndex)
        {
            switch(healthIndex)
            {
                case 5: health.sprite = FactoryEmoji.Instance.GetSprite(Constants.MoonImgData5); break;
                case 4: health.sprite = FactoryEmoji.Instance.GetSprite(Constants.MoonImgData4); break;
                case 3: health.sprite = FactoryEmoji.Instance.GetSprite(Constants.MoonImgData3); break;
                case 2: health.sprite = FactoryEmoji.Instance.GetSprite(Constants.MoonImgData2); break;
                default:
                case 1: health.sprite = FactoryEmoji.Instance.GetSprite(Constants.MoonImgData1); break;
            }
            lastHealthIndex = healthIndex;
        }
    }

}
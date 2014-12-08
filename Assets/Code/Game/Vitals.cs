using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Vitals : MonoBehaviour
{
    public Actor actor;

    private int lastHealthIndex = 0;
    private bool lastCanAdvance = false;
    public Image health;
    public Image house;
    public Image land;
    public Image wife;


    public void Initialize(Actor actor)
    {
        this.actor = actor;
        actor.vitals = this;
        health = FactoryEmoji.Instance.GetEmoji(Constants.MoonImgData5);
        health.transform.SetParent(this.transform, false);
        health.transform.localScale = 1.4f*Vector3.one;
        health.transform.localPosition = new Vector3(-309f, 240f, -9f);
        health.name = "Health";
        house = FactoryEmoji.Instance.GetEmoji(Constants.HouseImgData);
        house.transform.SetParent(this.transform, false);
        house.transform.localScale = 1.5f*Vector3.one;
        house.transform.localPosition = new Vector3(435f, 165f, -7f);
        house.name = "House";
        wife = FactoryEmoji.Instance.GetEmoji(Constants.WifeImgDataNo);
        wife.transform.SetParent(this.transform, false);
        wife.transform.localScale = 0.5f*Vector3.one;
        wife.transform.localPosition = new Vector3(435f, 165f, -6f);
        wife.name = "Wife";
        land = FactoryEmoji.Instance.GetEmoji(Constants.LandImgData);
        land.transform.SetParent(this.transform, false);
        land.transform.localScale = 5f*Vector3.one;
        land.transform.localPosition = new Vector3(535f, 225f, -8f);
        land.name = "Land";
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

        bool canAdvance = actor.progressRemaining <= 0;
        if (canAdvance != lastCanAdvance)
        {
            if (canAdvance)
            {
                wife.sprite = FactoryEmoji.Instance.GetSprite(Constants.WifeImgDataYes); 
            }
            else
            {
                wife.sprite = FactoryEmoji.Instance.GetSprite(Constants.WifeImgDataNo);    
            }
            lastCanAdvance = canAdvance;
        }
    }

    public void TurnOff()
    {
        health.enabled = false;
        wife.enabled = false;
        land.enabled = false;
        house.enabled = false;
        this.enabled = false;
        Destroy(this.gameObject);
    }

}
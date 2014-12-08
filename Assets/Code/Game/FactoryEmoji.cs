
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class FactoryEmoji : MonoBehaviour {

    public static FactoryEmoji Instance;
    public Texture2D masterImage;
    public Image imagePrefab;
    public Queue<Image> imagePool;

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogError("FactoryEntity Already exists");
        }
        Instance = this;
        imagePool = new Queue<Image>();
    }

    public Image GetEmoji(ImageProperties prop)
    {
        Image result = GetImage();
        result.sprite = GetSprite(prop);
        return result;
    }

    public Sprite GetSprite(ImageProperties prop)
    {
        Rect rect = new Rect(prop.rect.x, prop.rect.y, 64, 64);
        return Sprite.Create(masterImage,rect,Vector2.one,1f);
    }

    public Image GetImage()
    {
        if (imagePool.Count > 0) {
            return imagePool.Dequeue();
        } else {
            return Instantiate(imagePrefab) as Image;
        }
    }

    public void PoolImage(Image img)
    {
        imagePool.Enqueue(img);
    }

    
}

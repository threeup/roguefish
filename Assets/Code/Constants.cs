
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Constants
{
	public static ImageProperties BoatImgData 			= new ImageProperties(new Rect(1660,1274,64,64));
	public static ImageProperties HookImgData 			= new ImageProperties(new Rect(64,64,64,64));
	public static ImageProperties FishImgData 			= new ImageProperties(new Rect(768,1344,64,64));
	public static ImageProperties AngelFishImgData 		= new ImageProperties(new Rect(768,1280,64,64));
	public static ImageProperties BrownFishImgData 		= new ImageProperties(new Rect(768,1216,64,64));
	public static ImageProperties TurtleImgData 		= new ImageProperties(new Rect(768,1152,64,64));
	public static ImageProperties BootImgData 			= new ImageProperties(new Rect(896,1024,64,64));
	public static ImageProperties HappyWhaleImgData 	= new ImageProperties(new Rect(896,64,64,64));
	public static ImageProperties BigWhaleImgData 		= new ImageProperties(new Rect(896,768,64,64));

	public static ImageProperties StartImgData 			= new ImageProperties(new Rect(256,1216,64,64));
	public static ImageProperties QuitImgData 			= new ImageProperties(new Rect(192,896,64,64));


	public static EntityProperties BoatData = new EntityProperties(
		BoatImgData, 		PropType.BOAT, 10, 10, 10, 1f, 1f, 1f, new Vector2(100f,0f), new Vector2(100f,0f), 1f);
	public static EntityProperties HookData = new EntityProperties(
		HookImgData, 		PropType.HOOK, 10, 10, 10, 1f, 1f, 1f, new Vector2(999f,75f), new Vector2(999f,75f), 1f);
	public static EntityProperties FishData = new EntityProperties(
		FishImgData, 		PropType.FISH, 10, 10, 10, 1f, 1f, 1f, new Vector2(100f,65f),  new Vector2(60f,40f), 1f);
	public static EntityProperties AngelFishData = new EntityProperties(
		AngelFishImgData, 	PropType.FISH, 10, 10, 10, 1f, 1f, 1f, new Vector2(100f,65f),  new Vector2(60f,40f), 1f);
	public static EntityProperties BrownFishData = new EntityProperties(
		BrownFishImgData, 	PropType.FISH, 10, 10, 10, 1f, 1f, 1f, new Vector2(100f,65f),  new Vector2(60f,40f), 1f);
	public static EntityProperties TurtleData = new EntityProperties(
		TurtleImgData, 		PropType.TURT, 10, 10, 10, 1f, 1f, 1f, new Vector2(100f,65f),  new Vector2(60f,40f), 1f);
	public static EntityProperties BootData	= new EntityProperties(
		BootImgData, 		PropType.BOOT, 10, 10, 10, 1f, 1f, 1f, new Vector2(100f,65f),  new Vector2(60f,40f), 1f);
	public static EntityProperties HappyWhaleData = new EntityProperties(
		FishImgData, 		PropType.WHAL, 10, 10, 10, 1f, 1f, 1f, new Vector2(100f,65f),  new Vector2(60f,40f), 1f);
	public static EntityProperties BigWhaleData = new EntityProperties(
		FishImgData, 		PropType.WHAL, 10, 10, 10, 1f, 1f, 1f, new Vector2(100f,65f),  new Vector2(60f,40f), 1f);
}

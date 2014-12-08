
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


public class Constants
{
	//public static ImageProperties BoatImgData 			= new ImageProperties(new Rect(1660,1274,64,64));
	public static ImageProperties BoatImgData 			= new ImageProperties(new Rect(128,1088,64,64));
	public static ImageProperties HookImgData 			= new ImageProperties(new Rect(64,64,64,64));
	public static ImageProperties FishImgData 			= new ImageProperties(new Rect(768,1344,64,64));
	public static ImageProperties AngelFishImgData 		= new ImageProperties(new Rect(768,1280,64,64));
	public static ImageProperties BrownFishImgData 		= new ImageProperties(new Rect(768,1216,64,64));
	public static ImageProperties TurtleImgData 		= new ImageProperties(new Rect(768,1152,64,64));
	public static ImageProperties BootImgData 			= new ImageProperties(new Rect(896,1024,64,64));
	public static ImageProperties HappyWhaleImgData 	= new ImageProperties(new Rect(896,64,64,64));
	public static ImageProperties BigWhaleImgData 		= new ImageProperties(new Rect(896,768,64,64));
	public static ImageProperties CloudImgData 			= new ImageProperties(new Rect(64,1728,64,64));
	public static ImageProperties RainImgData 			= new ImageProperties(new Rect(1024,450,64,64));
	public static ImageProperties LightningImgData 		= new ImageProperties(new Rect(128,1856,64,64));

	public static ImageProperties StartImgData 			= new ImageProperties(new Rect(256,1216,64,64));
	public static ImageProperties QuitImgData 			= new ImageProperties(new Rect(192,896,64,64));
	public static ImageProperties HouseImgData 			= new ImageProperties(new Rect(640,510,64,64));
	public static ImageProperties LandImgData 			= new ImageProperties(new Rect(1415,1031,50,50));
	public static ImageProperties MoonImgData5			= new ImageProperties(new Rect(320,576,64,64));
	public static ImageProperties MoonImgData4			= new ImageProperties(new Rect(320,512,64,64));
	public static ImageProperties MoonImgData3			= new ImageProperties(new Rect(320,448,64,64));
	public static ImageProperties MoonImgData2			= new ImageProperties(new Rect(320,384,64,64));
	public static ImageProperties MoonImgData1			= new ImageProperties(new Rect(320,256,64,64));
	public static ImageProperties WifeImgDataNo			= new ImageProperties(new Rect(1540,384,64,64));
	public static ImageProperties WifeImgDataYes		= new ImageProperties(new Rect(1540,0,64,64));


	public static EntityProperties BoatData = new EntityProperties(
		BoatImgData, 		PropType.BOAT, 10, 10, 10, 1f, 1f, 1.5f, new Vector2(100f,0f), new Vector2(100f,0f), 1f, 0f);
	public static EntityProperties HookData = new EntityProperties(
		HookImgData, 		PropType.HOOK, 10, 10, 10, 1f, 1f, 1f, new Vector2(999f,75f), new Vector2(999f,75f), 1f, 0f);

	public static EntityProperties CloudData = new EntityProperties(
		CloudImgData, 		PropType.BOAT, 10, 10, 10, 1f, 1f, 3f, new Vector2(1000f,0f), new Vector2(1000f,0f), 1f, 0f);
	public static EntityProperties RainData = new EntityProperties(
		RainImgData, 		PropType.FREE, 10, 10, 10, 1f, 1f, 0.2f, new Vector2(10f,300f), new Vector2(10f,300f), 1f, 1f);
	public static EntityProperties LightningData = new EntityProperties(
		LightningImgData, 	PropType.FREE, 10, 10, 10, 1f, 1f, 0.4f, new Vector2(10f,500f), new Vector2(10f,500f), 1f, 1f);


	public static EntityProperties FishData = new EntityProperties(
		FishImgData, 		PropType.FISH, 10, 10, 10, 1f, 1f, 0.75f, new Vector2(100f,65f),  new Vector2(60f,40f), 1f, 0f);
	public static EntityProperties AngelFishData = new EntityProperties(
		AngelFishImgData, 	PropType.FISH, 10, 10, 10, 1f, 1f, 1f, new Vector2(60f,40f),  new Vector2(30f,20f), 1f, 0f);
	public static EntityProperties BrownFishData = new EntityProperties(
		BrownFishImgData, 	PropType.FISH, 10, 10, 10, 1f, 1f, 1f, new Vector2(130f,80f),  new Vector2(60f,40f), 1f, 0f);
	public static EntityProperties TurtleData = new EntityProperties(
		TurtleImgData, 		PropType.TURT, 10, 10, 10, 1f, 1f, 1.3f, new Vector2(50f,35f),  new Vector2(10f,3f), 1f, 0f);
	public static EntityProperties BootData	= new EntityProperties(
		BootImgData, 		PropType.BOOT, 10, 10, 10, 1f, 1f, 1f, new Vector2(30f,30f),  new Vector2(30f,30f), 1f, 0f);
	public static EntityProperties HappyWhaleData = new EntityProperties(
		FishImgData, 		PropType.WHAL, 10, 10, 10, 1f, 1f, 1f, new Vector2(100f,65f),  new Vector2(60f,40f), 1f, 0f);
	public static EntityProperties BigWhaleData = new EntityProperties(
		FishImgData, 		PropType.WHAL, 10, 10, 10, 1f, 1f, 1f, new Vector2(100f,65f),  new Vector2(60f,40f), 1f, 0f);
}

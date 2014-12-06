using UnityEngine;

[System.Serializable]
public class QuadTree
{
	public static QuadTree Root;
	public static int depth = 4;

	public int identifier;

	public int radius;
	public Vector2 center;
	public QuadTree[] quads;

	public QuadTree(int x, int y, int rad)
	{
		center = new Vector2(x,y);
		radius = rad;
		identifier = -1;
	}
	public QuadTree(Vector2 cen, int rad)
	{
		center = cen;
		radius = rad;
		identifier = -1;
	}

	public void Identify(int root, int idx)
	{
		identifier = root*10+(idx+1);
	}

	public bool HasLeaf() { return quads != null; }

	public void MakeLeaf()
	{
		int rhalf = radius/2;
		
		quads = new QuadTree[4];
		quads[0] = new QuadTree((int)center.x + IsoMath.OrdinalStepX[(int)OrdinalDir.NW]*rhalf, 
								(int)center.y + IsoMath.OrdinalStepY[(int)OrdinalDir.NW]*rhalf,
								rhalf);
		quads[1] = new QuadTree((int)center.x + IsoMath.OrdinalStepX[(int)OrdinalDir.NE]*rhalf, 
								(int)center.y + IsoMath.OrdinalStepY[(int)OrdinalDir.NE]*rhalf,
								rhalf);
		quads[2] = new QuadTree((int)center.x + IsoMath.OrdinalStepX[(int)OrdinalDir.SE]*rhalf, 
								(int)center.y + IsoMath.OrdinalStepY[(int)OrdinalDir.SE]*rhalf,
								rhalf);
		quads[3] = new QuadTree((int)center.x + IsoMath.OrdinalStepX[(int)OrdinalDir.SW]*rhalf, 
								(int)center.y + IsoMath.OrdinalStepY[(int)OrdinalDir.SW]*rhalf,
								rhalf);
		quads[0].Identify(identifier,0);
		quads[1].Identify(identifier,1);
		quads[2].Identify(identifier,2);
		quads[3].Identify(identifier,3);

		
		/*DebugRenderer.Instance.AddLine(DebugRenderMode.ALL, (Vector3)center, (Vector3)quads[0].center, 35f, new Color(0.3f,1.0f,0.0f,0.7f));
		DebugRenderer.Instance.AddLine(DebugRenderMode.ALL, (Vector3)center, (Vector3)quads[1].center, 35f, new Color(1.0f,0.3f,0.0f,0.7f));
		DebugRenderer.Instance.AddLine(DebugRenderMode.ALL, (Vector3)center, (Vector3)quads[2].center, 35f, new Color(0.0f,0.3f,1.0f,0.7f));
		DebugRenderer.Instance.AddLine(DebugRenderMode.ALL, (Vector3)center, (Vector3)quads[3].center, 35f, new Color(0.0f,1.0f,0.3f,0.7f));
		*/
		if (identifier < Mathf.Pow(10,depth))
		{
			quads[0].MakeLeaf();
			quads[1].MakeLeaf();
			quads[2].MakeLeaf();
			quads[3].MakeLeaf();
		}
	}


	/*
	To use the grid
	public void MakeQuadTree()
	{

		int radius = Mathf.Max((maxX-minX)/2, (maxY-minY)/2);
		rootQuad = new QuadTree((maxX+minX)/2, (maxY+minY)/2, radius);
		rootQuad.Identify(0,0);
		rootQuad.MakeLeaf();
		QuadTree.Root = rootQuad;
	}*/
}
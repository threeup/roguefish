using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public enum DebugRenderMode
{
    NONE,
    COLLISION,
    AI,
}

public struct LineData
{
    public Vector3 src;
    public Vector3 dst;
    public bool hasWidth;
    public Vector3 width;
    public float expire;
    public Color color;

    public LineData(Vector3 src, Vector3 dst, float expire, Color color)
    {
        this.src = src;
        this.dst = dst;
        this.expire = expire;
        this.color = color;
        this.hasWidth = false;
        this.width = Vector3.zero;
    }

    public LineData(Vector3 src, Vector3 dst, Vector3 width, float expire, Color color)
    {
        this.src = src;
        this.dst = dst;
        this.expire = expire;
        this.color = color;
        this.hasWidth = true;
        this.width = width;
    }

}

public class DebugRenderer : LazySingletonBehavior<DebugRenderer>
{    
    public DebugRenderMode renderMode = DebugRenderMode.COLLISION;
    private int maxLines = 512;
    
    public List<Queue<LineData>> timedLinesList;
    public Queue<LineData> timedLines;
    public Queue<LineData> frameLines;

    bool isInitialized = false;
    public bool IsInitialized{ get { return isInitialized; } }
    
    public Material _glMaterial;

    private float thickness = 0.2f;

    public bool ShouldRender { get { return renderMode != DebugRenderMode.NONE; } set { } } 

    public void Awake()
    {

        this.gameObject.transform.position = Camera.main.transform.position;
        this.gameObject.transform.rotation = Camera.main.transform.rotation;
        Initialize();       
    }

    public void Initialize()
    {
        if( isInitialized )
            return; 
        isInitialized = true;
        timedLinesList = new List<Queue<LineData>>();
        int renderModesCount = System.Enum.GetValues(typeof(DebugRenderMode)).Length;
        for(int i=0; i < renderModesCount; ++i)
        {
            timedLinesList.Add(new Queue<LineData>(maxLines));
        }
		timedLines = timedLinesList[(int)renderMode];
        frameLines = new Queue<LineData>(maxLines);
    }

    public DebugRenderMode GetRenderMode()
    {
        return renderMode;
    }

    public void SetRenderMode(DebugRenderMode channel)
    {
        if (renderMode == channel)
        {
            return;
        }
        
        renderMode = channel;
        timedLines = timedLinesList[(int)channel];
    }

    public void ClearRenderMode(DebugRenderMode mode)
    {
        timedLinesList[(int)renderMode].Clear();
    }

    public void AddOutlineSquare(DebugRenderMode channel, Vector3 start, float radius, float expiration, Color col)
    {
        Vector3 b1 = start, b2 = start, b3 = start, b4 = start;
        b1.x += radius;
        b1.z -= radius;
        b2.x -= radius;
        b2.z -= radius;
        b3.x -= radius;
        b3.z += radius;
        b4.x += radius;
        b4.z += radius;

        CreateLine(channel, PrepQuad(b1, b2, expiration, col));   
        CreateLine(channel, PrepQuad(b2, b3, expiration, col));   
        CreateLine(channel, PrepQuad(b3, b4, expiration, col));   
        CreateLine(channel, PrepQuad(b4, b1, expiration, col));   
    }

    public void AddSolidSquare(DebugRenderMode channel, Vector3 start, float radius, float expiration, Color col)
    {
        Vector3 b1 = start, b2 = start;
        b1.x -= radius;
        b2.x += radius;
        CreateLine(channel, PrepQuad(b1, b2, Vector3.forward*radius, expiration, col));
    }

    public void AddQuad(DebugRenderMode channel, Vector3 start, Vector3 end, float expiration, Color col)
    {
        CreateLine(channel, PrepQuad(start, end, expiration, col));
    }

    public void AddLine(DebugRenderMode channel, Vector3 start, Vector3 end, float expiration, Color col)
    {
        CreateLine(channel, PrepLine(start, end, expiration, col));
    }

    private void CreateLine(DebugRenderMode channel, LineData data)
    {
        if (data.expire < 0.01f)
        {
            if (IsRendererActive() && renderMode == channel)
            {
                frameLines.Enqueue(data);
            }
        }
        else
        {
            data.expire += Time.time;
            Queue<LineData> currentQueue = timedLinesList[(int)channel];
            if (currentQueue.Count >= maxLines)
            {
                currentQueue.Dequeue();
            }
            currentQueue.Enqueue(data);         
        }
    }

    public void AddSphere(DebugRenderMode channel, Vector3 center, float radius, Color col)
    {
        AddCapsule(channel, center, radius, radius, col);
    }

    public void AddCapsule(DebugRenderMode channel, Vector3 center, float radius, float height, Color col)
    {
		Camera mainCamera = Camera.main;

		Vector3[] vertices = new Vector3[6];
		vertices[0] = mainCamera.WorldToScreenPoint(center+height*0.5f*Vector3.up);
		vertices[1] = mainCamera.WorldToScreenPoint(center-height*0.5f*Vector3.up);
		vertices[2] = mainCamera.WorldToScreenPoint(center+radius*Vector3.forward);
		vertices[3] = mainCamera.WorldToScreenPoint(center-radius*Vector3.right);
        vertices[4] = mainCamera.WorldToScreenPoint(center-radius*Vector3.forward);
		vertices[5] = mainCamera.WorldToScreenPoint(center+radius*Vector3.right);


		for(int j=0; j<2; ++j)
		{
			for(int i=2; i<6; ++i)
			{
				CreateLine(channel, new LineData(vertices[j], vertices[i], 0f, col));
			}
		}
        CreateLine(channel, new LineData(vertices[0], vertices[1], 0f, col));
        CreateLine(channel, new LineData(vertices[2], vertices[3], 0f, col));
        CreateLine(channel, new LineData(vertices[3], vertices[4], 0f, col));
        CreateLine(channel, new LineData(vertices[4], vertices[5], 0f, col));
		CreateLine(channel, new LineData(vertices[5], vertices[2], 0f, col));
    }

    public void AddBounds(DebugRenderMode channel, Bounds bounds, Color col)
    {
        Camera mainCamera = Camera.main;

        Vector3[] vertices = new Vector3[8];
        Vector3 offset = bounds.extents;
        vertices[0] = mainCamera.WorldToScreenPoint(bounds.center+offset);
        offset.x = -offset.x;
        vertices[1] = mainCamera.WorldToScreenPoint(bounds.center+offset);
        offset.y = -offset.y;
        vertices[2] = mainCamera.WorldToScreenPoint(bounds.center+offset);
        offset.x = -offset.x;
        vertices[3] = mainCamera.WorldToScreenPoint(bounds.center+offset);
        offset.z = -offset.z;
        offset.y = -offset.y;
        
        vertices[4] = mainCamera.WorldToScreenPoint(bounds.center+offset);
        offset.x = -offset.x;
        vertices[5] = mainCamera.WorldToScreenPoint(bounds.center+offset);
        offset.y = -offset.y;
        vertices[6] = mainCamera.WorldToScreenPoint(bounds.center+offset);
        offset.x = -offset.x;
        vertices[7] = mainCamera.WorldToScreenPoint(bounds.center+offset);


        for(int j=0; j<4; ++j)
        {
            CreateLine(channel, new LineData(vertices[j], vertices[j+4], 0f, col));
        }
        for(int j=0; j<4; ++j)
        {
            int next = j == 3 ? 0 : j+1;
            CreateLine(channel, new LineData(vertices[j], vertices[next], 0f, col));
        }
        for(int j=4; j<8; ++j)
        {
            int next = j == 7 ? 4 : j+1;
            CreateLine(channel, new LineData(vertices[j], vertices[next], 0f, col));
        }
    }

    private LineData PrepLine(Vector3 start, Vector3 end, float expire, Color col)
    {
        return new LineData(Camera.main.WorldToScreenPoint(start), Camera.main.WorldToScreenPoint(end), expire, col);
    }

    private LineData PrepQuad(Vector3 start, Vector3 end, float expire, Color col)
    {
        Vector3 widthVector = (end - start).normalized;
        widthVector.y = widthVector.z;//use y as buffer
        widthVector.z = widthVector.x;
        widthVector.x = widthVector.y;
        widthVector.y = 0.0f;
        widthVector *= thickness/2f;
        return new LineData(start, end, widthVector, expire, col);
    }

    private LineData PrepQuad(Vector3 start, Vector3 end, Vector3 width, float expire, Color col)
    {
        return new LineData(start, end, width, expire, col);
    }
    
    public void Update()
    {
    }

    public void OnPostRender() 
    {
        if (IsRendererActive() && (timedLines.Count > 0 || frameLines.Count > 0))
        {
            RenderProj();
        }
    }   


    public void RenderProj()
    {
        _glMaterial.SetPass(0);
        float currentTime = Time.time;
        GL.PushMatrix();
        GL.LoadPixelMatrix();
		GL.Begin(GL.LINES);
        
        foreach(LineData line in timedLines)
        {
			if (line.expire > currentTime && !line.hasWidth && line.src.z > 0f && line.dst.z > 0f)
            {
				GL.Color(line.color);
				GL.Vertex3(line.src.x, line.src.y, 0f);
				GL.Vertex3(line.dst.x, line.dst.y, 0f);
            }
        }
        foreach(LineData line in frameLines)
        {
			if (!line.hasWidth && line.src.z > 0f && line.dst.z > 0f)
            {
				GL.Color(line.color);
                GL.Vertex3(line.src.x, line.src.y, 0f);
                GL.Vertex3(line.dst.x, line.dst.y, 0f);
            }
        }
		GL.End();
		GL.PopMatrix();
		GL.PushMatrix();
		GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
		GL.Begin(GL.QUADS);
		foreach(LineData line in timedLines)
		{
			if (line.expire > currentTime)
			{
				if (line.hasWidth)
				{
					GL.Color(line.color);
					GL.Vertex3(line.src.x + line.width.x, line.src.y, line.src.z - line.width.z);
					GL.Vertex3(line.src.x - line.width.x, line.src.y, line.src.z + line.width.z);
					GL.Vertex3(line.dst.x - line.width.x, line.dst.y, line.dst.z + line.width.z);
					GL.Vertex3(line.dst.x + line.width.x, line.dst.y, line.dst.z - line.width.z);
					GL.Vertex3(line.dst.x + line.width.x, line.dst.y, line.dst.z - line.width.z);
					GL.Vertex3(line.dst.x - line.width.x, line.dst.y, line.dst.z + line.width.z);
					GL.Vertex3(line.src.x - line.width.x, line.src.y, line.src.z + line.width.z);
					GL.Vertex3(line.src.x + line.width.x, line.src.y, line.src.z - line.width.z);
				}
			}
		}
		foreach(LineData line in frameLines)
		{
			if (line.hasWidth)
			{
				GL.Color(line.color);
				GL.Vertex3(line.src.x + line.width.x, line.src.y, line.src.z - line.width.z);
				GL.Vertex3(line.src.x - line.width.x, line.src.y, line.src.z + line.width.z);
				GL.Vertex3(line.dst.x - line.width.x, line.dst.y, line.dst.z + line.width.z);
				GL.Vertex3(line.dst.x + line.width.x, line.dst.y, line.dst.z - line.width.z);
			}
		}
        
       
        GL.End();
        GL.PopMatrix();
        frameLines.Clear();
    }


    public bool IsRendererActive()
    {
        return isInitialized && _glMaterial != null;
    }
}

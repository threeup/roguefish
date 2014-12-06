using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using System;

public enum BackerEntitlements
{
	Backer_EarlyAccess,
	Backer_DocWagon,
	Backer_Special,
	Backer_Level,
	Backer_ID
}


public static class Utilities
{
#if UNITY_EDITOR
	public static void ClearLog()
	{
	    Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
	
	    System.Type type = assembly.GetType("UnityEditorInternal.LogEntries");
	    MethodInfo method = type.GetMethod("Clear");
	    method.Invoke(new object(), null);
	}
	
	public static void SetSelectedGameObject(GameObject target)
	{
		Selection.activeGameObject = target;
	}
#else
	public static void ClearLog(){}
	public static void SetSelectedGameObject(GameObject target){}
#endif
	
	public static string GetStackTrace(int skipFrames = 1)
	{
		StringBuilder sb = new StringBuilder();
		System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace(true);

		// maybe count-skipframes;
		string stackIndent = "|";
		string filename = "";
        for(int i = skipFrames; i< st.FrameCount; i++ )
        {
        	stackIndent += " ";
            System.Diagnostics.StackFrame sf = st.GetFrame(i);
            sb.Append(stackIndent);
            filename = sf.GetFileName();
            if (filename != null)
            {
	            int lastIndex = filename.LastIndexOf("\\");
	            if (lastIndex > 0)
	            {
	            	sb.Append(filename.Substring(lastIndex));	
	            }
	            else
	            {
	            	sb.Append(filename);	
	            }
	        }
            
            sb.Append(sf.GetFileLineNumber());
        }
        return sb.ToString();
	}

	public static string FormattedRealTime()
	{
		float seconds = Mathf.Round(Time.realtimeSinceStartup);
		float minutes = Mathf.Floor(seconds / 60f);
		seconds -= minutes*60f;
		return String.Format("@{0:00}:{1:00}", minutes,seconds);
	}


	public static string FormattedTime(float ticks)
	{
		float seconds = Mathf.Round(ticks);
		float minutes = Mathf.Floor(seconds / 60f);
		seconds -= minutes*60f;
		float hours = Mathf.Floor(minutes / 60f);
		minutes -= hours*60f;
		return String.Format("{0}:{1:00}:{2:00}",hours, minutes,seconds);
	}

	public static string FormattedTimeLetters(float ticks)
	{
		float seconds = Mathf.Round(ticks);
		float minutes = Mathf.Floor(seconds / 60f);
		seconds -= minutes*60f;
		float hours = Mathf.Floor(minutes / 60f);
		minutes -= hours*60f;
		return String.Format("{0}h {1:00}m",hours, minutes);
	}

	public static string FormattedMinutesLetters(float minutes)
	{
		float hours = Mathf.Floor(minutes / 60f);
		minutes -= hours*60f;
		return String.Format("{0}h {1:00}m",hours, minutes);
	}
	
	public static string ColorToHex(Color32 color)
	{
		return (color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2"));
		
	}
	
	public static Color HexToColor(string hex)
	{
		byte r = byte.Parse(hex.Substring(0,2), System.Globalization.NumberStyles.HexNumber);
		byte g = byte.Parse(hex.Substring(2,2), System.Globalization.NumberStyles.HexNumber);
		byte b = byte.Parse(hex.Substring(4,2), System.Globalization.NumberStyles.HexNumber);
		return new Color32(r,g,b, 255);
	}
	
	public static Color AdjustRGBAColor(Color32 color, float r = 1f, float g = 1f, float b = 1f, float a = 1f)
	{
		UnityEngine.Color c = color;
		
		c.r *= r;
		c.g *= g;
		c.b *= b;
		c.a *= a;
		
		return c;
	}
	
	public static Color AdjustHSBColor(Color32 color, float h = 1f, float s = 1f, float b = 1f)
	{
		HSBColor hsbColor = HSBColor.FromColor(color);
		
		hsbColor.h *= h;
		hsbColor.s *= s;
		hsbColor.b *= b;
		
		return hsbColor.ToColor();
	}
	
		
	public static Shader FindRequiredShader(string name)
	{
		Shader s = Shader.Find (name);
		if( s == null )
			throw new System.NullReferenceException("Required shader not found: " + name);
		return s;
	}
	
	public static T FindChildComponent<T>(GameObject go, string name) where T : UnityEngine.Component
	{
		T[] children = go.GetComponentsInChildren<T>();
		for( int i = 0; i < children.Length; i++ )
		{
			T child = children[i];
			if( child.gameObject.name == name )
			{
				return child;
			}
		}
		return null;
	}

	public static bool CheckSymbol(string text, int index)
	{
		int length = text.Length;

		if (index + 4 < length)
		{
			if (text[index + 2] == '-' && text[index + 3] == '}' && text[index + 4] == '}')
			{
				return true;
			}
			else if (index + 9 < length)
			{
				if (text[index + 8] == '}' && text[index + 9] == '}')
				{
					return true;
				}
			}
		}
		return false;
	}
	
		
	public static string HideSymbols(string text)
	{
		if (text != null)
		{
			// Replace special characters
			text = text.Replace("\u2013", "-");
			text = text.Replace("\u2014", "-");
			text = text.Replace("\u2018", "\'");
			text = text.Replace("\u2019", "\'");
			text = text.Replace("\u201C", "\"");
			text = text.Replace("\u201D", "\"");
			text = text.Replace("\u2026", "...");
			
			for (int i = 0, imax = text.Length; i < imax;)
			{
				char c = text[i];
				char prevC = ' ';
				char nextC = ' ';
				
				if (i > 0)
				{
					prevC = text[i-1];
				}
				if (i+1 < text.Length)
				{
					nextC = text[i+1];
				}

				if (c == '{' && nextC == '{' && prevC != '\\')
				{
					if (CheckSymbol(text, i))
					{
						text = text.Insert(i, "\\");
						imax = text.Length;
					}
				}
				++i;
			}
		}
		return text;
	}
	
	public static string UnhideSymbols(string text)
	{
		if (text != null)
		{
			for (int i = 0, imax = text.Length; i < imax;)
			{
				char c = text[i];
				char prevC = ' ';
				char nextC = ' ';
				
				if (i > 0)
				{
					prevC = text[i-1];
				}
				if (i+1 < text.Length)
				{
					nextC = text[i+1];
				}

				if (c == '{' && nextC == '{' && prevC == '\\')
				{
					text = text.Remove(i-1, 1);
					imax = text.Length;
					continue;
				}
				++i;
			}
		}
		return text;
	}
	
	public static string UppercaseFirst(string text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return string.Empty;
		}
		return char.ToUpper(text[0]) + text.Substring(1);
	}
	
	
	static public void SetLayerInChildren(Transform t, int layer)
	{
		if (t.gameObject.layer != LayerHelper.Interactable && t.gameObject.layer != LayerHelper.IgnoreRaycast && 
            (layer != LayerHelper.PlayerDefault || t.gameObject.layer != LayerHelper.VFX )
            )
			t.gameObject.layer = layer;
		
		for( int i = 0, n = t.childCount; i < n; i++ )
		{
			SetLayerInChildren(t.GetChild(i), layer);
		}
	}
	static public void SetUIHidden(GameObject root, bool hidden)
	{
		bool isHidden = root.layer == LayerHelper.UIHidden;
		if (hidden == isHidden)
			return;
		SetLayerInChildren(root.transform, hidden ? LayerHelper.UIHidden : LayerHelper.UI);
	}
	
	static public string Base64Encode(string str)
	{
		return System.Convert.ToBase64String(System.Text.Encoding.Unicode.GetBytes(str));
	}
	
	static public string Base64Decode(string str)
	{
		return System.Text.Encoding.Unicode.GetString(System.Convert.FromBase64String(str));
	}
	
	
	static public Hashtable ParseProperties(string propertiesText)
	{
		return (Hashtable)NGUIJson.jsonDecode(propertiesText.Trim());
	}
	
	
	
	
	static public System.Text.RegularExpressions.Regex WildcardRegex(string pattern, bool ignoreCase = true)
	{
		return new Regex("^" + Regex.Escape(pattern)
				.Replace(@"\*", ".*")
				.Replace(@"\?", ".") + "$", ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None);
	}
	
	
	enum TextExpansionType
	{
		NONE,
		ALLUPPER,
		UPPERFIRST,
		ALLLOWER,
	}
	
	static string ModifyTextExpansion(string text, TextExpansionType type)
	{
		switch (type)
		{
		case TextExpansionType.ALLUPPER:
			return text.ToUpper();
		case TextExpansionType.UPPERFIRST:
			return UppercaseFirst(text);
		case TextExpansionType.ALLLOWER:
			return text.ToLower();
		}
		
		return text;
	}
		
	public static string RemoveEnd(this string input, string end, System.StringComparison comparison = System.StringComparison.Ordinal)
	{
		if( input.EndsWith(end, comparison) )
			return input.Substring(0, input.Length - end.Length);
		
		return input;
	}
	
	public static string RemoveStart(this string input, string start, System.StringComparison comparison = System.StringComparison.Ordinal)
	{
		if( input.StartsWith(start, comparison) )
			return input.Substring(start.Length);
		
		return input;
	}
	
	public static string CommasInNumbers(double number)
	{
		return string.Format("{0:#,###0}", number);
	}
	
	public static string FromEnumToString(string enumToString)
	{
		string[] strParts = enumToString.Split('_');
		enumToString = "";
		foreach (string str in strParts)
		{
			string s = Utilities.UppercaseFirst(str.ToLower());
			enumToString = enumToString + s + " ";
		}
		return enumToString.Trim();
	}
	
	public static Rect ResizeRectFromFixedResolution(Rect rect, float resWidth = 1024f, float resHeight = 768f)
	{
		float width = rect.width;
		float height = rect.height;
		float x = rect.x;
		float y = rect.y;
		
		bool retinal = false;

		
		if (Screen.width > resWidth)
		{
			width = rect.width * resWidth / Screen.width;
			x = rect.x * resWidth / Screen.width;
			
			if (retinal)
			{
				width *= 2;
				x *= 2;
			}
		}
		
		if (Screen.height > resHeight)
		{
			height = rect.height * resHeight / Screen.height;
			y = rect.y * Screen.height / resHeight * 1.15f;
			
			if (retinal)
			{
				height *= 2;
				y /= 2;
			}
		}
		
		return new Rect(x, y, width, height);
	}
	
	public static int CarouselIndex(int start, int offset, int min, int max)
	{
		int end = start;
		while (Mathf.Abs(offset) > 0)
		{
			if (offset > 0)
			{
				++end;
				if (end > max)
				{
					end = min;
				}
				--offset;
			}
			else if (offset < 0)
			{
				--end;
				if (end < min)
				{
					end = max;
				}
				++offset;
			}
		}
		
		return end;
	}

    public static int GetWordCount(string s)
    {
        MatchCollection collection = Regex.Matches(s, @"[\S]+");
        return collection.Count;
    }

	public class Cloner
	{
        public static T DeepCopy<T>(T obj)
		{
            if (obj == null)
                throw new ArgumentNullException("Object cannot be null");
            return (T)Process(obj);
        }

        static object Process(object obj)
		{
            if (obj==null)
                return null;
            Type type = obj.GetType();
            if (type.IsValueType || type == typeof(string))
			{
                return obj;
            }
            else if (type.IsArray)
			{
                Type elementType = Type.GetType(type.AssemblyQualifiedName.Replace("[]", string.Empty));
                var array = obj as Array;
                Array copied = Array.CreateInstance(elementType, array.Length);
                for (int i = 0; i<array.Length; ++i)
				{
                    copied.SetValue(Process(array.GetValue(i)), i);
                }
                return Convert.ChangeType(copied, obj.GetType());
            }
            else if (type.IsClass)
			{
                object toret = Activator.CreateInstance(obj.GetType());
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
				{
                    object fieldValue = field.GetValue(obj);
                    if (fieldValue == null)
                        continue;
                    field.SetValue(toret, Process(fieldValue));
                }
                return toret;
            }
            else
                throw new ArgumentException("Unknown type");
        }

    }
	
	public static bool StringContainsCaseInsensitive(string lhs, string rhs)
	{
		return (lhs.IndexOf(rhs, StringComparison.OrdinalIgnoreCase) >= 0);
	}

    public static Quaternion GetRotationFromMatix(Matrix4x4 m)
    {
        float qw = Mathf.Sqrt(1f + m.m00 + m.m11 + m.m22) / 2.0f;
        float w = 4.0f * qw;
        float qx = (m.m21 - m.m12) / w;
        float qy = (m.m02 - m.m20) / w;
        float qz = (m.m10 - m.m01) / w;
        return new Quaternion(qx, qy, qz, qw);
    }
	
	public static string CombineStrings(ref StringBuilder sb, params string [] args)
	{
		sb.Length = 0;
		for (int i = 0; i < args.Length; ++i)
		{
			sb.Append(args[i]);
		}
		return sb.ToString();
	}

    public static string CleanFileName(string fileName)
    {
        Regex r = new Regex("[^0-9a-zA-z]");
        fileName = r.Replace(fileName, "_");
        return fileName;
    }

    public static float HueToRGB(float p, float q, float t)
    {
    	if(t < 0f) 
    		t += 1f;
        if(t > 1f) 
        	t -= 1f;
        if(t < 1f/6f) 
        	return p + (q - p) * 6f * t;
        if(t < 1f/2f) 
        	return q;
        if(t < 2f/3f) 
        	return p + (q - p) * (2f/3f - t) * 6f;
        return p;
    }
    // adapted from http://en.wikipedia.org/wiki/HSL_color_space

    public static Color HSLtoRGB(float h, float s, float l)
    {
    	Color output = Color.black;
    	if (s < 0.01f)
    	{
    		return output;
    	}

        float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
        float p = 2f * l - q;
        output.r = HueToRGB(p, q, h + 1f/3f);
        output.g = HueToRGB(p, q, h);
        output.b = HueToRGB(p, q, h - 1f/3f);
        return output;
    }

	public static void ApplyScale(GameObject go, Vector2 screenFactor)
    {
        Vector3 scale = go.transform.localScale;
        scale.x *= screenFactor.x;
        scale.y *= screenFactor.y;
        go.transform.localScale = scale;
        RectTransform rectTrans = (RectTransform)go.transform;
        Vector3 pos = rectTrans.anchoredPosition;
        pos.x *= screenFactor.x;
        pos.y *= screenFactor.y;
        rectTrans.anchoredPosition = pos;
    }
    
    public static void ReverseScale(GameObject go, Vector2 screenFactor)
    {
    	Vector3 scale = go.transform.localScale;
        scale.x /= screenFactor.x;
        scale.y /= screenFactor.y;
        go.transform.localScale = scale;
        RectTransform rectTrans = (RectTransform)go.transform;
        Vector3 pos = rectTrans.anchoredPosition;
        pos.x /= screenFactor.x;
        pos.y /= screenFactor.y;
        rectTrans.anchoredPosition = pos;
    }

    public static bool Intersect(Rect a, Rect b)
    {
        return (a.x < b.xMax) && (a.xMax > b.x) && (a.y < b.yMax) && (a.yMax > b.y);
    }
}


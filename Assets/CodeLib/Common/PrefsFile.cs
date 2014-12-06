
using System;
using System.Collections;
using System.IO;

/// <summary>
/// A Unity PlayerPrefs-style class that can read/write to an explicit file. It uses JSON
/// to serialize the data.
/// </summary>
public class PrefsFile
{
	protected string filename;
	
	protected bool dirty = false;
	
	protected Hashtable data = new Hashtable();
	
	protected bool _AutoSave = false;
	public bool AutoSave
	{
		get
		{
			return _AutoSave;
		}
		set
		{
			_AutoSave = value;
			if( dirty )
				Save();
		}
	}

	
	public string Filename
	{
		get
		{
			return filename;
		}
		set
		{
			data.Clear();
			filename = value;
			Load();
		}
	}
	
	public ICollection Keys
	{
		get
		{
			return data.Keys;
		}
	}
	
	public PrefsFile()
	{
	}
	
	public PrefsFile(string filename)
	{
		Filename = filename;
	}
	
	public int GetInt(string key, int defaultValue = 0)
	{
		if( !HasKey(key) )
			return defaultValue;
		
		object o = data[key];
		if( o is string )
		{
			int v = defaultValue;
			return int.TryParse((string)o, out v) ? v : defaultValue;
		}
		
		try
		{
			return (int)o;
		}
		catch( InvalidCastException )
		{
			return defaultValue;
		}
	}
	
	public void SetInt(string key, int value)
	{
		data[key] = value;
		dirty = true;
		if( _AutoSave )
			Save();
	}
	
	public float GetFloat(string key, float defaultValue = 0.0f)
	{
		if( !HasKey(key) )
			return defaultValue;
		
		object o = data[key];
		if( o is string )
		{
			float v = defaultValue;
			return float.TryParse((string)o, out v) ? v : defaultValue;
		}
		
		try
		{
			return (int)o;
		}
		catch( InvalidCastException )
		{
			return defaultValue;
		}
	}
	
	public void SetFloat(string key, float value)
	{
		data[key] = value;
		dirty = true;
		if( _AutoSave )
			Save();
	}
	
	public string GetString(string key, string defaultValue = "")
	{
		if( !HasKey(key) )
			return defaultValue;
		
		return data[key].ToString();
	}
	
	public void SetString(string key, string value)
	{
		data[key] = value;
		dirty = true;
		if( _AutoSave )
			Save();
	}
	
	public void SetUInt32(string key, uint value)
	{
		data[key] = value.ToString(); // Store as string to not lose precision during serialization
		dirty = true;
		if( _AutoSave )
			Save();
	}
	
	public uint GetUInt32(string key, uint defaultValue = 0)
	{
		if( !HasKey(key) )
			return defaultValue;
		
		object o = data[key];
		if( o is string )
		{
			uint v = defaultValue;
			return uint.TryParse((string)o, out v) ? v : defaultValue;
		}
		
		try
		{
			return (uint)o;
		}
		catch( InvalidCastException )
		{
			return defaultValue;
		}
	}

	public void SetUInt64(string key, ulong value)
	{
		data[key] = value.ToString(); // Store as string to not lose precision during serialization
		dirty = true;
		if( _AutoSave )
			Save();
	}
	
	public ulong GetUInt64(string key, ulong defaultValue = 0)
	{
		if( !HasKey(key) )
			return defaultValue;
		
		object o = data[key];
		if( o is string )
		{
			ulong v = defaultValue;
			return ulong.TryParse((string)o, out v) ? v : defaultValue;
		}
		
		try
		{
			return (ulong)o;
		}
		catch( InvalidCastException )
		{
			return defaultValue;
		}
	}
	
	public bool HasKey(string key)
	{
		return data.ContainsKey(key);	
	}
	
	public void DeleteKey(string key)
	{
		if( data.ContainsKey(key) )
		{
			data.Remove(key);
			dirty = true;
			if( _AutoSave )
				Save();
		}
	}
	
	public void DeleteAll()
	{
		data.Clear();
		dirty = true;
		if( _AutoSave )
			Save();
	}
	
	virtual protected bool SaveImpl(byte[] bytes)
	{
		if( string.IsNullOrEmpty(filename) )
			return false;
		File.WriteAllBytes(filename, bytes);
		return true;
	}
	
	virtual protected bool LoadImpl(out byte[] bytes)
	{
		if( string.IsNullOrEmpty(filename) || !File.Exists(filename) )
		{
			bytes = null;
			return false;
		}
		
		bytes = File.ReadAllBytes(filename);
		
		return true;
	}
	
	public void Save()
	{
		if( !string.IsNullOrEmpty(filename) )
		{
			if( SaveImpl(System.Text.UTF8Encoding.UTF8.GetBytes(NGUIJson.jsonEncode(data))) )
				dirty = false;
		}
	}
	
	public void Load()
	{
		data = null;
		if( !string.IsNullOrEmpty(filename) )
		{
			byte[] bytes = null;
			if( LoadImpl(out bytes) )
				data = (Hashtable)NGUIJson.jsonDecode(System.Text.UTF8Encoding.UTF8.GetString(bytes));
		}
		if( data == null )
			data = new Hashtable();
		dirty = false;
	}
}

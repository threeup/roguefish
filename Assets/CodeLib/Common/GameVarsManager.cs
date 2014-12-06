using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Reflection;

public class GameVarsManager
{
	public Dictionary<string, object> initialValues = new Dictionary<string, object>();
	
	public const string ERROR_STRING = "ERROR: INVALID VALUE";
	public string varStatusString = " STATUS: OK ";
	
	public const string CONSTANTS_FILE = "Constants.cs";
	public const string BACKUP_CONSTANTS_FILE = "BACKUP_Constants.cs.txt";
	public const string CONSTANTS_FILE_DIR = "../Shadowrun/Assets/Scripts/";
	
	private bool dirty = false;
	
	public bool ShouldSave()
	{
		return dirty;
	}
	public GameVarsManager()
	{

		foreach(FieldInfo fi in typeof(Constants).GetFields())
		{
			initialValues.Add(fi.Name, fi.GetValue(null));
		}
		
		
	}
	
	
	// Reset to default sets all of the variables 
	// back to whatever they were when the game loaded.
	// These values will always be the same until recompiled.
	public void ResetToDefault()
	{
		var gamevarType = typeof(Constants);
		foreach (KeyValuePair<string, object> gvar in initialValues)
		{
			FieldInfo field = gamevarType.GetField(gvar.Key);
			if (field != null)
			{
				field.SetValue(null, gvar.Value);
			}
		}
		varStatusString = " STATUS: All values reverted to the current build defaults. (Click save to write to disc).";
	}
	

	
	public void Cancel()
	{
		varStatusString = " STATUS: Changes discarded";
	}
	
	
	
	// get the current value of a constant in Constants.cs
	// represented by a string.
	public string GetCurrentValueAsString(string fieldName)
	{
		var gamevarType = typeof(Constants);
		FieldInfo field = gamevarType.GetField(fieldName);
		if (field != null)
		{
			return field.GetValue(null).ToString();
		}
		
		Logger.LogWarning(LogChannel.SYS, "INVALID FIELD NAME!");
		return "";
	}
	
	public string GetTypeAsString(System.Type laType)
	{
		if(typeof(float) == laType)
			return "float";
		
		if(typeof(bool) == laType)
			return "bool";
		
		if(typeof(double) == laType)
			return "double";
		
		if(typeof(int) == laType)
			return "int";
		
		if(typeof(uint) == laType)
			return "unsigned int";
		
		if(typeof(string) == laType)
			return "string";
		
		//if(typeof(Constants.AIAttackRandomization) == laType)
		//	return "AI Attack Randomization Level";
				
		return laType.ToString();
	}
	public void ShowMetaData(string varName)
	{
		var gamevarType = typeof(Constants);
		FieldInfo field = gamevarType.GetField(varName);
		if (field != null)
		{
			varStatusString = varName + ": Type=" + GetTypeAsString(field.FieldType) + " - Initial Value="  + initialValues[varName].ToString();
		}
	}
	
	
	public void SaveConstantsFile(bool saveBackup = false)
	{
		
#if UNITY_IPHONE || UNITY_ANDROID
		return;
	}
#else
		if (saveBackup)
			Logger.Log(LogChannel.SYS, "Saving " + BACKUP_CONSTANTS_FILE);
		else
			Logger.Log(LogChannel.SYS, "Saving " + CONSTANTS_FILE);
		
		List<string> fileLines = new List<string>();
		//bool savedOK = true;
		try
		{
			// read in file line by line
			using(StreamReader sr = new StreamReader(CONSTANTS_FILE_DIR + CONSTANTS_FILE))
			{
				var gamevarType = typeof(Constants);
				while(!sr.EndOfStream)
				{
					string currentString = sr.ReadLine();
					
					bool isVariableLine = false;
					
					isVariableLine = currentString.Contains("public") && currentString.Contains("static") && currentString.Contains("=") && currentString.Contains(";") && !currentString.Contains("NO_MODIFY");
					 
					if( isVariableLine )
					{
					
						// attempt to find the variable name for the current line
						string [] splitString = currentString.Split(new char[] {' ','=','\t'});
					
						List<string> removeValues = new List<string>();
					
						foreach(string str in splitString)
						{
							removeValues.Add(str);
						}
					

						while(removeValues.Remove(""));
					
						splitString = removeValues.ToArray();
						// just in case user is using spaces vs tabs...
						if (splitString.Length >= 4)
						{
							uint splitOffset = 0;
							
							foreach( string str in splitString )
							{
								if(str == "public")
								{
									break;
								}
								++splitOffset;
								
							}
							
							// just set it back to zero because the code below can handle it
							if(splitOffset + 3 >= splitString.Length)
								splitOffset = 0;

							// final test, the third entry should be the variable name...
							string varName = splitString[3 + splitOffset];
							string newString = "\tpublic static " + splitString[2 + splitOffset] + " " + varName + " = ";
							//Logger.LogError(varName);
							//Logger.LogError(splitString[0 + splitOffset] + "|" + splitString[1 + splitOffset] + "|" + splitString[2 + splitOffset]);
							FieldInfo field = gamevarType.GetField(varName);
							if (field != null)
							{
								object currentObject = field.GetValue(null);
								
								if(typeof(string) == field.FieldType)
								{
									newString += "\"" + currentObject + "\";";
								}
								else if(typeof(bool) == field.FieldType)
								{
									newString += currentObject.ToString().ToLower() + ";";
								}
								else if(typeof(float) == field.FieldType)
								{
									string floatString = currentObject.ToString();
									if(!floatString.Contains("."))
									{
										floatString += ".0";
									}
									newString += floatString + "f;";
								}
								else if(typeof(double) == field.FieldType)
								{
									string doubleString = currentObject.ToString();
									if(!doubleString.Contains("."))
									{
										doubleString += ".0";
									}
									newString += doubleString + ";";
								}
								else if(typeof(Color) == field.FieldType)
								{
									newString += "new " +  currentObject.ToString().Replace("RGBA", "Color").Replace(",", "f,").Replace(")","f)") + ";";
								}
								/*else if(typeof(Constants.AIAttackRandomization) == field.FieldType)
								{
									newString += "AIAttackRandomization."+currentObject+";";
								}*/
								else
								{
									newString += currentObject + ";";
								}
								fileLines.Add(newString);
							}
							else
							{
								fileLines.Add(currentString);
							}
						}
						else
						{
							fileLines.Add(currentString);
						}
					}
					else
					{
						fileLines.Add(currentString);
					}
				}
			}

			// save file line by line...
			try 
			{	
				string finalDir = "";
				if(saveBackup)
				{
					finalDir = BACKUP_CONSTANTS_FILE;
				}
				else
				{
					finalDir = CONSTANTS_FILE_DIR + CONSTANTS_FILE;
				}
				using(StreamWriter sr = new StreamWriter(finalDir, false))
				{
					// this is pretty sloppy, but I didn't want to waste any time on a fancier way
					foreach(string str in fileLines)
					{
						sr.WriteLine(str);
					}
					dirty = true;
				}
			}
			catch (System.Exception e)
			{
				//savedOK = false;
				Logger.LogWarning(LogChannel.SYS, "Unable to modify " + CONSTANTS_FILE + "(" + e.ToString() + ")");
			}
			
			
		}
		catch (System.Exception e)
		{
			//savedOK = false;
			Logger.LogWarning(LogChannel.SYS, "Unable to modify " + CONSTANTS_FILE + "(" + e.ToString() + ")");
		}
		
		//if(savedOK)
		//{
		//	varStatusString += " AND saved to disc!";
		//}
		//else
		//{
		//	varStatusString += " BUT could not save to disc!";
		//}
	}
#endif
	
	// takes an array of var names, and their values as a string.
	// for every name, it looks up the field in constants and 
	// attempts to convert the string back to the known type. 
	// if it fails, the string value passed in is modified to be displayed
	// to user.
	// afterwards, all current values are saved to a CS file
    public void Save(string[] names, ref string[] vals)
    {
		int errorCount = 0;
		var gamevarType = typeof(Constants);
    	for(int i=0; i < names.Length; ++i)
    	{
			object result = null;
			object current = initialValues[names[i]];			

			try
			{
				System.Type theType = current.GetType();
				if(theType == typeof(Color))
				{
					string tempStr = vals[i];
					tempStr = tempStr.Replace("RGBA", "");
					tempStr = tempStr.Replace("(","");
					tempStr = tempStr.Replace(")","");
					tempStr = tempStr.Replace(" ","");
					string[] values = tempStr.Split(new char[] {','});
					
					List<float> colorValues = new List<float>();
					
					foreach(string str in values)
					{
						if(str.Contains(" ")) continue;
						
						colorValues.Add( (float)System.Convert.ChangeType(str,System.TypeCode.Single));
					}
					
					result = new Color(colorValues[0], colorValues[1], colorValues[2], colorValues[3]);
				}
				/*else if (theType == typeof(Constants.AIAttackRandomization))
				{
					result = System.Enum.Parse(typeof(Constants.AIAttackRandomization), vals[i]);
				}*/
				else
				{
					result = System.Convert.ChangeType(vals[i], theType);
				}
			}
			catch
			{
				++errorCount;
				Logger.LogWarning( LogChannel.SYS, "CAUGHT EXCEPTION: Couldn't convert " + names[i] + " with val:  " + vals[i] + " to " + current.GetType());	
			}
			
			if(result == null)
			{
				vals[i] = "ERROR: INVALID VALUE";
			}
			else
			{
				//Logger.Log(LogChannel.MAIN, LogLevel.Info, "converted " + vals[i]  + " to " + result.GetType() + " : " + result.ToString());	
				
				FieldInfo field = gamevarType.GetField(names[i]);
				if (field != null)
				{
					field.SetValue(null, result);
				}
			}
    	}
		
		if(errorCount > 0)
		{
			varStatusString = " STATUS: " + errorCount.ToString() + " variable(s) not saved due to errors";
		}
		else
		{
			varStatusString = " STATUS: Variables modified successfully";
		}
		

		//SaveConstantsFile(true);
    }

	
}
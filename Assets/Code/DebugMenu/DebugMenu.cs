using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;


public class DebugMenu : LazySingletonBehavior<DebugMenu>
{
	const string DebugConsoleChannel = "DebugConsole";
	
	Dictionary<string, object> dataPairs = new Dictionary<string, object>();

    private bool hasScriptBindings = false;
    public bool isDebug = false;
    public bool isVisible = false;

	private List<DebugMenuEntry> entryList = new List<DebugMenuEntry>();
	private List<GameObject> hoveredList = new List<GameObject>();
	public Scrollbar visibleScrollbar;
	public GameObject scrollableArea;
	public GameObject buttonPrototype;
	public GameObject togglePrototype;
	public enum WindowMode
	{
		Icon = 0,
		Mini = 1,
		LogWindow = 2,
	}

	public enum ButtonArea
	{
		TopLeft = 0,
		TopRight = 1,
	}

	void Awake()
	{
		buttonPrototype.SetActive(false);
		togglePrototype.SetActive(false);
	}

	private bool hovered = false;
	public bool Hovered { get { return hovered; } } 

	public void Update()
	{
		if (!hasScriptBindings && isDebug)
		{
			BuildBindings();
		}
	}


	public void BuildBindings()
	{
        int entryUID = 0;



		foreach( KeyValuePair<string, object> pair in dataPairs )
        {
        	string valueName = string.Empty;
            if( pair.Value is MemberInfo )
            {
                object val = pair.Value;
                if (val is float)
                {
                	valueName = string.Format("{0:0.00}", ((float)val));
                }
                else
                {
                	valueName = val.ToString();
                }
                AddNumberEntry(entryUID, pair.Key, valueName);
               
            }
            else 
            {
                valueName = pair.Value.GetType().FullName;
                AddToggleEntry(entryUID, pair.Key, valueName);
            }
            
            entryUID++;
		}
		hasScriptBindings = true;
	}

	public void RefreshBindings(DebugMenuEntry entry)
	{
		foreach( KeyValuePair<string, object> pair in dataPairs )
        {
        	if (string.Compare(pair.Key, entry.contentLabel) == 0)
        	{
            	string valueName = string.Empty;
                if( pair.Value is MemberInfo )
                {
                    object val = pair.Value;
                    if (val is float)
                    {
                    	valueName = string.Format("{0:0.00}", ((float)val));
                    }
                    else
                    {
                    	valueName = val.ToString();
                    }
                    SetEntryContent(entry, pair.Key, valueName);
                }
                else
                {
                    valueName = pair.Value.GetType().FullName;
                    SetEntryContent(entry, pair.Key, valueName);
                }
                
            }
        }
	}


	public void AddNumberEntry(int entrySlot, string labelName, string valueName)
	{
		GameObject go = GameObject.Instantiate(buttonPrototype) as GameObject;
		go.transform.SetParent(scrollableArea.transform);
		go.transform.localPosition = buttonPrototype.transform.localPosition;
			go.transform.localRotation = buttonPrototype.transform.localRotation;
			go.transform.localScale = buttonPrototype.transform.localScale;
			go.SetActive(true);
		RectTransform rectTrans = go.transform as RectTransform;
		Vector3 pos = rectTrans.anchoredPosition;
		pos.y -= 50*entrySlot;
		rectTrans.anchoredPosition = pos;
		go.name = "Number"+entrySlot;
		DebugMenuEntry entry = go.GetComponent<DebugMenuEntry>();
		entry.entryUID = entrySlot;
		entryList.Add(entry);
		SetEntryContent(entry, labelName, valueName);
		SetContentBounds(pos.y-rectTrans.sizeDelta.y);
	}

	public void AddToggleEntry(int entrySlot, string labelName, string valueName)
	{
		GameObject go = GameObject.Instantiate(togglePrototype) as GameObject;
		go.transform.SetParent(scrollableArea.transform);
		go.transform.localPosition = togglePrototype.transform.localPosition;
			go.transform.localRotation = togglePrototype.transform.localRotation;
			go.transform.localScale = togglePrototype.transform.localScale;
			go.SetActive(true);
		RectTransform rectTrans = go.transform as RectTransform;
		Vector3 pos = rectTrans.anchoredPosition;
		pos.y -= 50*entrySlot;
		rectTrans.anchoredPosition = pos;
		go.name = "Number"+entrySlot;
		DebugMenuEntry entry = go.GetComponent<DebugMenuEntry>();
		entry.entryUID = entrySlot;
		entryList.Add(entry);
		SetEntryContent(entry, labelName, valueName);
		SetContentBounds(pos.y-rectTrans.sizeDelta.y);
	}

	public void SetEntryContent(DebugMenuEntry entry, string labelName, string valueName)
	{
		entry.contentLabel = labelName;
		entry.contentValue = valueName;
		entry.textLabel.text = entry.contentLabel;
		entry.textValue.text = entry.contentValue;
	}

	public void SetContentBounds(float maxY)
	{
		RectTransform rectTrans = scrollableArea.transform as RectTransform;
		Vector2 size = rectTrans.sizeDelta;
		size.y = Mathf.Abs(maxY)+15;
		rectTrans.sizeDelta = size;
	}

	public void PressEntryIncrease(DebugMenuEntry entry)
	{
		float value = 0;
		if (name.Length >= 2)
		{
			value = Convert.ToSingle(entry.contentValue);
		}
		if (entry.contentValue.IndexOf('.') > 0)
		{
			value += 0.1f;
		}
		else
		{
			value++;
		}
		EvalInputString(entry.contentLabel+"="+value.ToString());
		RefreshBindings(entry);
	}
	public void PressEntryDecrease(DebugMenuEntry entry)
	{
		float value = 0;
		if (name.Length >= 2)
		{
			value = Convert.ToSingle(entry.contentValue);
		}
		if (entry.contentValue.IndexOf('.') > 0)
		{
			value -= 0.1f;
		}
		else
		{
			value--;
		}
		EvalInputString(entry.contentLabel+"="+value.ToString());
		RefreshBindings(entry);
	}
	public void PressEntryToggle(DebugMenuEntry entry)
	{
		EvalInputString(entry.contentLabel);
		RefreshBindings(entry);
	}

	/*public void PressEntry(BaseEventData eventData)
	{
		if (eventData == null ||  eventData.selectedObject == null)
		{
			Debug.Log("ButtonPressed has invalid data ");
			return;
		}

		PointerEventData pointerData = eventData as PointerEventData;
		DebugMenuEntry entry = eventData.selectedObject.GetComponent<DebugMenuEntry>();
		if (pointerData.button == PointerEventData.InputButton.Left)
		{
			PressEntryIncrease(entry);
		}
		else if (pointerData.button == PointerEventData.InputButton.Right)	
		{
			PressEntryDecrease(entry);
		}
	}*/

	public void HoverEnter(GameObject go)
    {
    	if (!hoveredList.Contains(go))
    	{
    		hoveredList.Add(go);
    	}	
    	hovered = hoveredList.Count > 0;
    }
    public void HoverExit(GameObject go)
    {
    	if (hoveredList.Contains(go))
    	{
    		hoveredList.Remove(go);
    	}
    	hovered = hoveredList.Count > 0;
    }

    public void ToggleVisible()
    {
    	isVisible = !isVisible;
    	visibleScrollbar.value = isVisible?0:1;
    	RectTransform rectTrans = scrollableArea.transform.parent as RectTransform;
		Vector3 pos = rectTrans.anchoredPosition;
		pos.x = isVisible?0:-400;
		rectTrans.anchoredPosition = pos;
    }

	private bool EvalInputString( string inputString )
    {
		if( inputString == null || inputString.Trim().Length == 0 )
		{
			return false;
		}
		
		return false;
    }


}

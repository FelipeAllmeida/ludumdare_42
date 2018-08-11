using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class CSVDebug 
{
	private List<string> _listVariations;

	private List<string> _listSandboxGroup; 
	private List<double> _listSPointY;

	private static CSVDebug _instance;
	public static CSVDebug instance
	{
		get
		{
			if(_instance == null)
			{
				_instance = new CSVDebug();
			}
			return _instance;
		}
	}

	public CSVDebug()
	{
		_instance = this;

		_listVariations = new List<string>();
		_listSandboxGroup = new List<string>();
		_listSPointY = new List<double>();
	}


	public void SetVariations(string p_variations)
	{
		_listVariations.Add(p_variations);
	}

	public void VariationsCSVDebug()
	{
		if(!Application.isEditor) return;
		
		StreamWriter _file = new StreamWriter("../Debug/variations.csv");
		string __reg = "Step; Variations";
		_file.WriteLine(__reg);
		for(int i = 0; i < _listVariations.Count; i++)
		{
			__reg = i + "; ";
			__reg +=  _listVariations[i] + "; ";
			_file.WriteLine(__reg);
		}
		_file.Close();
		
		Debug.Log("CSV salvo!");
	}


	public void SetSandboxGroup(string p_sandboxGroup)
	{
		_listSandboxGroup.Add (p_sandboxGroup);
	}

	public void SetSPointY(double p_sPointY)
	{
		_listSPointY.Add (p_sPointY);
	}

	public void SandboxGroupCSVDebug()
	{
		if(!Application.isEditor) return;
		
		StreamWriter _file = new StreamWriter("../Debug/SandboxGroup.csv");
		string __reg = "Step; SandboxGroup; SPointY";
		_file.WriteLine(__reg);
		for(int i = 0; i < _listSandboxGroup.Count; i++)
		{
			__reg = i + "; ";
			__reg +=  _listSandboxGroup[i] + "; ";
			__reg +=  _listSPointY[i] + "; ";
			_file.WriteLine(__reg);
		}
		_file.Close();
		
		Debug.Log("CSV salvo!");
	}
}

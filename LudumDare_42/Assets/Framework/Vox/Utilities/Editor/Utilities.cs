using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace Vox.Utilities
{
	/// <summary>
	/// Class that stores several functions which may be useful in some situations.
	/// </summary>
	public sealed class Utilities
	{
		public static string[] ReturnFilesOnDirectory(string directory, bool scanSubfolders)
		{
			string[] __files;
			List<string> __filesList = new List<string>();
			
			//Get files in subfolders
			if (scanSubfolders)
			{
				string[] ___directoryies = Directory.GetDirectories(directory);
				foreach (string dir in ___directoryies)
				{
					string[] ____files = ReturnFilesOnDirectory(dir + "/", true);
					foreach (string file in ____files)
						__filesList.Add(file);
				}
			}
			
			//Get files in current folder
			string[] __filesInThisFolder = Directory.GetFiles(directory);
			foreach (string fileInThisFolder in __filesInThisFolder)
				__filesList.Add(fileInThisFolder);
			
			
			//Get files from files list
			__files = new string[__filesList.Count];
			for (int i = 0; i < __filesList.Count; i++)
				__files[i] = __filesList[i]; 
			
			return __files;
		}
		
		public static List<Vector3> ConvertFromFloatArrayToVector3List(float []p_array)
		{
			List<Vector3> __resultingList = new List<Vector3>();

			float __x, __y, __z;
			__x = __y = __z = 0f;
			for(int i = 0; i < p_array.Length; i++)
			{	
				if(i % 3 == 0)
				{
					__x = p_array[i];
				}
				else if(i % 3 == 1)
				{
					__y = p_array[i];
				}
				else if(i % 3 == 2)
				{
					__z = p_array[i];
					
					__resultingList.Add(new Vector3(__x, __y, __z));
				}
			}
			
			return __resultingList;
		}
		
		public static List<float> ConvertFromFloatArrayToFloatList(float[] p_array)
		{
			List<float> __resultingList = new List<float>();
			
			for(int i = 0; i < p_array.Length; i++)
			{
				__resultingList.Add(p_array[i]);
			}
			
			return __resultingList;
		}
		
		public static void CreateCurveFromValueTimeList(List<Vector3> p_valueList, List<float> p_timeList, AnimationCurve p_offsetX, AnimationCurve p_offsetY, AnimationCurve p_offsetZ)
		{
			for(int i = 0; i < p_valueList.Count; i++)
			{
				p_offsetX.AddKey(p_timeList[i], p_valueList[i].x);
				p_offsetY.AddKey(p_timeList[i], p_valueList[i].y);
				p_offsetZ.AddKey(p_timeList[i], p_valueList[i].z);
			}
		}
		
		public static float GetRelativeNewPosition(float p_value, float p_valMin, float p_valMax, float p_posMin, float p_posMax)
		{
			float __deltaVal = p_valMax - p_valMin;
			float __deltaPos = p_posMax - p_posMin;
			float __percent = (p_value - p_valMin) / __deltaVal;
			float __newPos = __percent * __deltaPos - p_posMax;
					
			return __newPos;
		}

		public static float GetRelativeNewPositionWithClamp(float p_value, float p_valMin, float p_valMax, float p_posMin, float p_posMax)
		{
			p_value = (p_value > p_valMax) ? (p_valMax) : (p_value);
			p_value = (p_value < p_valMin) ? (p_valMin) : (p_value);

			float __deltaVal = p_valMax - p_valMin;
			float __deltaPos = p_posMax - p_posMin;
			float __percent = (p_value - p_valMin) / __deltaVal;
			float __newPos = __percent * __deltaPos - p_posMax;
			
			return __newPos;
		}
		
		public static float GetMaxInList(List<float> p_list)
		{
			float __max = -9999999f;
			for(int i = 0; i < p_list.Count; i++)
			{
				__max = (p_list[i] > __max) ? (p_list[i]) : (__max);
			}
			return __max;
		}

		public static double GetMaxInList(List<double> p_list)
		{
			double __max = -9999999f;
			for(int i = 0; i < p_list.Count; i++)
			{
				__max = (p_list[i] > __max) ? (p_list[i]) : (__max);
			}
			return __max;
		}
		
		public static float GetMinInList(List<float> p_list)
		{
			float __min = 9999999f;
			for(int i = 0; i < p_list.Count; i++)
			{
				__min = (p_list[i] < __min) ? (p_list[i]) : (__min);
			}
			return __min;
		}

		public static double GetMinInList(List<double> p_list)
		{
			double __min = 9999999f;
			for(int i = 0; i < p_list.Count; i++)
			{
				__min = (p_list[i] < __min) ? (p_list[i]) : (__min);
			}
			return __min;
		}
		
		public static int InvertIndex(int p_index, int p_listSize)
		{
			int __newIndex = p_listSize - 1 - p_index;
			return __newIndex;
		}

		public static string FixDate(string p_date)
		{
			string p_year = "";
			for(int i = 2; i < 4; i++)
			{
				p_year += p_date[i];
			}

			p_date = p_date.Remove(0, 5);

			return p_date += "-" + p_year;
		}

		public static void SetLayerRecursively(GameObject p_parent, string p_layerName)
		{
			foreach (Transform trans in p_parent.GetComponentsInChildren<Transform>(true))
			{
				trans.gameObject.layer = LayerMask.NameToLayer(p_layerName);
			}
		}

		public static string WrapText(string input, int lineLength)
		{
			string[] __arrayWords = input.Split(" "[0]);
			string __result = "";
			string __line = "";
			
			foreach(string s in __arrayWords)
			{
				string __cache = __line + " " + s;
				
				if(__cache.Length > lineLength)
				{
					__result += __line + "\n";
					__line = s;
				}
				else 
				{
					__line = __cache;
				}

			}
			__result += __line;
			
			// Remove first " " char
			return __result.Substring(1,__result.Length-1);
		}
	}
}
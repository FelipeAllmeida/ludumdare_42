/*
 * Vers.: 1.2
 * Made by Ivan S. Cavalheiro (Unity 3D programmer)
 * This classe is part of Vox's Framework
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// A utilities contains useful utility functions.
/// </summary>
/// <remarks>
/// There many uses for this class, and you can use it at will.
/// </remarks>
using System.Net;

public sealed class AUtilities
{
	/// <summary>
	/// Enable all given colliders.
	/// </summary>
	/// <remarks>
	/// You can use this function passing an array of colliders collected in a collision or by inspector and enable all of them.
	/// </remarks>
	/// <code>
	/// Enemy enemy = Enemy.SpawnNewEnemy();
	/// Collider[] enemyColliders = enemy.GetColliders();
	/// EnableColliders(enemyColliders);
	/// </code>
	public static void EnableColliders(Collider[] p_colliders)
	{
		foreach(var co in p_colliders)
		{
			co.enabled = true;
		}
	}
	

    public static void SetTargetFrameRate(int p_frameRate)
    {
        Application.targetFrameRate = p_frameRate;
    }

	/// <summary>
	/// Disable all given colliders.
	/// </summary>
	public static void DisableColliders(Collider[] p_colliders)
	{
		foreach(var co in p_colliders)
		{
			co.enabled = false;
		}
	}
	
	/// <summary>
	/// Gets a random object from the given list.
	/// </summary>
	/// <returns>
	/// The randomized selected object from given list.
	/// </returns>
	/// <param name='p_list'>
	/// The list with the objects. It can be an array([]) or a list (List<>).
	/// </param>
	/// <remarks>
	/// In this example we will get a list of itens that the player can win at end of the game, and we gonna get a random item to give him.<BR>
	/// <code>
	/// public List<BonusItem> bonusItensForEndGame;
	/// public Player player;
	/// 
	/// public void OnEndGame(bool p_playerWon)
	/// {
	/// 	if(p_playerWon == false)
	/// 	{
	/// 		Debug.Log("Owwwwh, you loose sire, nothing for you!");
	/// 	}
	/// 	else
	/// 	{
	/// 		//So the player won the game, now we gonna select a item for him in the list.
	/// 		BonusItem __itemToGiveToThePlayer = AUtilities.GetRandom<BonusItem>(bonusItensForEndGame);
	/// 		player.GiveItem(__itemToGiveToThePlayer);
	/// 	}
	/// }
	/// </code>
	/// <BR>
	/// See how fast it is to select a random item from a list with AUtilities.GetRandom method.<BR>
	/// </remarks>
	public static T GetRandom<T>(T[] p_list)
	{
		int random = UnityEngine.Random.Range(0, p_list.Length);
		return p_list[random];
	}
	public static T GetRandom<T>(List<T> p_list)
	{
		int random = UnityEngine.Random.Range(0, p_list.Count);
		return p_list[random];
	}
	
	public static float[] ConvertFromVector3ListToFloatArray(List<Vector3> p_list)
	{
		float[] __resultingArray = new float[p_list.Count * 3];
		
		for(int i = 0; i < p_list.Count; i++)
		{
			__resultingArray[i * 3] = p_list[i].x;
			__resultingArray[(i * 3) + 1] = p_list[i].y;
			__resultingArray[(i * 3) + 2] = p_list[i].z;
		}
		
		return __resultingArray;
	}
	
	public static float[] ConvertFromFloatListToFloatArray(List<float> p_list)
	{
		float[] __reslutingArray =  new float[p_list.Count];
		
		for(int i = 0; i < p_list.Count; i++)
		{
			__reslutingArray[i] = p_list[i];
		}
		
		return __reslutingArray;
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
//		Debug.Log ("Without Clamp");

		float __deltaVal = p_valMax - p_valMin;
		float __deltaPos = p_posMax - p_posMin;
		float __percent = (p_value - p_valMin) / __deltaVal;
		float __newPos = __percent * __deltaPos - p_posMax;
				
		return __newPos;
	}

	public static float GetRelativeNewPositionWithClamp(float p_value, float p_valMin, float p_valMax, float p_posMin, float p_posMax)
	{
//		Debug.Log ("With Clamp");

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
        p_parent.layer = LayerMask.NameToLayer(p_layerName);

        foreach (Transform trans in p_parent.GetComponentsInChildren<Transform>(true))
		{
			trans.gameObject.layer = LayerMask.NameToLayer(p_layerName);
		}
	}

	public static bool VerifyInternetConnection()
	{
		bool __isInternetConnected = false;
		try
		{
			Dns.GetHostEntry("www.google.com");
			__isInternetConnected = true; 
		}
		catch 
		{
			__isInternetConnected = false; 
		}
		
		return __isInternetConnected;
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

//	public static string WrapText(string p_text, float p_sizeLimit)
//	{
//		string builder = "";
//		float rowLimit = 10f;
//		string[] parts = p_text.Split(' ');
//		for (int i = 0; i < parts.Length; i++)
//		{
//			Debug.Log(parts[i]);
//			TextMesh.text += parts[i] + " ";
//			if (TextMesh.renderer.bounds.extents.x > rowLimit)
//			{
//				TextMesh.text = builder.TrimEnd() + System.Environment.NewLine + parts[i] + " ";
//			}
//			builder = TextMesh.text;
//		}
//	}
}

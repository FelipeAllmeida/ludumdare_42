using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ParticleScaler : EditorWindow 
{
	[MenuItem ("AFramework/Particle scaler")]
	public static void ParticleScalerMethod()
	{
		EditorWindow.GetWindow(typeof(ParticleScaler)).Show();
	}
	
	private float _multiplier = 1;
	private SerializedProperty _particlesReference;
	public List<ParticleSystem> particles = new List<ParticleSystem>();
	
	void OnGUI()
	{
		if(GUILayout.Button("Apply"))
		{
			for(int i = 0; i < particles.Count; i++)
			{
				ParticleSystem __cache = particles[i];				
				if(__cache == null)	continue;

				var __main = __cache.main;
				{//Process particle
					__main.startLifetimeMultiplier *= _multiplier;
					__main.startSpeedMultiplier *= _multiplier;
					__main.startSizeMultiplier *= _multiplier;
				}
				
				particles[i] = __cache;
				
			}
			
			//particles = new List<ParticleSystem>();
			Debug.Log("Foi " + particles.Count);
		}
		
		_multiplier = EditorGUILayout.FloatField("Multiplier:",_multiplier);
		
		/*
		if(_particlesReference == null)
		{
			var __referenceCache = new SerializedObject(this);
			_particlesReference = __referenceCache.FindProperty("particles");
		}
		
		EditorGUILayout.PropertyField(_particlesReference, true);
		*/
	}
	
	void Update()
	{
		
		particles = new List<ParticleSystem>();
		foreach(var obj in Selection.gameObjects)
		{
			ParticleSystem __cache = obj.GetComponent<ParticleSystem>();
			if(__cache != null)
			{
				particles.Add(__cache);
			}
		}
		
	}
}

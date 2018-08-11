#define SKIP_CODE_IN_DOCUMENTATION

using UnityEngine;
using System;
using System.Collections;
using VoxInternal;

namespace Vox
{
	/// <summary>
	/// Class used to create TweenNode and interpolate values.
	/// </summary>
	/// <remarks>
	/// The tween class is used to call functions that creates Tween with the paramaters specified and types.
	/// 
	/// Tweens have a lot of uses, such as making HUD animations, translating objects, and anything which depends on a value change.
	/// 
	/// Code example:
	/// 
	/// <code>
	/// 
	/// Vox.TweenNode __showSpriteNode = Vox.Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_alpha)
	/// {
	/// 	Color __newColor = p_renderer.color;
	/// 	__newColor.a = p_alpha;
	/// 	p_renderer.color = __newColor;
	/// });
	/// 
	/// __showSpriteNode.onFinish += delegate()
	/// {
	/// 	Vox.TweenNode __changeSpriteColor = Vox.Tween.ColorTo(Color.white, Color.red, 1f, EaseType.Linear, delegate(Color p_color) 
	/// 	{
	/// 		p_renderer.color = p_color;	
	/// 	}
	/// };
	/// 
	/// </code>
	///  
	/// </remarks>
	public class Tween 
	{
		#region Private Internal Only

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		/// <summary>
		/// Transition float from startValue to finalValue with passed parameters using Ease Functions.
		/// </summary>
		private static TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration,  EaseType p_easeType, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode, Action<float> p_callbackUpdate)
		{
			TweenNode __node = null;

			Action __actionOnUpdate = delegate
			{
				if (__node.currentState == NodeState.PLAYING)
				{
					float ___counter = __node.currentTimeStep;

					float __currentValue;

					if (___counter < p_duration)
					{
						___counter += p_useTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;

						float __normalizedTime = Mathf.Min(___counter / p_duration, 1f);

						__currentValue = EaseFunctionMaths.GetTransaction(p_startValue, p_finalValue, __normalizedTime, p_easeType);

						p_callbackUpdate(__currentValue);

						__node.currentTimeStep = ___counter;
					}
					else
					{
						if (__node.loopMode == TweenLoopType.NONE)
						{
							if(__node.onFinish != null)
								__node.onFinish();

						    __node.Cancel( );
                        }
                        else
						{
							if(__node.onFinish != null)	__node.onFinish();

							if(__node.loopMode == TweenLoopType.PING_PONG)
							{
								__node.isPingPongGoingBack = __node.isPingPongGoingBack == true ? false : true;

								float __finalValue = p_finalValue;
								p_finalValue = p_startValue;
								p_startValue = __finalValue;
							}

							__node.currentTimeStep = 0;
						}
					}
				}
			};

			Action __actionOnAnticipate = delegate
			{
				float __normalizedTime = 1f;
				float __finalValue = EaseFunctionMaths.GetTransaction(p_startValue, p_finalValue, __normalizedTime, p_easeType);

				p_callbackUpdate(__finalValue);

				if(__node.onFinish != null)	
					__node.onFinish();

			    __node.Cancel();
			};

			__node = new TweenNode(p_startValue, p_finalValue, p_duration, p_loopMode, p_updateMode, p_useTimeScale, TweenModule.instance.GetNextNodeID(), p_callbackUpdate, __actionOnUpdate, __actionOnAnticipate);

			TweenModule.instance.AddNode(__node, p_updateMode);

			return __node;
		}

		/// <summary>
		/// Transition float from startValue to finalValue with passed parameters using unity animation curves.
		/// </summary>
		private static TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration, AnimationCurve p_normalizedAnimationCurve, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode, Action<float> p_callbackUpdate)
		{
			TweenNode __node = null;

			Action __actionOnUpdate = delegate
			{
				if (__node.currentState == NodeState.PLAYING)
				{
					float ___counter = __node.currentTimeStep;

					float __currentValue = 0;

					if (___counter < p_duration)
					{
						if(p_useTimeScale)
						{
							___counter += Time.deltaTime;
						}
						else
						{
							___counter += Time.unscaledDeltaTime;
						}

						float __normalizedTime = Mathf.Min(___counter / p_duration, 1f);

						if(p_normalizedAnimationCurve == null)
						{
							p_normalizedAnimationCurve = new AnimationCurve();
						}

						float __curveValue = p_normalizedAnimationCurve.Evaluate(__normalizedTime);
						float __normalizedCurveValue = __curveValue/1;

						__currentValue = p_startValue + ((p_finalValue-p_startValue)*__normalizedCurveValue/1);

						p_callbackUpdate(__currentValue);

						__node.currentTimeStep = ___counter;
					}
					else
					{
						if (__node.loopMode == TweenLoopType.NONE)
						{
							if(__node.onFinish != null)
								__node.onFinish();

						    __node.Cancel( );
                        }
                        else
						{
							if(__node.onFinish != null)
								__node.onFinish();

							if(__node.loopMode == TweenLoopType.PING_PONG)
							{
								if(__node.isPingPongGoingBack == true)
								{
									__node.isPingPongGoingBack = false;
								}
								else
								{
									__node.isPingPongGoingBack = true;
								}

								float __finalValue = p_finalValue;
								p_finalValue = p_startValue;
								p_startValue = __finalValue;
							}

							__node.currentTimeStep = 0;
						}
					}
				}
			};

			Action __actionOnAnticipate = delegate() 
			{				
				float __finalValue = 0;

				__finalValue = p_normalizedAnimationCurve.Evaluate(1);

				p_callbackUpdate(__finalValue);

				if(__node.onFinish != null)
					__node.onFinish();

			    __node.Cancel( );
			};

			__node = new TweenNode(p_startValue, p_finalValue, p_duration, p_loopMode, p_updateMode, p_useTimeScale, TweenModule.instance.GetNextNodeID(), p_callbackUpdate, __actionOnUpdate, __actionOnAnticipate);

			TweenModule.instance.AddNode(__node, p_updateMode);

			return __node;
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#region FloatTo
		/// <summary>
		/// <STRONG>(8 overloads)</STRONG> Transition float from startValue to finalValue with passed parameters using Ease Functions.
		/// </summary>
		/// <remarks>
		/// This function interpolates a startValue to a finalValue with the passed parameters, returning in its callback the steps of the interpolation.
		/// 
		/// <STRONG><em>TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration, EaseType p_easeType, Action<float> p_callbackUpdate)</em></STRONG>
		///   
		/// Here is a code example, which uses a tween to fade a sprite renderer from full color to transparent in 1 second. Deacticating the object when finished.
		/// 
		/// <code>
		/// public void FadeSprite(SpriteRenderer p_renderer)
		/// {
		/// 	Vox.TweenNode __spriteFadeNode = Vox.Tween.FloatTo(1f, 0f, 1f, EaseType.Linear, delegate(float p_alpha)
		/// 	{
		/// 		Color __newColor = p_renderer.color;
		/// 		__newColor.a = p_alpha;
		/// 		p_renderer.color = __newColor;
		/// 	});
		/// 
		/// 	__spriteFadeNode.onFinish += delegate()
		/// 	{
		/// 		p_renderer.gameobject.SetActive(false);
		/// 	};
		/// }
		/// 
		/// </code>
		/// 
		/// <STRONG><em>TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<float> p_callbackUpdate)</em></STRONG>
		/// 
		/// The useTimeScale property defines if the tween update considers or not the unity Time.timescale property. Usefull for ignoring pauses and slowmotion effects.
		///  
		/// <code>
		/// public void PauseGameAndDisappearHUDimage(Image p_hudImage)
		/// {
		/// 	Time.timeScale = 0f;
		/// 
		/// 	Vox.TweenNode __dissapearHUDImageNode = Vox.Tween.FloatTo(1f, 0f, 1f, EaseType.Linear, false, delegate(float p_alpha)
		/// 	{
		/// 		Color __newColor = p_hudImage.color;
		/// 		__newColor.a = p_alpha;
		/// 		p_hudImage.color = __newColor;
		/// 	});
		/// }
		/// </code>
		/// 
		/// <STRONG><em>TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<float> p_callbackUpdate)</em></STRONG>
		/// 
		/// <code>
		/// public void FlashHUDImage(Image p_hudImage)
		/// {
		/// 	Vox.TweenNode __flashHUDImageNode = Vox.Tween.FloatTo(1f, 0f, 1f, EaseType.Linear, true, TweenLoopMode.PING_PONG, delegate(float p_alpha)
		/// 	{
		/// 		Color __newColor = p_hudImage.color;
		/// 		__newColor.a = p_alpha;
		/// 		p_hudImage.color = __newColor;
		/// 	});
		/// }
		/// </code>  
		/// 
		/// <STRONG><em>TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, TweenLoopType p_loopMode, UpdateTimeType p_updateMode, Action<float> p_callbackUpdate)</em></STRONG>
		/// 
		/// Here is a code example, which uses a tween to apply a physics force which increases in a cubic pattern.
		/// The p_updateMode defines if the tween will update using Update, LateUpdate, FixedUpdate or OnGUI.
		/// 
		/// <code>
		/// public void ApplyForceTween(Rigidbody p_rigidbody)
		/// { 
		/// 	Vox.TweenNode __applyForceToObjectNode = Vox.Tween.FloatTo(1f, 0f, 1f, EaseType.Linear, true, TweenLoopMode.PING_PONG, UpdateTimeType.FIXED_UPDATE, delegate(float p_value)
		/// 	{
		/// 		p_rigidbody.AddForce(Vector3.up * p_value)
		/// 	});
		/// }
		/// </code>
		/// 
		/// <STRONG><em>TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration, AnimationCurve p_normalizedAnimationCurve, Action<float> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration,  AnimationCurve p_normalizedAnimationCurve,  bool p_useTimeScale, Action<float> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration,  AnimationCurve p_normalizedAnimationCurve, bool p_useTimeScale, TweenLoopType p_loopMode, Action<float> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration,  AnimationCurve p_normalizedAnimationCurve, bool p_useTimeScale, TweenLoopType p_loopMode, UpdateTimeType p_updateMode, Action<float> p_callbackUpdate)</em></STRONG>
		/// 
		/// You can also use <A href="http://docs.unity3d.com/Manual/EditingCurves.html"><STRONG>Unity animation curves</STRONG></A>  instead of easing functions.
		/// 
		/// <code>
		/// public void FlashHUDImage(Image p_hudImage, AnimationCurve p_animationCurve)
		/// {
		/// 	Vox.TweenNode __flashHUDImageNode = Vox.Tween.FloatTo(1f, 0f, 1f, p_animationCurve, true, TweenLoopMode.PING_PONG, delegate(float p_alpha)
		/// 	{
		/// 		Color __newColor = p_hudImage.color;
		/// 		__newColor.a = p_alpha;
		/// 		p_hudImage.color = __newColor;
		/// 	});
		/// }
		/// </code>  
		/// 
		/// </remarks>
		public static TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration, EaseType p_easeType, Action<float> p_callbackUpdate)
		{
			return FloatTo(p_startValue, p_finalValue, p_duration, p_easeType, true,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<float> p_callbackUpdate)
		{
			return FloatTo(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}
			
		public static TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<float> p_callbackUpdate)
		{
			return FloatTo(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}
			
		public static TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, TweenLoopType p_loopMode, UpdateTimeType p_updateMode, Action<float> p_callbackUpdate)
		{
			return FloatTo(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale, p_updateMode, p_loopMode, p_callbackUpdate);
		}

		public static TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration, AnimationCurve p_normalizedAnimationCurve, Action<float> p_callbackUpdate)
		{
			return FloatTo(p_startValue, p_finalValue, p_duration, p_normalizedAnimationCurve, true, UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}
			
		public static TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration,  AnimationCurve p_normalizedAnimationCurve,  bool p_useTimeScale, Action<float> p_callbackUpdate)
		{
			return FloatTo(p_startValue, p_finalValue, p_duration, p_normalizedAnimationCurve, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}
			
		public static TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration,  AnimationCurve p_normalizedAnimationCurve, bool p_useTimeScale, TweenLoopType p_loopMode, Action<float> p_callbackUpdate)
		{
			return FloatTo(p_startValue, p_finalValue, p_duration, p_normalizedAnimationCurve, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}
			
		public static TweenNode FloatTo(float p_startValue, float p_finalValue, float p_duration,  AnimationCurve p_normalizedAnimationCurve, bool p_useTimeScale, TweenLoopType p_loopMode, UpdateTimeType p_updateMode, Action<float> p_callbackUpdate)
		{
			return FloatTo(p_startValue, p_finalValue, p_duration, p_normalizedAnimationCurve, p_useTimeScale, p_updateMode, p_loopMode, p_callbackUpdate);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion
		
		#region Vector3To
		/// <summary>
		/// <STRONG>(8 overloads)</STRONG> Transition Vector3 from startValue to finalValue with passed parameters.
		/// </summary>
		/// <remarks>
		/// This function interpolates a startValue to a finalValue with the passed parameters, returning in its callback the steps of the interpolation. 
		///   
		/// Here is a code example, which uses a tween to move a gameobject from a location to another.
		/// 
		/// <STRONG><em>TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, EaseType p_easeType, Action<Vector3> p_callbackUpdate)</em></STRONG>
		/// 
		/// <code>
		/// public void MoveObjectToPosition(GameObject p_object, Vector3 p_position)
		/// {
		/// 	Vector3 __startPosition = p_object.transform.position;
		/// 
		/// 	Vox.TweenNode __moveObjectNode = Vox.Tween.Vector3To(__startPosition, p_position, 1f, EaseType.Linear, delegate(Vector3 p_position)
		/// 	{
		///			p_object.transform.position = __startPosition;
		/// 	});
		/// 
		/// 	__moveObjectNode.onFinish += delegate()
		/// 	{
		/// 		Debug.Log("Object Reached position");
		/// 	};
		/// }
		/// </code>
		/// 
		/// <STRONG><em>TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Vector3> p_callbackUpdate)</em></STRONG>
		/// 
		/// Here is a code example, which uses a tween to move a gameobject from a location to another ignoring pause state by changing the useTimeScale parameter to false.
		/// 
		/// <code>
		/// public void MoveObjectToPosition(GameObject p_object, Vector3 p_position)
		/// {
		/// 	Vector3 __startPosition = p_object.transform.position;
		/// 
		/// 	Vox.TweenNode __moveObjectNode = Vox.Tween.Vector3To(__startPosition, p_position, 1f, EaseType.Linear, false, delegate(Vector3 p_position)
		/// 	{
		///			p_object.transform.position = __startPosition;
		/// 	});
		/// 
		/// 	__moveObjectNode.onFinish += delegate()
		/// 	{
		/// 		Debug.Log("Object Reached position");
		/// 	};
		/// }
		/// </code>
		/// 
		/// <STRONG><em>TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, TweenLoopType p_loopMode, Action<Vector3> p_callbackUpdate)</em></STRONG>
		/// 
		/// Here is a code example, which uses a tween to move a gameobject from a location to another in a ping pong loop.
		/// 
		/// <code>
		/// public void MoveObjectToPositionInLoop(GameObject p_object, Vector3 p_position)
		/// {
		/// 	Vector3 __startPosition = p_object.transform.position;
		/// 
		/// 	Vox.TweenNode __moveObjectNode = Vox.Tween.Vector3To(__startPosition, p_position, 1f, EaseType.Linear, true, TweenLoopMode.PING_PONG, delegate(Vector3 p_position)
		/// 	{
		///			p_object.transform.position = __startPosition;
		/// 	});
		/// 
		/// 	__moveObjectNode.onFinish += delegate()
		/// 	{
		/// 		Debug.Log("Object Going Back in loop: " + __moveObject.isPingPongGoingBack);
		/// 	};
		/// }
		/// </code>
		/// 
		/// <STRONG><em>TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, TweenLoopType p_loopMode, UpdateTimeType p_updateMode,  Action<Vector3> p_callbackUpdate)</em></STRONG>
		/// 
		/// Here is a code example, which apply a start force to an end force linearly in an object, reversing that force in a loop.
		/// 
		/// <code>
		/// public void ApplyForceToObjectInLoop(Rigidbody p_body, Vector3 p_startForce, Vector3 p_endForce)
		/// {
		/// 	Vox.TweenNode __forceTween = null;
		///  
		/// 	__forceTweenNode = Vox.Tween.Vector3To(__startPosition, p_position, 1f, EaseType.Linear, true, TweenLoopMode.PING_PONG, UpdateTimeType.FIXED_UPDATE, delegate(Vector3 p_force)
		/// 	{
		/// 		if(__forceTween.isPingPongGoingBack)
		///				p_body.AddForce(-p_force);
		/// 		else 
		///				p_body.AddForce(p_force);
		/// 	});
		/// }
		/// </code>  
		///
		/// <STRONG><em>TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Vector3> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Vector3> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Vector3> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Vector3> p_callbackUpdate)</em></STRONG>
		/// 
		/// You can also use <A href="http://docs.unity3d.com/Manual/EditingCurves.html"><STRONG>Unity animation curves</STRONG></A>  instead of easing functions.
		/// 
		/// <code>
		/// public void MoveObjectToPosition(GameObject p_object, Vector3 p_position, AnimationCurve p_curve)
		/// {
		/// 	Vector3 __startPosition = p_object.transform.position;
		/// 
		/// 	Vox.TweenNode __moveObjectNode = Vox.Tween.Vector3To(__startPosition, p_position, 1f, p_curve, delegate(Vector3 p_position)
		/// 	{
		///			p_object.transform.position = __startPosition;
		/// 	});
		/// 
		/// 	__moveObjectNode.onFinish += delegate()
		/// 	{
		/// 		Debug.Log("Object Reached position");
		/// 	};
		/// }
		/// </code>
		///   
		/// </remarks>
		public static TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, EaseType p_easeType, Action<Vector3> p_callbackUpdate)
		{
			return Vector3To(p_startValue, p_finalValue, p_duration, p_easeType, true, TweenLoopType.NONE,  UpdateTimeType.UPDATE, p_callbackUpdate);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Vector3> p_callbackUpdate)
		{
			return Vector3To(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale, TweenLoopType.NONE, UpdateTimeType.UPDATE, p_callbackUpdate);
		}
			
		public static TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, TweenLoopType p_loopMode, Action<Vector3> p_callbackUpdate)
		{
			return Vector3To(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  p_loopMode, UpdateTimeType.UPDATE, p_callbackUpdate);
		}
			
		public static TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, TweenLoopType p_loopMode, UpdateTimeType p_updateMode,  Action<Vector3> p_callbackUpdate)
		{
			TweenNode __temp = FloatTo(0, 1, p_duration, p_easeType, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
			{
				p_callbackUpdate(Vector3.Lerp(p_startValue, p_finalValue, newFloat));
			});

			return __temp;
		}
			
		public static TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Vector3> p_callbackUpdate)
		{
			return Vector3To(p_startValue, p_finalValue, p_duration, p_normalizedCurve, true,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Vector3> p_callbackUpdate)
		{
			return Vector3To(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}
			
		public static TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Vector3> p_callbackUpdate)
		{
			return Vector3To(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}
			
		public static TweenNode Vector3To(Vector3 p_startValue, Vector3 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Vector3> p_callbackUpdate)
		{
			TweenNode __temp = FloatTo(0, 1, p_duration, p_normalizedCurve, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
			{
				p_callbackUpdate(Vector3.Lerp(p_startValue, p_finalValue, newFloat));
			});

			return __temp;
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion
		
		#region QuaternionTo
		/// <summary>
		/// <STRONG>(8 overloads)</STRONG> Transition Quaternion from startValue to finalValue with passed parameters.
		/// </summary>
		/// <remarks>
		/// This function interpolates a startValue to a finalValue with the passed parameters, returning in its callback the steps of the interpolation.
		///
		/// Overrides availables:
		///
		/// <STRONG><em>TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, EaseType p_easeType, Action<Quaternion> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Quaternion> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Quaternion> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Quaternion> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Quaternion> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Quaternion> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Quaternion> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Quaternion> p_callbackUpdate)</em></STRONG>
		/// 
		/// For similar examples of usage see Tween.Vector3To or Tween.FloatTo functions.
		/// </remarks> 
		public static TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, EaseType p_easeType, Action<Quaternion> p_callbackUpdate)
		{
			return QuaternionTo(p_startValue, p_finalValue, p_duration, p_easeType, true, UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Quaternion> p_callbackUpdate)
		{
			return QuaternionTo(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Quaternion> p_callbackUpdate)
		{
			return QuaternionTo(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}
			
		public static TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Quaternion> p_callbackUpdate)
		{
			TweenNode __temp = FloatTo(0, 1, p_duration, p_easeType, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
			{
				p_callbackUpdate(Quaternion.Lerp(p_startValue, p_finalValue, newFloat));
			});

			return __temp;
		}

		public static TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Quaternion> p_callbackUpdate)
		{
			return QuaternionTo(p_startValue, p_finalValue, p_duration, p_normalizedCurve, true,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}
			
		public static TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Quaternion> p_callbackUpdate)
		{
			return QuaternionTo(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Quaternion> p_callbackUpdate)
		{
			return QuaternionTo(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}

		public static TweenNode QuaternionTo(Quaternion p_startValue, Quaternion p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Quaternion> p_callbackUpdate)
		{
			TweenNode __temp = FloatTo(0, 1, p_duration, p_normalizedCurve, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
			{
				p_callbackUpdate(Quaternion.Lerp(p_startValue, p_finalValue, newFloat));
			});

			return __temp;
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion
		
		#region ColorTo
		/// <summary>
		/// <STRONG>(8 overloads)</STRONG> Transition Color from startValue to finalValue with passed parameters.
		/// </summary>
		/// <remarks>
		/// This function interpolates a startValue to a finalValue with the passed parameters, returning in its callback the steps of the interpolation.
		/// 
		/// Overrides availables:
		///
		/// <STRONG><em>TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType, Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// For similar examples of usage see Tween.Vector3To or Tween.FloatTo functions.
		/// </remarks> 
		public static TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType, Action<Color> p_callbackUpdate)
		{
			return ColorTo(p_startValue, p_finalValue, p_duration, p_easeType, true,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Color> p_callbackUpdate)
		{
			return ColorTo(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}
			
		public static TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Color> p_callbackUpdate)
		{
			return ColorTo(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}
			
		public static TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Color> p_callbackUpdate)
		{
			TweenNode __temp = FloatTo(0, 1, p_duration, p_easeType, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
			{
				p_callbackUpdate(Color.Lerp(p_startValue, p_finalValue, newFloat));
			});

			return __temp;
		}

		public static TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Color> p_callbackUpdate)
		{
			return ColorTo(p_startValue, p_finalValue, p_duration, p_normalizedCurve, true,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Color> p_callbackUpdate)
		{
			return ColorTo(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}
			
		public static TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Color> p_callbackUpdate)
		{
			return ColorTo(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}
			
		public static TweenNode ColorTo(Color p_startValue, Color p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Color> p_callbackUpdate)
		{
			TweenNode __temp = FloatTo(0, 1, p_duration, p_normalizedCurve, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
				{
					p_callbackUpdate(Color.Lerp(p_startValue, p_finalValue, newFloat));
				});

			return __temp;
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion
		
		#region RectoTo
		/// <summary>
		/// <STRONG>(8 overloads)</STRONG> Transition Rect from startValue to finalValue with passed parameters.
		/// </summary>
		/// <remarks>
		/// This function interpolates a startValue to a finalValue with the passed parameters, returning in its callback the steps of the interpolation.
		/// 
		/// Overrides availables:
		///
		/// <STRONG><em>TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, EaseType p_easeType, Action<Rect> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Rect> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Rect> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Rect> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Rect> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Rect> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, TweenLoopType p_loopMode, Action<Rect> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Rect> p_callbackUpdate)</em></STRONG>
		/// 
		/// For similar examples of usage see Tween.Vector3To or Tween.FloatTo functions.
		/// </remarks> 
		public static TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, EaseType p_easeType, Action<Rect> p_callbackUpdate)
		{
			return RectTo(p_startValue, p_finalValue, p_duration, p_easeType, true,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}
			
		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Rect> p_callbackUpdate)
		{
			return RectTo(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Rect> p_callbackUpdate)
		{
			return RectTo(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}

		public static TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, EaseType p_easeType,  bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Rect> p_callbackUpdate)
		{
			TweenNode __temp = Tween.FloatTo(0, 1, p_duration, p_easeType, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
			{
				p_callbackUpdate(new Rect(
					Mathf.Lerp(p_startValue.x, p_finalValue.x, newFloat),
					Mathf.Lerp(p_startValue.y, p_finalValue.y, newFloat),
					Mathf.Lerp(p_startValue.width, p_finalValue.width, newFloat),
					Mathf.Lerp(p_startValue.height, p_finalValue.height, newFloat)
				));
			});

			return __temp;
		}

		public static TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Rect> p_callbackUpdate)
		{
			return RectTo(p_startValue, p_finalValue, p_duration, p_normalizedCurve, true,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Rect> p_callbackUpdate)
		{
			return RectTo(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, TweenLoopType p_loopMode, Action<Rect> p_callbackUpdate)
		{
			return RectTo(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}

		public static TweenNode RectTo(Rect p_startValue, Rect p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Rect> p_callbackUpdate)
		{
			TweenNode __temp = Tween.FloatTo(0, 1, p_duration, p_normalizedCurve, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
			{
				p_callbackUpdate(new Rect(
					Mathf.Lerp(p_startValue.x, p_finalValue.x, newFloat),
					Mathf.Lerp(p_startValue.y, p_finalValue.y, newFloat),
					Mathf.Lerp(p_startValue.width, p_finalValue.width, newFloat),
					Mathf.Lerp(p_startValue.height, p_finalValue.height, newFloat)
				));
			});

			return __temp;
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion
		
		#region Vector2
		/// <summary>
		/// <STRONG>(8 overloads)</STRONG> Transition Vector2 from startValue to finalValue with passed parameters.
		/// </summary>
		/// <remarks>
		/// This function interpolates a startValue to a finalValue with the passed parameters, returning in its callback the steps of the interpolation.
		/// 
		/// Overrides availables:
		///
		/// <STRONG><em>TweenNode Vector2To(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType, Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector2To(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector2To(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, TweenLoopType p_loopMode, Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector2To(Color p_startValue, Color p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Color> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Vector2> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Vector2> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Vector2> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Vector2> p_callbackUpdate)</em></STRONG>
		/// 
		/// For similar examples of usage see Tween.Vector3To or Tween.FloatTo functions.
		/// </remarks> 
		public static TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, EaseType p_easeType, Action<Vector2> p_callbackUpdate)
		{
			return Vector2To(p_startValue, p_finalValue, p_duration, p_easeType, true,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Vector2> p_callbackUpdate)
		{
			return Vector2To(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, TweenLoopType p_loopMode, Action<Vector2> p_callbackUpdate)
		{
			return Vector2To(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}
			
		public static TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Vector2> p_callbackUpdate)
		{
			TweenNode __temp = FloatTo(0, 1, p_duration, p_easeType, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
			{
				p_callbackUpdate(Vector2.Lerp(p_startValue, p_finalValue, newFloat));
			});

			return __temp;
		}
			
		public static TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Vector2> p_callbackUpdate)
		{
			return Vector2To(p_startValue, p_finalValue, p_duration, p_normalizedCurve, true,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Vector2> p_callbackUpdate)
		{
			return Vector2To(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Vector2> p_callbackUpdate)
		{
			return Vector2To(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}

		public static TweenNode Vector2To(Vector2 p_startValue, Vector2 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Vector2> p_callbackUpdate)
		{
			TweenNode __temp = FloatTo(0, 1, p_duration, p_normalizedCurve, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
			{
				p_callbackUpdate(Vector2.Lerp(p_startValue, p_finalValue, newFloat));
			});

			return __temp;
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion
		
		#region Vector4
		/// <summary>
		/// <STRONG>(8 overloads)</STRONG> Transition Vector4 from startValue to finalValue with passed parameters.
		/// </summary>
		/// <remarks>
		/// This function interpolates a startValue to a finalValue with the passed parameters, returning in its callback the steps of the interpolation.
		/// 
		/// Overrides availables:
		///
		/// <STRONG><em>TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, EaseType p_easeType, Action<Vector4> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Vector4> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, TweenLoopType p_loopMode, Action<Vector4> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Vector4> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Vector4> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Vector4> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Vector4> p_callbackUpdate)</em></STRONG>
		/// 
		/// <STRONG><em>TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Vector4> p_callbackUpdate)</em></STRONG>
		/// 
		/// For similar examples of usage see Tween.Vector3To or Tween.FloatTo functions.
		/// 
		/// </remarks> 
		public static TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, EaseType p_easeType, Action<Vector4> p_callbackUpdate)
		{
			return Vector4To(p_startValue, p_finalValue, p_duration, p_easeType, true,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @cond
		#endif

		public static TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, Action<Vector4> p_callbackUpdate)
		{
			return Vector4To(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, TweenLoopType p_loopMode, Action<Vector4> p_callbackUpdate)
		{
			return Vector4To(p_startValue, p_finalValue, p_duration, p_easeType, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}

		public static TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, EaseType p_easeType, bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Vector4> p_callbackUpdate)
		{
			TweenNode __temp = FloatTo(0, 1, p_duration, p_easeType, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
				{
					p_callbackUpdate(Vector4.Lerp(p_startValue, p_finalValue, newFloat));
				});

			return __temp;
		}

		public static TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, Action<Vector4> p_callbackUpdate)
		{
			return Vector4To(p_startValue, p_finalValue, p_duration, p_normalizedCurve, true,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}

		public static TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve, bool p_useTimeScale, Action<Vector4> p_callbackUpdate)
		{
			return Vector4To(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, TweenLoopType.NONE, p_callbackUpdate);
		}
			
		public static TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, TweenLoopType p_loopMode, Action<Vector4> p_callbackUpdate)
		{
			return Vector4To(p_startValue, p_finalValue, p_duration, p_normalizedCurve, p_useTimeScale,  UpdateTimeType.UPDATE, p_loopMode, p_callbackUpdate);
		}

		public static TweenNode Vector4To(Vector4 p_startValue, Vector4 p_finalValue, float p_duration, AnimationCurve p_normalizedCurve,  bool p_useTimeScale, UpdateTimeType p_updateMode, TweenLoopType p_loopMode,  Action<Vector4> p_callbackUpdate)
		{
			TweenNode __temp = FloatTo(0, 1, p_duration, p_normalizedCurve, p_useTimeScale, p_loopMode, p_updateMode, delegate(float newFloat)
			{
				p_callbackUpdate(Vector4.Lerp(p_startValue, p_finalValue, newFloat));
			});

			return __temp;
		}

		#if SKIP_CODE_IN_DOCUMENTATION
		/// @endcond
		#endif

		#endregion

		#region Clean Nodes
		/// <summary>
		/// Clear all tween nodes
		/// </summary>
		/// <remarks>
		/// ClearAllNodes should be called to clean all tween nodes initialized via this Tween class. 
		/// Useful to make sure nothing is left behind when not intended.
		/// 
		/// <code>
		/// 
		/// void StartSphereAnimation(GameObejct p_sphere)
		/// {
		/// 	TweenNode __moveSphereInCircle = Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_value)
		/// 	{
		/// 		p_sphere.transform.position = new Vector3(Mathf.Sin(p_value), Mathf.Cos(p_value), 0);
		/// 	});
		/// 
		/// }
		///
		/// void override OnUnloadedScene()
		/// {
		/// 	Vox.Tween.ClearAllNodes();
		/// }
		/// 
		/// </code>
		///  
		/// </remarks>  
		public static void ClearAllNodes()
		{
			TweenModule.instance.ClearAllNodes ();
		}

		/// <summary>
		/// Clear all tween nodes with the passed tag
		/// </summary>
		/// <remarks>
		/// ClearNodesWithTag clean all nodes which had the passed tag attached to them. For more information check Vox.TweenNode.tag.
		/// 
		/// <code>
		/// // Script inside a state of a statemachine
		/// 
		/// void StartSphereAnimation(GameObejct p_sphere)
		/// {
		/// 	TweenNode __moveSphereInCircle = Tween.FloatTo(0f, 1f, 1f, EaseType.Linear, delegate(float p_value)
		/// 	{
		/// 		p_sphere.transform.position = new Vector3(Mathf.Sin(p_value), Mathf.Cos(p_value), 0);
		/// 	});
		/// 
		/// 	__moveSphereInCircle.tag = "GamePlayState";
		/// }
		///
		/// void override StateOnUnloadedScene()
		/// {
		/// 	Vox.Tween.ClearNodesWithTag("GamePlayState");
		/// }
		/// </code>
		///  
		/// </remarks> 
		public static void ClearNodesWithTag(string p_tag)
		{
			TweenModule.instance.ClearNodesWithTag (p_tag);
		}

		/// <summary>
		/// Clear all tween nodes excpet for the ones with the passed tag
		/// </summary>
		/// <remarks>
		/// ClearNodesExceptForTag clean all nodes except for the ones with the passed tag attached to them. For more information check Vox.TweenNode.tag.
		/// 
		/// <code>
		///
		/// void override OnUnloadedScene()
		/// {
		/// 	Vox.Tween.ClearNodesExceptForTag("GlobalNodes");
		/// }
		/// 
		/// </code>
		///  
		/// </remarks> 
		public static void ClearNodesExceptForTag(string p_tag)
		{
			TweenModule.instance.ClearNodesExceptForTag (p_tag);
		}

		#endregion
	}
}
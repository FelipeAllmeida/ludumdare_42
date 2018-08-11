using UnityEngine;
using System.Collections;
using Vox;

namespace VoxInternal
{
	/// <summary>
	/// Class used internally to calculate ease maths.
	/// </summary>
	public class EaseFunctionMaths
	{
		/// <summary>
		/// Function to redirect for private function to calculate ease function request and return the float value.
		/// </summary>
		public static float GetTransaction(float start, float end, float normalizedTime, EaseType ease)
		{
			switch(ease)
			{
			default:
			case EaseType.LINEAR:
				return Linear(start, end, normalizedTime);
			case EaseType.SPRING:
				return SpringEase(start, end, normalizedTime);
			case EaseType.QUAD_IN:
				return EaseInQuad(start, end, normalizedTime);
			case EaseType.QUAD_OUT:
				return EaseOutQuad(start, end, normalizedTime);
			case EaseType.QUAD_IN_OUT:
				return EaseInOutQuad(start, end, normalizedTime);
			case EaseType.CUBIC_IN:
				return EaseInCubic(start, end, normalizedTime);
			case EaseType.CUBIC_OUT:
				return EaseOutCubic(start, end, normalizedTime);
			case EaseType.CUBIC_IN_OUT:
				return EaseInOutCubic(start, end, normalizedTime);
			case EaseType.BOUNCE_IN:
				return EaseInBounce(start, end, normalizedTime);
			case EaseType.BOUNCE_OUT:
				return EaseOutBounce(start, end, normalizedTime);
			case EaseType.ELASTIC_IN:
				return EaseInElastic(start, end, normalizedTime);
			case EaseType.ELASTIC_OUT:
				return EaseOutElastic(start, end, normalizedTime);
			case EaseType.ELASTIC_IN_OUT:
				return EaseInOutElastic(start, end, normalizedTime);
			case EaseType.QUART_IN:
				return EaseInQuart(start, end, normalizedTime);
			case EaseType.QUART_OUT:
				return EaseOutQuart(start, end, normalizedTime);
			case EaseType.QUART_IN_OUT:
				return EaseInOutQuart(start, end, normalizedTime);
			case EaseType.QUINT_IN:
				return EaseInQuint(start, end, normalizedTime);
			case EaseType.QUINT_OUT:
				return EaseOutQuint(start, end, normalizedTime);
			case EaseType.QUINT_IN_OUT:
				return EaseInOutQuint(start, end, normalizedTime);
			case EaseType.SINE_IN:
				return EaseInSine(start, end, normalizedTime);
			case EaseType.SINE_OUT:
				return EaseOutSine(start, end, normalizedTime);
			case EaseType.SINE_IN_OUT:
				return EaseInOutSine(start, end, normalizedTime);
//			case EaseType.EXPO_IN:
//				return EaseInExpo(start, end, normalizedTime);
//			case EaseType.EXPO_OUT:
//				return EaseOutExpo(start, end, normalizedTime);
//			case EaseType.EXPO_IN_OUT:
//				return EaseInOutExpo(start, end, normalizedTime);
			case EaseType.CIRC_IN:
				return EaseInCirc(start, end, normalizedTime);
			case EaseType.CIRC_OUT:
				return EaseOutCirc(start, end, normalizedTime);
			case EaseType.CIRC_IN_OUT:
				return EaseInOutCirc(start, end, normalizedTime);
			case EaseType.BACK_IN:
				return EaseInBack(start, end, normalizedTime);
			case EaseType.BACK_OUT:
				return EaseOutBack(start, end, normalizedTime);
			case EaseType.BACK_IN_OUT:
				return EaseInOutBack(start, end, normalizedTime);
			}
		}

		/// <summary>
		/// Linear ease
		/// </summary>
		private static  float Linear(float start, float end, float normalizedTime)
		{
			return Mathf.Lerp(start, end, normalizedTime);
		}
		/// <summary>
		/// in Quad ease
		/// </summary>
		private static  float EaseInQuad(float start, float final, float normalizedTime)
		{
			final -= start;
			return final * normalizedTime * normalizedTime + start;
		}
		/// <summary>
		/// out Quad ease
		/// </summary>
		private static  float EaseOutQuad(float start, float end, float normalizedTime)
		{
			end -= start;
			return -end * normalizedTime * (normalizedTime - 2) + start;
		}
		/// <summary>
		/// in out Quad ease
		/// </summary>
		private static  float EaseInOutQuad(float start, float end, float normalizedTime)
		{
			normalizedTime /= .5f;
			end -= start;
			if (normalizedTime < 1) return end / 2 * normalizedTime * normalizedTime + start;
			normalizedTime--;
			return -end / 2 * (normalizedTime * (normalizedTime - 2) - 1) + start;
		}
		/// <summary>
		/// Spring ease
		/// </summary>
		private static  float SpringEase(float start, float end, float normalizedTime)
		{
			normalizedTime = Mathf.Clamp01(normalizedTime);
			normalizedTime = (Mathf.Sin(normalizedTime * Mathf.PI * (0.2f + 2.5f * normalizedTime * normalizedTime * normalizedTime)) * Mathf.Pow(1f - normalizedTime, 2.2f) + normalizedTime) * (1f + (1.2f * (1f - normalizedTime)));
			return start + (end - start) * normalizedTime;
		}
		/// <summary>
		/// in cubic ease
		/// </summary>
		private static float EaseInCubic(float start, float end, float normalizedTime)
		{
			end -= start;
			return end * normalizedTime * normalizedTime * normalizedTime + start;
		}
		/// <summary>
		/// out cubic ease
		/// </summary>
		private static float EaseOutCubic(float start, float end, float normalizedTime)
		{
			normalizedTime--;
			end -= start;
			return end * (normalizedTime * normalizedTime * normalizedTime + 1) + start;
		}
		/// <summary>
		/// in out cubic ease
		/// </summary>
		private static float EaseInOutCubic(float start, float end, float normalizedTime)
		{
			normalizedTime /= .5f;
			end -= start;
			if (normalizedTime < 1) return end / 2 * normalizedTime * normalizedTime * normalizedTime + start;
			normalizedTime -= 2;
			return end / 2 * (normalizedTime * normalizedTime * normalizedTime + 2) + start;
		}
		/// <summary>
		/// in bounce ease
		/// </summary>
		private static float EaseInBounce(float start, float end, float normalizedTime)
		{
			end -= start;
			return end - EaseOutBounce(0, end, 1f-normalizedTime) + start;
		}
		/// <summary>
		/// out bounce ease
		/// </summary>
		private static float EaseOutBounce(float start, float end, float normalizedTime)
		{
			normalizedTime /= 1f;
			end -= start;
			if (normalizedTime < (1 / 2.75f))
			{
				return end * (7.5625f * normalizedTime * normalizedTime) + start;
			}
			else if (normalizedTime < (2 / 2.75f))
			{
				normalizedTime -= (1.5f / 2.75f);
				return end * (7.5625f * (normalizedTime) * normalizedTime + .75f) + start;
			}
			else if (normalizedTime < (2.5 / 2.75))
			{
				normalizedTime -= (2.25f / 2.75f);
				return end * (7.5625f * (normalizedTime) * normalizedTime + .9375f) + start;
			}
			else
			{
				normalizedTime -= (2.625f / 2.75f);
				return end * (7.5625f * (normalizedTime) * normalizedTime + .984375f) + start;
			}
		}
		/// <summary>
		/// in elastic ease
		/// </summary>
		private static float EaseInElastic(float start, float end, float normalizedTime)
		{
			end -= start;
			
			float __temp1 = 1f;
			float __temp2 = __temp1 * .3f;
			float __temp3 = 0;
			float __temp4 = 0;
			
			if (normalizedTime == 0) 
				return start;
			
			if ((normalizedTime /= __temp1) == 1) 
				return start + end;
			
			if (__temp4 == 0f || __temp4 < Mathf.Abs(end))
			{
				__temp4 = end;
				__temp3 = __temp2 / 4;
			}
			else
			{
				__temp3 = __temp2 / (2 * Mathf.PI) * Mathf.Asin(end / __temp4);
			}
			
			return -(__temp4 * Mathf.Pow(2, 10 * (normalizedTime-=1)) * Mathf.Sin((normalizedTime * __temp1 - __temp3) * (2 * Mathf.PI) / __temp2)) + start;
		}		
		/// <summary>
		/// out elastic ease
		/// </summary>
		private static float EaseOutElastic(float start, float end, float normalizedTime)
		{
			end -= start;
			
			float __temp1 = 1f;
			float __temp2 = __temp1 * .3f;
			float __temp3 = 0;
			float __temp4 = 0;
			
			if (normalizedTime == 0) 
				return start;
			
			if ((normalizedTime /= __temp1) == 1) 
				return start + end;
			
			if (__temp4 == 0f || __temp4 < Mathf.Abs(end))
			{
				__temp4 = end;
				__temp3 = __temp2 / 4;
			}
			else
			{
				__temp3 = __temp2 / (2 * Mathf.PI) * Mathf.Asin(end / __temp4);
			}
			
			return (__temp4 * Mathf.Pow(2, -10 * normalizedTime) * Mathf.Sin((normalizedTime * __temp1 - __temp3) * (2 * Mathf.PI) / __temp2) + end + start);
		}		
		/// <summary>
		/// in out elastic ease
		/// </summary>
		private static float EaseInOutElastic(float start, float end, float normalizedTime)
		{
			end -= start;
			
			float __temp1 = 1f;
			float __temp2 = __temp1 * .3f;
			float __temp3 = 0;
			float __temp4 = 0;
			
			if (normalizedTime == 0) 
				return start;
			
			if ((normalizedTime /= __temp1/2) == 2) 
				return start + end;
			
			if (__temp4 == 0f || __temp4 < Mathf.Abs(end))
			{
				__temp4 = end;
				__temp3 = __temp2 / 4;
			}
			else
			{
				__temp3 = __temp2 / (2 * Mathf.PI) * Mathf.Asin(end / __temp4);
			}
			
			if (normalizedTime < 1) 
				return -0.5f * (__temp4 * Mathf.Pow(2, 10 * (normalizedTime-=1)) * Mathf.Sin((normalizedTime * __temp1 - __temp3) * (2 * Mathf.PI) / __temp2)) + start;
			
			return __temp4 * Mathf.Pow(2, -10 * (normalizedTime-=1)) * Mathf.Sin((normalizedTime * __temp1 - __temp3) * (2 * Mathf.PI) / __temp2) * 0.5f + end + start;
		}		
		/// <summary>
		/// in quart ease
		/// </summary>
		private static float EaseInQuart(float start, float end, float normalizedTime)
		{
			end -= start;
			return end * normalizedTime * normalizedTime * normalizedTime * normalizedTime + start;
		}
		/// <summary>
		/// out quart ease
		/// </summary>
		private static float EaseOutQuart(float start, float end, float normalizedTime)
		{
			normalizedTime--;
			end -= start;
			return -end * (normalizedTime * normalizedTime * normalizedTime * normalizedTime - 1) + start;
		}
		/// <summary>
		/// in out quart ease
		/// </summary>
		private static float EaseInOutQuart(float start, float end, float normalizedTime)
		{
			normalizedTime /= .5f;
			end -= start;
			if (normalizedTime < 1) 
				return end / 2 * normalizedTime * normalizedTime * normalizedTime * normalizedTime + start;
			normalizedTime -= 2;
			return -end / 2 * (normalizedTime * normalizedTime * normalizedTime * normalizedTime - 2) + start;
		}
		/// <summary>
		/// in quint ease
		/// </summary>
		private static float EaseInQuint(float start, float end, float normalizedTime)
		{
			end -= start;
			return end * normalizedTime * normalizedTime * normalizedTime * normalizedTime * normalizedTime + start;
		}
		/// <summary>
		/// out quint ease
		/// </summary>
		private static float EaseOutQuint(float start, float end, float normalizedTime)
		{
			normalizedTime--;
			end -= start;
			return end * (normalizedTime * normalizedTime * normalizedTime * normalizedTime * normalizedTime + 1) + start;
		}
		/// <summary>
		/// in out quint ease
		/// </summary>
		private static float EaseInOutQuint(float start, float end, float normalizedTime)
		{
			normalizedTime /= .5f;
			end -= start;
			if (normalizedTime < 1) 
				return end / 2 * normalizedTime * normalizedTime * normalizedTime * normalizedTime * normalizedTime + start;
			normalizedTime -= 2;
			return end / 2 * (normalizedTime * normalizedTime * normalizedTime * normalizedTime * normalizedTime + 2) + start;
		}
		/// <summary>
		/// in sine ease
		/// </summary>
		private static float EaseInSine(float start, float end, float normalizedTime)
		{
			end -= start;
			return -end * Mathf.Cos(normalizedTime / 1 * (Mathf.PI / 2)) + end + start;
		}
		/// <summary>
		/// out sine ease
		/// </summary>
		private static float EaseOutSine(float start, float end, float normalizedTime)
		{
			end -= start;
			return end * Mathf.Sin(normalizedTime / 1 * (Mathf.PI / 2)) + start;
		}
		/// <summary>
		/// in out sine ease
		/// </summary>
		private static float EaseInOutSine(float start, float end, float normalizedTime)
		{
			end -= start;
			return -end / 2 * (Mathf.Cos(Mathf.PI * normalizedTime / 1) - 1) + start;
		}
		/// <summary>
		/// in expo ease
		/// </summary>
//		private static float EaseInExpo(float start, float end, float normalizedTime)
//		{
//			end -= start;
//			return end * Mathf.Pow(2, 10 * (normalizedTime / 1 - 1)) + start;
//		}
//		/// <summary>
//		/// out expo ease
//		/// </summary>
//		private static float EaseOutExpo(float start, float end, float normalizedTime)
//		{
//			end -= start;
//			return end * (-Mathf.Pow(2, -10 * normalizedTime / 1) + 1) + start;
//		}
//		/// <summary>
//		/// in out expo ease
//		/// </summary>
//		private static float EaseInOutExpo(float start, float end, float normalizedTime)
//		{
//			normalizedTime /= .5f;
//			end -= start;
//
//			if (normalizedTime < 1) 
//				return end / 2 * Mathf.Pow(2, 10 * (normalizedTime - 1)) + start;
//			
//			normalizedTime--;
//
//			return end / 2 * (-Mathf.Pow(2, -10 * normalizedTime) + 2) + start;
//		}
		/// <summary>
		/// in circ ease
		/// </summary>
		private static float EaseInCirc(float start, float end, float normalizedTime)
		{
			end -= start;
			return -end * (Mathf.Sqrt(1 - normalizedTime * normalizedTime) - 1) + start;
		}
		/// <summary>
		/// out circ ease
		/// </summary>
		private static float EaseOutCirc(float start, float end, float normalizedTime)
		{
			normalizedTime--;
			end -= start;
			return end * Mathf.Sqrt(1 - normalizedTime * normalizedTime) + start;
		}
		/// <summary>
		/// in out circ ease
		/// </summary>
		private static float EaseInOutCirc(float start, float end, float normalizedTime)
		{
			normalizedTime /= .5f;
			end -= start;
			if (normalizedTime < 1) 
				return -end / 2 * (Mathf.Sqrt(1 - normalizedTime * normalizedTime) - 1) + start;
			normalizedTime -= 2;
			return end / 2 * (Mathf.Sqrt(1 - normalizedTime * normalizedTime) + 1) + start;
		}
		/// <summary>
		/// in back ease
		/// </summary>
		private static float EaseInBack(float start, float end, float normalizedTime)
		{
			end -= start;
			normalizedTime /= 1;
			float s = 1.70158f;
			return end * (normalizedTime) * normalizedTime * ((s + 1) * normalizedTime - s) + start;
		}
		/// <summary>
		/// out back ease
		/// </summary>
		private static float EaseOutBack(float start, float end, float normalizedTime)
		{
			float __temp = 1.70158f;
			end -= start;
			normalizedTime = (normalizedTime / 1) - 1;
			return end * ((normalizedTime) * normalizedTime * ((__temp + 1) * normalizedTime + __temp) + 1) + start;
		}
		/// <summary>
		/// in out back ease
		/// </summary>
		private static float EaseInOutBack(float start, float end, float normalizedTime)
		{
			float __temp = 1.70158f;
			end -= start;
			normalizedTime /= .5f;
			if ((normalizedTime) < 1)
			{
				__temp *= (1.525f);
				return end / 2 * (normalizedTime * normalizedTime * (((__temp) + 1) * normalizedTime - __temp)) + start;
			}
			normalizedTime -= 2;
			__temp *= (1.525f);
			return end / 2 * ((normalizedTime) * normalizedTime * (((__temp) + 1) * normalizedTime + __temp) + 2) + start;
		}
		 
	}
}
using UnityEngine;
using System.Collections;
using Vox;

public enum EaseType
{
	LINEAR,
	SPRING,
	QUAD_IN,
	QUAD_OUT,
	QUAD_IN_OUT,
	CUBIC_IN,
	CUBIC_OUT,
	CUBIC_IN_OUT,
	BOUNCE_IN,
	BOUNCE_OUT,
	ELASTIC_IN,
	ELASTIC_OUT,
	ELASTIC_IN_OUT,
	QUART_IN,
	QUART_OUT,
	QUART_IN_OUT,
	QUINT_IN,
	QUINT_OUT,
	QUINT_IN_OUT,
	SINE_IN,
	SINE_OUT,
	SINE_IN_OUT,
	//		EXPO_IN,
	//		EXPO_OUT,
	//		EXPO_IN_OUT,
	CIRC_IN,
	CIRC_OUT,
	CIRC_IN_OUT,
	BACK_IN,
	BACK_OUT,
	BACK_IN_OUT
}

namespace Vox
{
	/// <summary>
	/// Types of eases available to use in tweens.
	/// </summary>
	/// <remarks>
	/// Types of eases available to be used in tweens. <BR>
	/// "Easing functions specify the rate of change of a parameter over time. <BR>
	/// Objects in real life don’t just start and stop instantly, and almost never move at a constant speed. When we open a drawer, we first move it quickly, and slow it down as it comes out. Drop something on the floor, and it will first accelerate downwards, and then bounce back up after hitting the floor.<BR>
	/// <A href="http://easings.net/pt"><STRONG>This page</STRONG></A> helps you choose the right easing function."
	/// </remarks>
	#region EaseEnum
//	public enum EaseType
//	{
//		LINEAR,
//		SPRING,
//		QUAD_IN,
//		QUAD_OUT,
//		QUAD_IN_OUT,
//		CUBIC_IN,
//		CUBIC_OUT,
//		CUBIC_IN_OUT,
//		BOUNCE_IN,
//		BOUNCE_OUT,
//		ELASTIC_IN,
//		ELASTIC_OUT,
//		ELASTIC_IN_OUT,
//		QUART_IN,
//		QUART_OUT,
//		QUART_IN_OUT,
//		QUINT_IN,
//		QUINT_OUT,
//		QUINT_IN_OUT,
//		SINE_IN,
//		SINE_OUT,
//		SINE_IN_OUT,
////		EXPO_IN,
////		EXPO_OUT,
////		EXPO_IN_OUT,
//		CIRC_IN,
//		CIRC_OUT,
//		CIRC_IN_OUT,
//		BACK_IN,
//		BACK_OUT,
//		BACK_IN_OUT
//	}
	#endregion
}

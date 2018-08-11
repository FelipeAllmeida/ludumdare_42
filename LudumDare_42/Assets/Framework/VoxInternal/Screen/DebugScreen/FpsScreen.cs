using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace VoxInternal
{
	/// <summary>
	/// Class used to debug FPS in the screen.
	/// </summary>
	public class FpsScreen
	{
		#region Private Data
		private string[] _fpsSTringsFrom0to99 = 
		{
			"00", "01", "02", "03", "04", "05", "06", "07", "08", "09",
			"10", "11", "12", "13", "14", "15", "16", "17", "18", "19",
			"20", "21", "22", "23", "24", "25", "26", "27", "28", "29",
			"30", "31", "32", "33", "34", "35", "36", "37", "38", "39",
			"40", "41", "42", "43", "44", "45", "46", "47", "48", "49",
			"50", "51", "52", "53", "54", "55", "56", "57", "58", "59",
			"60", "61", "62", "63", "64", "65", "66", "67", "68", "69",
			"70", "71", "72", "73", "74", "75", "76", "77", "78", "79",
			"80", "81", "82", "83", "84", "85", "86", "87", "88", "89",
			"90", "91", "92", "93", "94", "95", "96", "97", "98", "99"
		};

		private string _fpsText;
		private Color _guiColor = Color.green;

		#endregion

		private Queue<float> _framesQueue = new Queue<float>();

		#region Update
		/// <summary>
		/// Update the fps check by storing frames in a queue.
		/// </summary>
		public void UpdateFPS () 
		{
			_framesQueue.Enqueue(Time.time);

			while (Time.time - _framesQueue.Peek() > 1)
			{
				_framesQueue.Dequeue();  
			}

			// display two fractional digits (f2 format)
			int fps = _framesQueue.Count;

			_fpsText = _fpsSTringsFrom0to99[Mathf.Clamp(fps, 0, 99)];

			if (fps > 50 && fps <= 60)
				_guiColor = Color.green;
			else if(fps > 30 && fps < 50)
				_guiColor = Color.yellow;
			else if(fps < 30)
				_guiColor = Color.red;
		}

		public string GetFPSValue()
		{
			return _fpsText;
		}

		#endregion

		#region GUI
		/// <summary>
		/// Function that draws the fps on the screen.
		/// </summary>
		public void OnFPSGUI()
		{
			GUI.color = _guiColor;
			GUI.Label (new Rect (10, 10, 100, 100), _fpsText);
		}
		#endregion
	}
}
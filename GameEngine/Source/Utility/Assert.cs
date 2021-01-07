using System;
using System.Collections.Generic;
using System.Text;

//----------------------------------------------------------------------------------
//----------------------------------------------------------------------------------
namespace GameEngine
{
	//----------------------------------------------------------------------------------
	//----------------------------------------------------------------------------------
	public static class Assert
	{
		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public static bool Valid(object oObj, string szMsg, object objToPrint)
		{
			if (oObj == null)
				Debug.Log("Assertion Failed: " + szMsg + " (" + objToPrint + ")");

			return (oObj != null);
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public static bool Valid(object oObj, string szMsg)
		{
			if (oObj == null)
				Debug.Log("Assertion Failed: " + szMsg);

			return (oObj != null);
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public static bool Valid(object oObj)
		{
			return Valid(oObj, "Null Reference.");
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public static bool True(bool bCondition, string szMsg, object objToPrint)
		{
			if (!bCondition)
				Debug.Log("Assertion Failed: " + szMsg + " (" + objToPrint + ")");

			return bCondition;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public static bool True(bool bCondition, string szMsg)
		{
			if (!bCondition)
				Debug.Log("Assertion Failed: " + szMsg);

			return bCondition;
		}

		//----------------------------------------------------------------------------------
		//----------------------------------------------------------------------------------
		public static bool True(bool bCondition)
		{
			return True(bCondition, "Condition was false.");
		}
	}
}

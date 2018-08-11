using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Collections;

namespace VoxInternal.Editor
{
	/// <summary>
	/// Class where utilities available from Vox tab are implemented.
	/// </summary>
	public class FrameworkMenuUtilities
	{
	//	[MenuItem ("Vox/Script/Validate all")]
	//	public static void ValidateCode()
	//    {
	//        //Not ready yet
	//        if(UnityEngine.Random.Range(0,2) >= 0)
	//            return;
	//        
	//
	//        //Filter content
	//        string __scriptsFolderName = "Scripts";
	//        string __projectFolderName = "_Project";
	//        Dictionary<string, string> logFilter = new Dictionary<string, string>()
	//		{
	//			//Key to search inline					//(W - Warning, E - Error, L - log) + Key to message
	//			{"Time.deltaTime",						"L DeltaTime"},
	//			{"using System.Threading;", 			"W Threading"},
	//			{"unsafe",								"E Unsafe"},
	//			{"){",									"W Brackets"},
	//			{".Find(",								"E Find"},
	//			{".FindGameObjectsWithTag(",			"E Find"},
	//			{".FindGameObjectWithTag(",				"E Find"},
	//			{".FindObjectOfType(",					"E Find"},
	//			{".FindObjectsOfType(",					"E Find"},
	//			{".FindObjectsOfTypeIncludingAssets(",	"E Find"},
	//			{".FindSceneObjectsOfType(",			"E Find"},
	//			{".FindWithTag(",						"E Find"}
	//		};
	//
	//        Dictionary<string, string> logMessages = new Dictionary<string, string>()
	//		{
	//			{"ScriptsUnorganized",//Look for scripts outside its right folder
	//				"There are C# scripts out of ''Assets/Scripts/'' and ''Assets/_Project/Scripts/'' folders.\nPlease move this scripts to its location to keep the project organized."},
	//			{"Reflection",//Save functions in buffer, than reread all files looking for functions inside strings
	//				"Never use reflection. If you created a method the use that please think in another way to do what you want.\nIf you are using any thrid part library that require that please request the functionality as a update to this framework (You can talk with Ivan S. Cavalheiro)."},
	//			{"DeltaTime",
	//				"Delta time is not safe, mainly becouse when Time is paused it will always return 0.\nTry to use Timer.RealDeltaTime if its possible."},
	//			{"Unsafe",
	//				"Unsafe code is only needed when you try to use pointers inside C# code.\nIts highly recommended to not use this in your code, mainly becouse its a infity font of bugs and becouse there are always a way to avoid it with something else."},
	//			{"Threading",
	//				"Threads are a bit Complicated to use. There are always a way to avoid this kind of methods in your script.\nPlease dont use this."},
	//			{"Brackets",//Remember to skip "},"
	//				"Wrong identation.\nBrackets: ''{'', ''}'', ''};'' or ''});'' sould be isolated in a single line."},
	//			{"Find", 
	//				"Find methods are not fast enought for being used in games (mainly inside methods called once per frame).\nPlease redo the logic of your script to remove such methods."},
	//			{"NoRegionUsage",//Look for methods declarations that dont have   before it
	//				"You should use ''  *method name*'' before and '' '' after every method of every class you make.\nThis is a essential to maintain the organization of the project."},
	//			{"Clean",//Return when code has no Warnings/Errors (Logs dont count)
	//				"Congrats! There are no errors found at any of your scripts placed at scanned folders.\nKeep working like that!"},
	//			{"ErrorsFound",//Return when code has Warnings/Errors (ErrorsFound and Clean will never be called at same time)
	//				"There are errors in one or more scripts.\nCheck the log for more information."},
	//			{"NoScriptsFound",
	//				"No scripts were found.\nCheck if they are in the correct folder: ''Assets/Scripts/'' or ''Assets/_Project/Scripts''."}
	//		};
	//
	//        string __noScriptsAndNoProjectScripts = "No scripts folder found. The scripts must be within ''Assets/" + __scriptsFolderName + "/'' or preferably on ''Assets/" + __projectFolderName + "/" + __scriptsFolderName + "/''";
	//        string __hasScriptsAndNoProjectScripts = "No ''Assets/" + __projectFolderName + "/" + __scriptsFolderName + "/'' foulder found. Please, preferably use this folder instead of ''Assets/" + __scriptsFolderName + "/'' folder to maintain the organization of the project. ";
	//        string __hasScriptsAndHastProjectScripts = "You have both ''Assets/" + __scriptsFolderName + "/'' and ''Assets/" + __projectFolderName + "/" + __scriptsFolderName + "'' folders. Please preferably use only ''Assets/" + __projectFolderName + "/" + __scriptsFolderName + "'' folder for organization purposes.";
	//
	//        //Setup temp variables
	//        string __scriptsDir = @"Assets/";
	//        string __projectScriptsDir = @"Assets/" + __projectFolderName + "/";
	//        bool __hasScripts = false;
	//        bool __hasProjectScripts = false;
	//        bool __errorFound = false;
	//
	//        //Verify if scripts folder exist
	//        string[] __directory = Directory.GetDirectories(__scriptsDir);
	//        foreach (string verify in __directory)
	//        {
	//            if (verify == (__scriptsDir + __scriptsFolderName))
	//                __hasScripts = true;
	//            if (verify == (__scriptsDir + __projectFolderName))
	//            {
	//                string[] ___projectDirectory = Directory.GetDirectories(__projectScriptsDir);
	//                foreach (string _verify in ___projectDirectory)
	//                {
	//                    if (_verify == (__projectScriptsDir + __scriptsFolderName))
	//                        __hasProjectScripts = true;
	//                }
	//            }
	//        }
	//        if (!__hasScripts && !__hasProjectScripts)
	//            Debug.LogError(__noScriptsAndNoProjectScripts);
	//
	//        if (__hasScripts && !__hasProjectScripts)
	//            Debug.LogWarning(__hasScriptsAndNoProjectScripts);
	//
	//        if (__hasScripts && __hasProjectScripts)
	//            Debug.LogWarning(__hasScriptsAndHastProjectScripts);
	//
	//        //Get files
	//        string[] __files = VFramework.ReturnFilesOnDirectory(@"Assets/", true);
	//        List<string> __scriptsFound = new List<string>();
	//
	//        //Get scripts
	//        foreach (string file in __files)
	//        {
	//            if (file.EndsWith(".cs"))
	//            {
	//                if (file.Contains(@"/Scripts/"))
	//                    __scriptsFound.Add(file);
	//                else if (!file.Contains(@"/Framework/") && !file.Contains(@"/Plugins/"))
	//                {
	//                    Debug.LogError(logMessages["ScriptsUnorganized"] + " Script unorganized at: " + file + ". This script will not be scanned.");
	//                }
	//            }
	//        }
	//
	//        //Scan scripts
	//        foreach (string scriptPath in __scriptsFound)
	//        {
	//            string[] __lines = File.ReadAllLines(scriptPath);
	//
	//            //Pass filters line by line
	//            int __lineIndex = 0;
	//            foreach (string line in __lines)
	//            {
	//                __lineIndex++;
	//                foreach (var filter in logFilter)
	//                {
	//                    if (line.Contains(filter.Key))
	//                    {
	//                        string[] _____messageKey = logFilter[filter.Key].Split(' ');
	//                        string _____message = logMessages[_____messageKey[1]];
	//
	//                        switch (_____messageKey[0])
	//                        {
	//                            default:
	//                            case "E":
	//                                __errorFound = true;
	//                                Debug.LogError(_____message + "\nScript: " + scriptPath + " line " + __lineIndex);
	//                                break;
	//                            case "W":
	//                                __errorFound = true;
	//                                Debug.LogWarning(_____message + "\nScript: " + scriptPath + " line " + __lineIndex);
	//                                break;
	//                            case "L":
	//                                Debug.Log(_____message + "\nScript: " + scriptPath + " line " + __lineIndex);
	//                                break;
	//                        }
	//                    }
	//                }
	//            }
	//        }
	//
	//        //Finish method(If your script has any error thats not the place to look!)
	//        if (__errorFound)
	//            Debug.LogError(logMessages["ErrorsFound"]);
	//        else if (__scriptsFound.Count > 0)
	//            Debug.Log(logMessages["Clean"]);
	//        else
	//            Debug.LogWarning(logMessages["NoScriptsFound"]);
	//    }

		/// <summary>
		/// Clear all playerprefs stored in the system.
		/// </summary>
		[MenuItem ("Vox/Utilities/Clean player prefs &p")]
		public static void CleanPlayerPrefs()
		{
			PlayerPrefs.DeleteAll();
		}

        //[MenuItem( "Vox/Utilities/Apply All Prefabs (Be Carefull)" )]
        //public static void ApllyAllPrefabsSelected()
        //{
        //	for (int i = 0; i < UnityEditor.Selection.gameObjects.Length; i++)
        //	{
        //		if (UnityEditor.PrefabUtility.GetPrefabObject( UnityEditor.Selection.gameObjects[i] ))
        //		{
        //			UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications( UnityEditor.Selection.gameObjects[i] );
        //		}
        //	}
        //}
    }
}
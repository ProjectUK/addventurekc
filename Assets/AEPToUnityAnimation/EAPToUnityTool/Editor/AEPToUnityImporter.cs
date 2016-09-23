using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AEPToUnityImporter
{
	[MenuItem("Window/AEPToUnity/ImportAEPToUnity")]
	public static void ImportAEPToUnity()
	{
		
		#if UNITY_4_0_0 ||UNITY_4_0 || UNITY_4_0_1||UNITY_4_1||UNITY_4_2||UNITY_4_3||UNITY_4_4||UNITY_4_5||UNITY_4_6||UNITY_4_7||UNITY_4_8||UNITY_4_9
		string fileName="AEPToUnity4.importer";
		#else
		string fileName="AEPToUnity5.importer";
		#endif
		
		#if UNITY_4_0_0 ||UNITY_4_0 || UNITY_4_0_1||UNITY_4_1||UNITY_4_2||UNITY_4_3||UNITY_4_4||UNITY_4_5||UNITY_4_6||UNITY_4_7||UNITY_4_8||UNITY_4_9
		string fileDll="AEPToUnity4.dll";
		#else
		string fileDll="AEPToUnity5.dll";
		#endif
		string currentPath=Application.dataPath;
		//Debug.LogError(currentPath);
		string[] fullPath = Directory.GetFiles(currentPath+"/AEPToUnityAnimation", fileName ,SearchOption.AllDirectories);
		string pathFile="";
		if(fullPath.Length>0)
		{
			pathFile=fullPath[0];
		}
		if(pathFile.Length>0)
		{
			ChangeFileType(fileName,pathFile);
		}
		else
		{
			fullPath = Directory.GetFiles(currentPath, fileName ,SearchOption.AllDirectories);
			if(fullPath.Length>0)
			{
				pathFile=fullPath[0];
			}
			if(pathFile.Length>0)
			{
				ChangeFileType(fileName,pathFile);
			}
			else
			{
				
				#if UNITY_4_0_0 ||UNITY_4_0 || UNITY_4_0_1||UNITY_4_1||UNITY_4_2||UNITY_4_3||UNITY_4_4||UNITY_4_5||UNITY_4_6||UNITY_4_7||UNITY_4_8||UNITY_4_9
				string strInfo="Unity 4";
				#else
				string strInfo="Unity 5";
				#endif
				fullPath = Directory.GetFiles(currentPath+"/AEPToUnityAnimation", fileDll ,SearchOption.AllDirectories);
				if(fullPath.Length<0)
				{
					fullPath = Directory.GetFiles(currentPath, fileDll ,SearchOption.AllDirectories);
					if(fullPath.Length<0)
					{
						EditorUtility.DisplayDialog("Import Error","Can not find file \""+fileName+"\" for importing !","OK");
					}
					else
					{
						EditorUtility.DisplayDialog("Already imported","To using AEP Converter press\nWindow->AEPToUnity->"+strInfo,"OK");
					}
				}
				else{
					EditorUtility.DisplayDialog("Already imported","To using AEP Converter press\nWindow->AEPToUnity->"+strInfo,"OK");
				}
			}
		}
	}
	static private void ChangeFileType(string filename, string path)
	{
		string str = Path.ChangeExtension(path, ".dll");
		File.Move(path,str);
		AssetDatabase.Refresh();
		
		#if UNITY_4_0_0 ||UNITY_4_0 || UNITY_4_0_1||UNITY_4_1||UNITY_4_2||UNITY_4_3||UNITY_4_4||UNITY_4_5||UNITY_4_6||UNITY_4_7||UNITY_4_8||UNITY_4_9
		string strInfo="Unity 4";
		#else
		string strInfo="Unity 5";
		#endif
		EditorUtility.DisplayDialog("Import Finish","import finish, To using AEP Converter press\nWindow->AEPToUnity->"+strInfo,"OK");
		
	}
	
}

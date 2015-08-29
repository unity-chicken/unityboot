using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.IO;
using LitJson;

public class Builder {
	static string BuildName = "SPROJECT";
	static string[] PackageScenes = {
		"Assets/Demo/Scene/BootDemo.unity",
	};

	static string[] BlackListScenes = {
		"Assets/Demo/Scene/Test.unity",
	};

	[MenuItem("Build/Android/Build Resources")]
	static void ExportResourceMenuAndroid() {
		ExportResource("Bin", BuildTarget.Android);
	}

	[MenuItem("Build/Android/Build Scenes")]
	static void ExportSceneAssetBundleMenuAndroid() {
		ExportSceneAssetBundle("Bin", BuildTarget.Android);
	}

	[MenuItem("Build/Android/Export Google Anroid Project")]
	static void ExportMenuAndroid() {
		ExportAndroidProject();
	}

	[MenuItem("Build/Android/Full Build")]
	static void BuildAndroidMenuAndroid() {
		if (Directory.Exists("Bin") == true) {
			Directory.Delete("Bin", true);
		}
		Directory.CreateDirectory("Bin");

		ExportResource("Bin", BuildTarget.Android);
		ExportSceneAssetBundle("Bin", BuildTarget.Android);
		ExportAndroidProject();
	}

	[MenuItem("Build/iPhone/Build Resources")]
	static void ExportResourceMenuIPhone() {
		ExportResource("Bin", BuildTarget.iOS);
	}

	[MenuItem("Build/iPhone/Build Scenes")]
	static void ExportSceneAssetBundleMenuIPhone() {
		ExportSceneAssetBundle("Bin", BuildTarget.iOS);
	}


	[MenuItem("Build/iPhone/Full Build")]
	static void BuildAndroidMenuIPhoneAndroid() {
		if (Directory.Exists("Bin") == true) {
			Directory.Delete("Bin", true);
		}
		Directory.CreateDirectory("Bin");

		ExportResource("Bin", BuildTarget.iOS);
		ExportSceneAssetBundle("Bin", BuildTarget.iOS);
		GenericBuild(PackageScenes, "bin/" + BuildName + ".ipa", BuildTarget.iOS, BuildOptions.None);
	}

	static void GenericBuild(string[] scenes, string target_filename, BuildTarget buildTarget, BuildOptions buildOptions) {
		EditorUserBuildSettings.SwitchActiveBuildTarget(buildTarget);
		string res = BuildPipeline.BuildPlayer(scenes, target_filename, buildTarget, buildOptions);
		if (res.Length <= 0) {
			return;
		}
	}

	static void ExportAndroidProject() {
		string output = "BuildAndroid/";
		string keyaliasPass = CommandLineReader.GetCustomArgument("keyaliasPass");
		string keystorePass = CommandLineReader.GetCustomArgument("keystorePass");

		if (Directory.Exists("BuildAndroid/"+BuildName+"/assets/bin") == true) {
			Directory.Delete("BuildAndroid/"+BuildName+"/assets/bin", true);
		}
		
		GenericBuild(PackageScenes, output, BuildTarget.Android, BuildOptions.AcceptExternalModificationsToPlayer);
	}

	static List<string> TraverseTree(string root) {
		List<string> res = new List<string>();
		Stack<string> dirs = new Stack<string>(20);
		if (!Directory.Exists(root)) {
			Debug.LogError("No root directory:" + root);
			return res;
		}
		dirs.Push(root);

		while (dirs.Count > 0) {
			string currentDir = dirs.Pop();
			string[] subDirs;
			string[] files = null;

			subDirs = Directory.GetDirectories(currentDir);
			files = Directory.GetFiles(currentDir);

			foreach (string file in files) {
				FileInfo fi = new FileInfo(file);
				if (fi.Name.IndexOf(".meta") > 0) {
					continue;
				}
				res.Add(currentDir + "/" + fi.Name);
			}

			foreach (string str in subDirs) {
				dirs.Push(str);
			}
		}

		return res;
	}

	static void ExportResource(string directory, BuildTarget target) {
		directory = GetTargetDirectory(directory, target) + "resources/";
		ReadyDirectory(directory, target);

		List<Object> objects = new List<Object>();
		List<string> files = TraverseTree("Assets/Resources2Pack");
		Dictionary<string, uint> crcList = new Dictionary<string, uint>();

		foreach (string file in files) {
			Object obj = AssetDatabase.LoadAssetAtPath<Object>(file);
			objects.Add(obj);
			Object[] selection = objects.ToArray();

			string fileName = file.Substring(file.LastIndexOf("/") + 1);
			string name = fileName.Substring(0, fileName.LastIndexOf("."));
			string path = directory + name + ".unity3d";
			if (name.Length == 0) {
				continue;
			}

			if (crcList.ContainsKey(name)) {
				Debug.LogError(name + " is already exist!");
			}

			uint crc = 0;
			BuildPipeline.BuildAssetBundle(Selection.activeObject, selection, path, out crc, 
										   BuildAssetBundleOptions.CollectDependencies | 
										   BuildAssetBundleOptions.CompleteAssets, 
										   target);
			objects.Clear();			
			crcList.Add(name, crc);
		}

		string json = JsonMapper.ToJson(crcList);
		PersistenceUtil.SaveTextFile(directory + "resources.meta", json);
	}

	static void ExportSceneAssetBundle(string directory, BuildTarget target) {
		directory = GetTargetDirectory(directory, target) + "scenes/";
		ReadyDirectory(directory, target);
		Dictionary<string, uint> crcList = new Dictionary<string, uint>();

		List<string> scenes = FindRemoteScenes();
		int index = 0;
		foreach (string scene in scenes) {
			uint crc = 0;
			string fileName = scene.Substring(scene.LastIndexOf("/") + 1);
			string name = fileName.Substring(0, fileName.LastIndexOf("."));
			string path = directory + name + ".unity3d";
			string [] levels = {scene};

			foreach( string s in levels) {
				Debug.Log("build:" + s);
			}

			BuildPipeline.BuildStreamedSceneAssetBundle(levels, path, target, out crc, 0);
			crcList.Add(name, crc);
			index++;
		}

		string json = JsonMapper.ToJson(crcList);
		PersistenceUtil.SaveTextFile(directory + "scenes.meta", json);
	}

	static void ExportSceneAssetBundle(string scene, string directory, BuildTarget target) {
		directory = GetTargetDirectory(directory, target) + "scenes/";

		uint crc = 0;
		string fileName = scene.Substring(scene.LastIndexOf("/") + 1);
		string name = fileName.Substring(0, fileName.LastIndexOf("."));
		string path = directory + name + ".unity3d";
		string [] levels = {scene};

		BuildPipeline.BuildStreamedSceneAssetBundle(levels, path, target, out crc, 0);
		Debug.LogError(name + ":" + crc);
	}

	static List<string> FindRemoteScenes() {
		HashSet<string> filter = new HashSet<string>();
		foreach (string scene in PackageScenes) {
			filter.Add(scene);
		}
		foreach (string scene in BlackListScenes) {
			filter.Add(scene);
		}

		List<string> scenes = new List<string>();
		foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
			if (filter.Contains(scene.path)) {
				continue;
			}
			scenes.Add(scene.path);
		}

		return scenes;
	}

	static string GetTargetDirectory(string path, BuildTarget target) {
		if (target == BuildTarget.Android) {
			path += "/AOS";
		}
		else if (target == BuildTarget.iOS) {
			path += "/iOS";
		}

		string postfix = string.Format("{0:D3}", Version.remoteBundleVersion);
		return path + postfix + "/";
	}

	static string ReadyDirectory(string path, BuildTarget target) {
		if (Directory.Exists(path) == true) {
			Directory.Delete(path, true);
		}

		Directory.CreateDirectory(path);
		return path;
	}
}

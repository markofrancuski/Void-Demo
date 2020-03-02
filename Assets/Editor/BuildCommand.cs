using UnityEditor;
using System.Linq;
using System;

public static class BuildCommand
{
	static string GetArgument (string name)
	{
		string[] args = Environment.GetCommandLineArgs ();
		for (int i = 0; i < args.Length; i++) {
			if (args [i].Contains (name)) {
				return args [i + 1];
			}
		}
		return null;
	}

	static string[] GetEnabledScenes ()
	{
		return (
		    from scene in EditorBuildSettings.scenes
		 	where scene.enabled
		 	select scene.path
		).ToArray ();
	}

	static BuildTarget GetBuildTarget ()
	{
		string buildTargetName = GetArgument ("customBuildTarget");
		Console.WriteLine (":: Received customBuildTarget " + buildTargetName);

		if (buildTargetName.ToLower () == "android") {
			#if !UNITY_5_6_OR_NEWER
			// https://issuetracker.unity3d.com/issues/buildoptions-dot-acceptexternalmodificationstoplayer-causes-unityexception-unknown-project-type-0
			// Fixed in Unity 5.6.0
			// side effect to fix android build system:
			EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Internal;
			#endif
		}

		return ToEnum<BuildTarget> (buildTargetName, BuildTarget.NoTarget);
	}

	static string GetBuildPath ()
	{
		string buildPath = GetArgument ("customBuildPath");
		Console.WriteLine (":: Received customBuildPath " + buildPath);
		if (buildPath == "") {
			throw new Exception ("customBuildPath argument is missing");
		}
		return buildPath;
	}

	static string GetBuildName ()
	{
		string buildName = GetArgument ("customBuildName");
		Console.WriteLine (":: Received customBuildName " + buildName);
		if (buildName == "") {
			throw new Exception ("customBuildName argument is missing");
		}
		return buildName;
	}

	static string GetFixedBuildPath (BuildTarget buildTarget, string buildPath, string buildName) 
	{
		if (buildTarget.ToString().ToLower().Contains("windows")) {
			buildName = buildName + ".exe";
		} else if (buildTarget.ToString().ToLower().Contains("webgl")) {
			// webgl produces a folder with index.html inside, there is no executable name for this buildTarget
			buildName = "";
		}
		return buildPath + buildName;
	}

	static BuildOptions GetBuildOptions ()
	{
		string buildOptions = GetArgument ("customBuildOptions");
		return buildOptions == "AcceptExternalModificationsToPlayer" ? BuildOptions.AcceptExternalModificationsToPlayer : BuildOptions.None;
	}

	// https://stackoverflow.com/questions/1082532/how-to-tryparse-for-enum-value
	static TEnum ToEnum<TEnum> (this string strEnumValue, TEnum defaultValue)
	{
		if (!Enum.IsDefined (typeof(TEnum), strEnumValue)) {
			return defaultValue;
		}

		return (TEnum)Enum.Parse (typeof(TEnum), strEnumValue);
	}

	static string getEnv (string key, bool secret = false, bool verbose = true)
	{
		var env_var = Environment.GetEnvironmentVariable (key);
		if (verbose) {
			if (env_var != null) {
				if (secret) {
					Console.WriteLine (":: env['" + key + "'] set");
				} else {
					Console.WriteLine (":: env['" + key + "'] set to '" + env_var + "'");
				}
			} else {
				Console.WriteLine (":: env['" + key + "'] is null");
			}
		}
		return env_var;
	}

	static int getEnvInt (string key, bool secret = false, bool verbose = true)
	{
		var env_var = Environment.GetEnvironmentVariable (key);
		if (verbose) {
			if (env_var != null) {
				if (secret) {
					Console.WriteLine (":: env['" + key + "'] set");
				} else {
					Console.WriteLine (":: env['" + key + "'] set to '" + env_var + "'");
				}
			} else {
				Console.WriteLine (":: env['" + key + "'] is null");
			}
		}
		return int.Parse(env_var);
	}

	public class EditorSetup 
	{
		public static string AndroidSdkRoot 
		{
			get { return EditorPrefs.GetString("AndroidSdkRoot"); }
			set { EditorPrefs.SetString("AndroidSdkRoot", value); }
		}

		public static string JdkRoot 
		{
			get { return EditorPrefs.GetString("JdkPath"); }
			set { EditorPrefs.SetString("JdkPath", value); }
		}

		// This requires Unity 5.3 or later
		public static string AndroidNdkRoot 
		{
			get { return EditorPrefs.GetString("AndroidNdkRoot"); }
			set { EditorPrefs.SetString("AndroidNdkRoot", value); }
		}
	}

	static void PerformBuild ()
	{
		string buildEnv = getEnv ("BUILD_ENV");
		Console.WriteLine (":: buildEnv: " + buildEnv);
		Console.WriteLine (":: Performing build");


		if (buildEnv.ToLower () == "production") {
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");
		} else {
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "MARBLE_DEBUG;MARBLE_LOGGING");
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "MARBLE_DEBUG;MARBLE_LOGGING");
		}

        PlayerSettings.SetScriptingBackend(BuildTargetGroup.iOS, ScriptingImplementation.IL2CPP);
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
        EditorUserBuildSettings.buildAppBundle = true;

        Console.WriteLine (":: iOS Symbols: " + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS));
		Console.WriteLine (":: Android Symbols: " + PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android));

		// PlayerSettings.Android.keystoreName = getEnv ("KEYSTORE_NAME", true);
		// PlayerSettings.Android.keystorePass = getEnv ("KEYSTORE_PASS", true);
		// PlayerSettings.Android.keyaliasName = getEnv ("KEY_ALIAS_NAME", true);
		// PlayerSettings.Android.keyaliasPass = getEnv ("KEY_ALIAS_PASS", true);

		PlayerSettings.Android.bundleVersionCode = getEnvInt ("BUILD_NUMBER");
		PlayerSettings.iOS.buildNumber = getEnv ("BUILD_NUMBER");

		EditorSetup.AndroidSdkRoot = getEnv ("ANDROID_SDK_HOME");
		EditorSetup.JdkRoot = getEnv ("JAVA_HOME");
		EditorSetup.AndroidNdkRoot = getEnv ("ANDROID_NDK_HOME");

        var buildTarget = GetBuildTarget ();
		var buildPath = GetBuildPath ();
		var buildName = GetBuildName ();
		var fixedBuildPath = GetFixedBuildPath (buildTarget, buildPath, buildName);

		BuildPipeline.BuildPlayer (GetEnabledScenes (), fixedBuildPath, buildTarget, GetBuildOptions ());
		Console.WriteLine (":: Done with build");
	}
}

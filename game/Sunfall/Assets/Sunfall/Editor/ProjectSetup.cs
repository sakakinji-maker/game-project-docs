using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Sunfall.Editor
{
    [InitializeOnLoad]
    public static class ProjectSetup
    {
        const string ScenePath="Assets/Sunfall/Scenes/Prototype.unity";
        static ProjectSetup(){EditorApplication.delayCall+=EnsureScene;}
        static void EnsureScene()
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode||EditorApplication.isCompiling)return;
            if(!File.Exists(ScenePath))
            {
                Directory.CreateDirectory("Assets/Sunfall/Scenes");
                var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene,ScenePath);AssetDatabase.Refresh();
            }
            if(EditorBuildSettings.scenes.Length==0)EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
        }
        [MenuItem("Sunfall/Open Prototype")]
        public static void Open()
        {
            if(!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())return;
            EnsureScene();EditorSceneManager.OpenScene(ScenePath);
        }
        [MenuItem("Sunfall/Build Windows Prototype")]
        public static void BuildWindows()
        {
            EnsureScene();Directory.CreateDirectory("Builds/Windows");
            var result=BuildPipeline.BuildPlayer(new[]{ScenePath},"Builds/Windows/Sunfall.exe",BuildTarget.StandaloneWindows64,BuildOptions.None);
            if(result.summary.result!=BuildResult.Succeeded)throw new System.Exception("Windows build failed: "+result.summary.result);
        }
        // Batch entry point for portable QA releases. No Play mode or test run.
        [MenuItem("Sunfall/Build Windows QA (Developer Keys)")]
        public static void BuildWindowsQA()
        {
            const string output="Builds/WindowsQA";
            string version="0.1.0-qa";
            var args=System.Environment.GetCommandLineArgs();
            for(int i=0;i+1<args.Length;i++)if(args[i]=="-qaVersion")version=args[i+1];
            string previousVersion=PlayerSettings.bundleVersion;
            try
            {
                EnsureScene();Directory.CreateDirectory(output);
                PlayerSettings.bundleVersion=version;
                var report=BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                    scenes=new[]{ScenePath},locationPathName=output+"/Sunfall.exe",
                    target=BuildTarget.StandaloneWindows64,
                    options=BuildOptions.Development|BuildOptions.CompressWithLz4HC
                });
                var summary=report.summary;
                File.WriteAllText(output+"/BUILD_REPORT.txt",
                    "Version: "+version+"\nUnity: "+Application.unityVersion+
                    "\nTarget: Windows x64\nDevelopment build: yes\nResult: "+summary.result+
                    "\nErrors: "+summary.totalErrors+"\nWarnings: "+summary.totalWarnings+
                    "\nBytes: "+summary.totalSize+"\nPlay / gameplay QA: not performed\n");
                if(summary.result!=BuildResult.Succeeded)
                    throw new System.Exception("Windows QA build failed: "+summary.result);
                Debug.Log("SUNFALL_QA_BUILD_SUCCEEDED "+Path.GetFullPath(output));
            }
            finally {PlayerSettings.bundleVersion=previousVersion;}
        }
    }
}

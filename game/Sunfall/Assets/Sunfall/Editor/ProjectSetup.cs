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
    }
}

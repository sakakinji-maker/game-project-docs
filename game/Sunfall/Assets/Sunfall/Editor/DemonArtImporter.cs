using System.IO;
using UnityEditor;
using UnityEngine;

namespace Sunfall.Editor
{
    public sealed class DemonArtImporter : AssetPostprocessor
    {
        const string Root="Assets/Sunfall/Resources/Demons/";
        static readonly string[] Names={"Melee","Archer","Heavy","Stage1Boss","Stage1BossFlight","Stage2ChainWardenV1","Stage2MorgaHuskV2","Stage3SolarRegentV1","Stage3AurelEclipseV2","Stage4VarkanV1","Stage4VarkanKnightV2","Stage4VarkanHammerV2","Stage5LysseaV1","Stage5LysseaMaskBodyV2","Stage6NoctisV1","Stage6NoctisRavenV2","Stage7SolveinV1","Stage7SolveinThornV2","MeleeAttackV2","ArcherAttackV2","HeavyAttackV2","BossDashV2","BossBoltsV2","BossWaveV2","Motion/MeleeMotionV3","Motion/ArcherMotionV3","Motion/HeavyMotionV3","Motion/Stage1MotionV3","Motion/Stage2MotionV3","Motion/Stage3MotionV3","Motion/Stage4MotionV3","Motion/Stage5MotionV3","Motion/Stage6MotionV3","Motion/Stage7MotionV3"};
        [InitializeOnLoadMethod]
        static void EnsureSettings()
        {
            EditorApplication.delayCall+=()=>{
                foreach(var name in Names)
                {
                    var importer=AssetImporter.GetAtPath(Root+name+".png") as TextureImporter;
                    if(importer==null)continue;
                    if(importer.textureShape!=TextureImporterShape.Texture2D||importer.textureType!=TextureImporterType.Default||
                       importer.mipmapEnabled||!importer.alphaIsTransparency||importer.filterMode!=FilterMode.Point||
                       importer.alphaSource!=TextureImporterAlphaSource.FromInput||importer.wrapMode!=TextureWrapMode.Clamp||
                       importer.npotScale!=TextureImporterNPOTScale.None||importer.maxTextureSize!=2048||
                       importer.textureCompression!=TextureImporterCompression.Uncompressed)
                    {Configure(importer);importer.SaveAndReimport();}
                }
                CheckResources();
            };
        }
        static void Configure(TextureImporter importer)
        {
            importer.textureType=TextureImporterType.Default;importer.textureShape=TextureImporterShape.Texture2D;
            importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.alphaIsTransparency=true;
            importer.mipmapEnabled=false;importer.filterMode=FilterMode.Point;importer.wrapMode=TextureWrapMode.Clamp;
            importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;
            importer.textureCompression=TextureImporterCompression.Uncompressed;
        }
        void OnPreprocessTexture(){if(assetPath.StartsWith(Root))Configure((TextureImporter)assetImporter);}
        [MenuItem("Sunfall/Check Demon Resources (No Play)")]
        static void CheckResources()
        {
            var report=new System.Text.StringBuilder("Demon asset check (no Play): "+System.DateTime.UtcNow.ToString("O")+"\n");
            foreach(var name in Names)
            {
                var texture=Resources.Load<Texture2D>("Demons/"+name);
                report.AppendLine(name+": "+(texture==null?"MISSING":texture.width+"x"+texture.height+", "+texture.format+", "+texture.filterMode));
            }
            Directory.CreateDirectory("Temp");File.WriteAllText("Temp/DemonResourceDiagnostics.txt",report.ToString());
        }
    }
}


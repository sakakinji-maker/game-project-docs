using UnityEditor;
using UnityEngine;
using System.IO;

namespace Sunfall.Editor
{
    // Keep pixel art crisp and preserve the generated alpha on every import.
    public sealed class HeroArtImporter : AssetPostprocessor
    {
        static readonly string[] ArtNames={"HeroAtlas","HeroPortrait","Combat/Combo1V2","Combat/Combo2V2","Combat/Combo3V2","Combat/AirSlashV2","Combat/Combo1V3","Combat/Combo2V3","Combat/Combo3V3","Combat/AirSlashV3","Combat/EmberCastV3"};
        [InitializeOnLoadMethod]
        static void EnsureImportedSettings()
        {
            // The editor may have imported the PNGs before this script compiled.
            EditorApplication.delayCall+=()=>{
                foreach(var name in ArtNames)
                {
                    var importer=AssetImporter.GetAtPath("Assets/Sunfall/Resources/Hero/"+name+".png") as TextureImporter;
                    if(importer==null)continue;
                    var filter=name=="HeroPortrait"?FilterMode.Bilinear:FilterMode.Point;
                    int maxSize=name.StartsWith("Combat/")?4096:2048;
                    if(importer.mipmapEnabled||importer.filterMode!=filter||!importer.alphaIsTransparency||
                       importer.textureCompression!=TextureImporterCompression.Uncompressed||importer.npotScale!=TextureImporterNPOTScale.None||
                       importer.maxTextureSize!=maxSize||importer.wrapMode!=TextureWrapMode.Clamp||
                       importer.alphaSource!=TextureImporterAlphaSource.FromInput||importer.textureType!=TextureImporterType.Default||
                       importer.textureShape!=TextureImporterShape.Texture2D)
                        importer.SaveAndReimport();
                }
                WriteResourceDiagnostics();
            };
        }
        [MenuItem("Sunfall/Check Hero Resources (No Play)")]
        static void WriteResourceDiagnostics()
        {
            // Asset-only check: does not start play mode or spawn a game instance.
            var report=new System.Text.StringBuilder();
            report.AppendLine("Hero resource check: "+System.DateTime.UtcNow.ToString("O"));
            foreach(var name in ArtNames)
            {
                var texture=Resources.Load<Texture2D>("Hero/"+name);
                report.AppendLine(name+": "+(texture==null?"MISSING":texture.width+"x"+texture.height+", "+texture.format+", "+texture.filterMode));
                var asset=AssetDatabase.LoadMainAssetAtPath("Assets/Sunfall/Resources/Hero/"+name+".png");
                report.AppendLine("AssetDatabase: "+(asset==null?"MISSING":asset.GetType().FullName+" / "+asset.name));
            }
            report.AppendLine("Resources folder: "+AssetDatabase.IsValidFolder("Assets/Sunfall/Resources"));
            report.AppendLine("Hero folder: "+AssetDatabase.IsValidFolder("Assets/Sunfall/Resources/Hero"));
            Directory.CreateDirectory("Temp");File.WriteAllText("Temp/HeroResourceDiagnostics.txt",report.ToString());
        }
        void OnPreprocessTexture()
        {
            if(!assetPath.StartsWith("Assets/Sunfall/Resources/Hero/"))return;
            var texture=(TextureImporter)assetImporter;
            texture.textureType=TextureImporterType.Default;
            texture.textureShape=TextureImporterShape.Texture2D;
            texture.alphaSource=TextureImporterAlphaSource.FromInput;
            texture.alphaIsTransparency=true;
            texture.mipmapEnabled=false;
            texture.wrapMode=TextureWrapMode.Clamp;
            texture.npotScale=TextureImporterNPOTScale.None;
            texture.textureCompression=TextureImporterCompression.Uncompressed;
            texture.maxTextureSize=assetPath.Contains("/Combat/")?4096:2048;
            texture.filterMode=assetPath.EndsWith("HeroPortrait.png")?FilterMode.Bilinear:FilterMode.Point;
        }
    }
}

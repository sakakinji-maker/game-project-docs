using UnityEditor;
using UnityEngine;

namespace Sunfall.Editor
{
    public sealed class VillageArtImporter : AssetPostprocessor
    {
        const string Root="Assets/Sunfall/Resources/Village/";
        static readonly string[] Names={"BlacksmithV1","MerchantV1","ArinV1"};
        static void Configure(TextureImporter importer)
        {
            importer.textureType=TextureImporterType.Default;importer.textureShape=TextureImporterShape.Texture2D;
            importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.alphaIsTransparency=true;
            importer.mipmapEnabled=false;importer.filterMode=FilterMode.Point;importer.wrapMode=TextureWrapMode.Clamp;
            importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;
            importer.textureCompression=TextureImporterCompression.Uncompressed;
        }
        void OnPreprocessTexture(){if(assetPath.StartsWith(Root))Configure((TextureImporter)assetImporter);}
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
            };
        }
    }
}

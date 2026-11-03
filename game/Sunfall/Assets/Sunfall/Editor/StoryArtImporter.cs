using UnityEditor;
using UnityEngine;

namespace Sunfall.Editor
{
    public sealed class StoryArtImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if(!assetPath.StartsWith("Assets/Sunfall/Resources/Story/",System.StringComparison.Ordinal))return;
            var importer=(TextureImporter)assetImporter;
            importer.textureType=TextureImporterType.Default;importer.textureShape=TextureImporterShape.Texture2D;
            bool portrait=assetPath.Contains("/BossPortraits/")||assetPath.Contains("/ResidentPortraits/");
            importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.alphaIsTransparency=portrait;
            importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;
            importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=portrait||assetPath.Contains("/Cinematic/")?2048:1024;
        }
    }
}

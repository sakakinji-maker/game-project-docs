using UnityEditor;
using UnityEngine;

namespace Sunfall.Editor
{
    public sealed class SkillIconImporter : AssetPostprocessor
    {
        const string Root="Assets/Sunfall/Resources/SkillIcons/";
        void OnPreprocessTexture()
        {
            if(!assetPath.StartsWith(Root,System.StringComparison.Ordinal))return;
            var importer=(TextureImporter)assetImporter;
            importer.textureType=TextureImporterType.Default;
            importer.textureShape=TextureImporterShape.Texture2D;
            importer.alphaSource=TextureImporterAlphaSource.FromInput;
            importer.alphaIsTransparency=false;
            importer.mipmapEnabled=false;
            importer.filterMode=FilterMode.Bilinear;
            importer.wrapMode=TextureWrapMode.Clamp;
            importer.npotScale=TextureImporterNPOTScale.None;
            importer.maxTextureSize=256;
            importer.textureCompression=TextureImporterCompression.Uncompressed;
        }
    }
}

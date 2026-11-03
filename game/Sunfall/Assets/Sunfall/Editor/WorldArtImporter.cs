using UnityEditor;
using UnityEngine;

namespace Sunfall.Editor
{
    public sealed class WorldArtImporter : AssetPostprocessor
    {
        void OnPreprocessTexture()
        {
            if(!assetPath.StartsWith("Assets/Sunfall/Resources/World/")&&
               !assetPath.StartsWith("Assets/Sunfall/Resources/Effects/")&&
               !assetPath.StartsWith("Assets/Sunfall/Resources/Equipment/"))return;
            var importer=(TextureImporter)assetImporter;
            importer.textureType=TextureImporterType.Default;importer.textureShape=TextureImporterShape.Texture2D;
            importer.alphaSource=TextureImporterAlphaSource.FromInput;importer.alphaIsTransparency=true;
            importer.filterMode=FilterMode.Point;importer.wrapMode=TextureWrapMode.Clamp;
            importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=4096;
        }
    }
}

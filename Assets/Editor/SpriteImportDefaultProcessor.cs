using UnityEditor;
using UnityEngine;

public class SpriteImportDefaultProcessor : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        var importer = (TextureImporter)assetImporter;

        // Only act on textures being imported for the first time
        if (!System.IO.File.Exists(importer.assetPath))
            return;

        // If sprite mode already set (user-edited), skip
        if (importer.importSettingsMissing)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.textureShape = TextureImporterShape.Texture2D;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
        }
    }
}
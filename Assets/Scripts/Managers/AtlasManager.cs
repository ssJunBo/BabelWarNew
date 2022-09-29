using System.Collections.Generic;
using _GameBase;
using UnityEngine;
using UnityEngine.U2D;

namespace Managers
{
    public class AtlasManager : Singleton<AtlasManager>
    {
        private readonly Dictionary<string, SpriteAtlas> SpriteAtlasMap = new();

        public Sprite GetSprite(string atlasName, string spriteName)
        {
            if (!SpriteAtlasMap.TryGetValue(atlasName, out var spriteAtlas))
            {
                spriteAtlas = Resources.Load<SpriteAtlas>("Atlas/"+atlasName);
                SpriteAtlasMap.Add(atlasName, spriteAtlas);
            }

            var spr = spriteAtlas.GetSprite(spriteName);

            if (spr == null)
                Debug.LogError("图集不包含此图片资源！");

            return spr;
        }
    }
}
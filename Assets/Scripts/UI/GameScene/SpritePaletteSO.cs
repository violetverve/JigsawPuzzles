using System.Collections.Generic;
using UnityEngine;

namespace UI.GameScene
{

    [CreateAssetMenu(fileName = "SpritePaletteSO", menuName = "Create SO/SpritePalette")]
    public class SpritePaletteSO : ScriptableObject
    {

        [SerializeField] private List<Sprite> _sprites;
        public List<Sprite> Sprites => _sprites;
        public Sprite GetSprite(int index)
        {
            if (_sprites == null || _sprites.Count == 0)
            {
                Debug.LogWarning("Sprite list is empty or not initialized.");
                return null;
            }
            int spriteIndex = index % _sprites.Count;
            return _sprites[spriteIndex];
        }
    }   
}
#if UNITY_EDITOR
using System.Collections.Generic;
using Bonkers.BlokControl;
using RoboRyanTron.Unite2017.Variables;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.ContentGeneration
{
    [CreateAssetMenu(fileName = "Content Generator Data", menuName = "Content Generation/Content Generation Data", order = 0)]
    public class ContentGeneratorData : SerializedScriptableObject
    {
        [SerializeField][MinMaxSlider(3, 30, true)] private Vector2Int levelSizeRange = new Vector2Int(15, 30);
        [SerializeField][InlineEditor] private BoolVariable debugStatementsOn;
        [SerializeField][InlineEditor] private BoolVariable spawnContentGeneratorPrefabs;
        [SerializeField] private GameObject contentParentPrefab;

        [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.ExpandedFoldout)]
        public Dictionary<int, IndividualBlokPoolingData> BlokTypesLookUp =
            new Dictionary<int, IndividualBlokPoolingData>();

        public Vector2Int LevelSizeRange => levelSizeRange;
        //public List<IndividualBlokPoolingData> BlokTypes => blokTypes;
        public int RandomLevelSize => Random.Range(levelSizeRange.x, levelSizeRange.y);
        public BoolVariable DebugStatementsOn => debugStatementsOn;
        public BoolVariable SpawnContentGeneratorPrefabs => spawnContentGeneratorPrefabs;
        public GameObject ContentParentPrefab => contentParentPrefab;
    }
}
#endif

#if UNITY_EDITOR

using System.Collections.Generic;
using Bonkers._2DDestruction;
using Bonkers.BlokControl;
using Bonkers.Core;
using RoboRyanTron.Unite2017.Variables;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;



namespace Bonkers.ContentGeneration
{
    [CreateAssetMenu(fileName = "Generated Data System", menuName = "Content Generation/Generated Data System", order = 1)]
    public class GeneratedDataSystem : ScriptableObject
    {
        [FormerlySerializedAs("path")] [SerializeField][InlineEditor] private StringVariable contentMappingsPath;
        [SerializeField] [InlineEditor] private StringVariable blokSpawnSystemsPath;
        [SerializeField][InlineEditor] private BoolVariable recordGeneratedData;
        [SerializeField] private List<IndividualBlokPoolingData> individualBlokDatas;
        [SerializeField][ReadOnly] private List<GeneratedContentData> generatedContentDataStructure = new List<GeneratedContentData>();
        [SerializeField] [ReadOnly]
        private List<ContentMappingArrayEditorData> generatedMappingArrayEditorDatas =
            new List<ContentMappingArrayEditorData>();
        [SerializeField] [InlineEditor] private BlokSpawnSystem defaultBlokSpawnSystem;

        [Button]
        public void ClearAllContentData()
        {
            foreach (var generatedMappingArrayEditorData in generatedMappingArrayEditorDatas)
            {
                generatedMappingArrayEditorData.ClearAllSpawnerBlokChangeSubscriptions();
            }
            foreach (var generatedContentData in generatedContentDataStructure)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(generatedContentData));
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            generatedContentDataStructure.Clear();
            generatedMappingArrayEditorDatas.Clear();
        }

        [Header("Level Prefabs")]
        [SerializeField] private GameObject uiPrefab;
        [SerializeField] private GameObject playerInitializerPrefab;
        [SerializeField] private GameObject corePrefab;
        [SerializeField] private GameObject bloksPrefab;
        
        private void AddNewGeneratedContentData(ContentLevelMapping contentLevelMapping, int mapSize)
        {
            ContentLevelMapping contentLevelMappingCopy = new ContentLevelMapping(contentLevelMapping.immovableBlokMappings, contentLevelMapping.basicBlokPositionMappings,
                contentLevelMapping.bombBlokPositionMappings, contentLevelMapping.glassBlokPositionMappings, contentLevelMapping.iceBlokPositionMappings,
                contentLevelMapping.woodBlokPositionMappings);

            GeneratedContentData contentData = GeneratedContentData.CreateInstance(contentLevelMappingCopy, mapSize);
            
            generatedContentDataStructure.Add(contentData);
            AssetDatabase.CreateAsset(contentData, AssetDatabase.GenerateUniqueAssetPath(contentMappingsPath.Value + "/Content Mapping.asset"));
            AssetDatabase.SaveAssets();
            
            ContentMappingArrayEditorData contentArrayEditorData = ContentMappingArrayEditorData.CreateInstance(contentData, individualBlokDatas);
            generatedMappingArrayEditorDatas.Add(contentArrayEditorData);
            contentData.SetCorrespondingArrayEditorData(contentArrayEditorData);
            AssetDatabase.AddObjectToAsset(contentArrayEditorData, contentData);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(contentArrayEditorData));
            AssetDatabase.SaveAssets();
        }

        public void AttemptRecordNewContentData(ContentLevelMapping contentLevelMapping, int mapSize)
        {
            if (!recordGeneratedData || !recordGeneratedData.Value) return;
            AddNewGeneratedContentData(contentLevelMapping, mapSize);
        }

        public void RemoveContentData(GeneratedContentData contentData)
        {
            contentData.CorrespondingArrayEditorData.ClearAllSpawnerBlokChangeSubscriptions();
            generatedMappingArrayEditorDatas.Remove(contentData.CorrespondingArrayEditorData);
            generatedContentDataStructure.Remove(contentData);
            string path = AssetDatabase.GetAssetPath(contentData);
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
        }

        public void GenerateNewLevelScene(GeneratedContentData contentDataAsset)
        {
            Scene newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            PrefabUtility.InstantiatePrefab(uiPrefab, newScene);
            GameObject playerInitializerObject = PrefabUtility.InstantiatePrefab(playerInitializerPrefab, newScene) as GameObject;
            
            GameObject coreObject = PrefabUtility.InstantiatePrefab(corePrefab, newScene) as GameObject;
            GameObject bloksObject = PrefabUtility.InstantiatePrefab(bloksPrefab, newScene) as GameObject;
            CoreLogic coreLogic = coreObject.GetComponent<CoreLogic>();
            coreLogic.SetEnvironmentAsParent(bloksObject.transform);
            PrefabUtility.UnpackPrefabInstance(bloksObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            
            
            //Set up new player spawn positions
            playerInitializerObject.GetComponent<PlayerSpawnLevelGeneration>().GenerateNewPlayerSpawnPositions(contentDataAsset.MapSize, contentDataAsset);
            
            BlokLevelGeneration blokLevelGeneration = bloksObject.GetComponent<BlokLevelGeneration>();
            //Iterate through the entire master position mapping structure. Holds everything but the spawner bloks that were generated separately
            foreach (var positionMappingStructure in contentDataAsset.ContentLevelMapping.masterPositionMappingsStructure)
            {
                foreach (var blokMapping in positionMappingStructure.Value)
                {
                    blokLevelGeneration.SetupNewBlok(blokMapping, newScene);
                }
            }

            //Iterate through all spawner blok mappings and generate accordingly
            foreach (var spawnerBlokMapping in contentDataAsset.SpawnerBlokMappings)
            {
                blokLevelGeneration.SetupNewBlok(spawnerBlokMapping, newScene);
            }

            //Moves camera to appropriate position
            float cameraCenter = contentDataAsset.MapSize / 2f - 0.5f;
            coreLogic.SetCameraPosition(cameraCenter, cameraCenter, -contentDataAsset.MapSize);
            
            //Blok fragment generation
            bloksObject.GetComponent<Configuration>().ConfigureExplodableBloks();
            bloksObject.GetComponent<BlokFragmentMaterialModificationHelper>().AssistMatchFragmentMaterials();
            
            //Set up A Star Pathfinding Graph for pathfinding agents
            coreLogic.SetupPathfinder(contentDataAsset.MapSize);
            
            //Repopulate patrol points for the pathfinding dudes
            coreLogic.SetupNewPatrolPoints(contentDataAsset.MapSize);
            
            
            BlokSpawnSystem blokSpawnSystem = BlokSpawnSystem.CreateInstance(defaultBlokSpawnSystem.PossibleBloksToSpawn, new Vector2Int(1, 1),
                new Vector2Int(contentDataAsset.MapSize - 2, contentDataAsset.MapSize - 2), defaultBlokSpawnSystem.MinTimeBetweenSpawns,
                defaultBlokSpawnSystem.MaxTimeBetweenSpawns, defaultBlokSpawnSystem.WallLayers, 
                defaultBlokSpawnSystem.EnemyLayers, defaultBlokSpawnSystem.TweenEffect);
            
            AssetDatabase.CreateAsset(blokSpawnSystem, AssetDatabase.GenerateUniqueAssetPath(blokSpawnSystemsPath.Value + "/Blok Spawn System.asset"));
            AssetDatabase.SaveAssets();
            coreLogic.ReferenceNewBlokSpawnSystem(blokSpawnSystem);
        }
    }
}
#endif

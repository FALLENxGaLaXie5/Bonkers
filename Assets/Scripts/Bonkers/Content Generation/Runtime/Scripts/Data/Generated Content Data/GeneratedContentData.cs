﻿#if UNITY_EDITOR
using System.Collections.Generic;
using Bonkers.BlokControl;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Bonkers.ContentGeneration
{
    /// <summary>
    /// These data objects will have the mappings of blok types/positions and map sizes that we be generated by our ML Agent
    /// Data does not include boundaries or spawner bloks- boundaries are implicit since they will just depend on the map size stored here
    ///     , and so are not stored. Blok spawners will likely be manually generated somehow.
    /// </summary>
    public class GeneratedContentData : ScriptableObject
    {
        [SerializeField][ReadOnly] ContentLevelMapping contentLevelMapping;
        [SerializeField][ReadOnly] int mapSize;
        [SerializeField] [ReadOnly] private List<BlokMapping> spawnerBloks = new List<BlokMapping>();
        [SerializeField] [ReadOnly] private ContentMappingArrayEditorData correspondingArrayEditorData;

        public ContentLevelMapping ContentLevelMapping => contentLevelMapping;
        public int MapSize => mapSize;
        public ContentMappingArrayEditorData CorrespondingArrayEditorData => correspondingArrayEditorData;
        public List<BlokMapping> SpawnerBlokMappings => spawnerBloks;

        private void Init(ContentLevelMapping contentLevelMapping, int mapSize)
        {
            this.contentLevelMapping = contentLevelMapping;
            this.mapSize = mapSize;
        }

        public static GeneratedContentData CreateInstance(ContentLevelMapping contentLevelMapping, int mapSize)
        {
            var data = ScriptableObject.CreateInstance<GeneratedContentData>();
            data.Init(contentLevelMapping, mapSize);
            return data;
        }

        public void AddBlok(int x, int y, IndividualBlokPoolingData blokData) => contentLevelMapping.AddBlok(x, y, blokData);

        public void AddSpawnerBlok(int x, int y, IndividualBlokPoolingData blokData) => spawnerBloks.Add(new BlokMapping(new Vector2Int(x, y), blokData));

        public void RemoveSpawnerBlok(BlokMapping blokMapping)
        {
            List<BlokMapping> markedToRemoveSpawnerMappings = new List<BlokMapping>();
            spawnerBloks.Remove(blokMapping);
            foreach (var spawnerBlok in spawnerBloks)
            {
                if (spawnerBlok.Value.x == blokMapping.Value.x && spawnerBlok.Value.y == blokMapping.Value.y)
                {
                    markedToRemoveSpawnerMappings.Add(spawnerBlok);
                }
            }

            foreach (var markedToRemoveSpawnerMapping in markedToRemoveSpawnerMappings)
            {
                spawnerBloks.Remove(markedToRemoveSpawnerMapping);
            }
        }

        public void SetCorrespondingArrayEditorData(ContentMappingArrayEditorData correspondingArrayEditorData) => this.correspondingArrayEditorData = correspondingArrayEditorData;

        public void RemoveAnyBlokMappingAt(int positionX, int positionY)
        {
            List<BlokMapping> blokMappingsToRemove = new List<BlokMapping>();
            foreach (var spawnerBlok in spawnerBloks)
            {
                if (spawnerBlok.Value.x == positionX && spawnerBlok.Value.y == positionY)
                {
                    blokMappingsToRemove.Add(spawnerBlok);
                }
            }

            foreach (var blokMapping in blokMappingsToRemove)
            {
                spawnerBloks.Remove(blokMapping);
            }
            blokMappingsToRemove.Clear();

            foreach (var blokMappings in contentLevelMapping.masterPositionMappingsStructure)
            {
                foreach (var blokMapping in blokMappings.Value)
                {
                    if (blokMapping.Value.x == positionX && blokMapping.Value.y == positionY)
                    {
                        blokMappingsToRemove.Add(blokMapping);
                    }
                }
                
                foreach (var blokMapping in blokMappingsToRemove)
                {
                    blokMappings.Value.Remove(blokMapping);
                }
                blokMappingsToRemove.Clear();
            }
        }
    }
}
#endif
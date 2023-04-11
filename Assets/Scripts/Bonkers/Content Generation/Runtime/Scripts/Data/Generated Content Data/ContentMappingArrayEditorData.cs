#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Bonkers.BlokControl;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEditor;

namespace Bonkers.ContentGeneration
{
    public class ContentMappingArrayEditorData : SerializedScriptableObject
    {
        [SerializeField] private IndividualBlokPoolingData spawnerBlokData;
        [TableMatrix(HorizontalTitle = "Blok Cell Drawing", DrawElementMethod = "DrawColoredEnumElement", ResizableColumns = false, RowHeight = 16)]
        public BlokMapping[,] BlokCellDrawing;

        private List<IndividualBlokPoolingData> possibleBlokDatas = new List<IndividualBlokPoolingData>();

        #region Class Variables
        
        private GeneratedContentData generatedContentDataParent;

        #endregion

        public GeneratedContentData GeneratedContentData => generatedContentDataParent;
        /// <summary>
        /// Initializer that is called when creating this object.
        /// </summary>
        /// <param name="generatedContentDataParent"></param>
        private void Init(GeneratedContentData generatedContentDataParent, List<IndividualBlokPoolingData> blokDatas)
        {
            this.generatedContentDataParent = generatedContentDataParent;
            possibleBlokDatas.AddRange(blokDatas);
            InitializeBlokCellDrawingStructure(generatedContentDataParent, blokDatas[0]);
            GenerateBoundaries(generatedContentDataParent, blokDatas);
        }

        /// <summary>
        /// Adds a new blok to the block cell drawing array and the generated content structure.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="blokData"></param>
        private void AddNewRegularBlok(int x, int y, IndividualBlokPoolingData blokData)
        {
            //BlokCellDrawing[x, y] = new BlokMapping(new Vector2Int(x, y), blokData);
            BlokCellDrawing[x, y].SetPosition(new Vector2Int(x, y));
            BlokCellDrawing[x, y].SetType(blokData);
            generatedContentDataParent.AddBlok(x, y, blokData);
        }
        
        /// <summary>
        /// Generates the boundaries for the maps.
        /// </summary>
        /// <param name="generatedContentDataParent"></param>
        /// <param name="blokDatas"></param>
        private void GenerateBoundaries(GeneratedContentData generatedContentDataParent, List<IndividualBlokPoolingData> blokDatas)
        {
            for (int i = 0; i < generatedContentDataParent.MapSize; i++)
            {
                AddNewRegularBlok(i, 0, blokDatas[1]);
                AddNewRegularBlok(i, generatedContentDataParent.MapSize - 1, blokDatas[1]);
            }

            for (int i = 1; i < generatedContentDataParent.MapSize - 1; i++)
            {
                AddNewRegularBlok(0, i, blokDatas[1]);
                AddNewRegularBlok(generatedContentDataParent.MapSize - 1, i, blokDatas[1]);
            }
        }
        
        /// <summary>
        /// Initializes the blok drawing 2D array with default values and the corresponding blok values for each spot.
        /// </summary>
        /// <param name="generatedContentDataParent"></param>
        private void InitializeBlokCellDrawingStructure(GeneratedContentData generatedContentDataParent, IndividualBlokPoolingData defaultBlokData)
        {
            //BlokCellDrawing = ArrayUtilities.GetNew2DArray(30, 30, new BlokMapping(Vector2Int.zero, defaultBlokData));
            BlokCellDrawing = ArrayUtilities.GetNew2DArrayBlokMappings<BlokMapping>(30, 30, new BlokMapping(Vector2Int.zero, defaultBlokData));

            foreach (var blokMappingsList in generatedContentDataParent.ContentLevelMapping.masterPositionMappingsStructure
                .Values)
            {
                foreach (var blokMapping in blokMappingsList)
                {
                    BlokCellDrawing[blokMapping.Value.x, blokMapping.Value.y].SetPosition(new Vector2Int(blokMapping.Value.x, blokMapping.Value.y));
                    BlokCellDrawing[blokMapping.Value.x, blokMapping.Value.y].SetType(blokMapping.BlokType);
                }
            }
            
            ArrayUtilities.SubscribeBlokMappingsSpawnerChange(BlokCellDrawing, AttemptChangeToSpawnerBlok);
        }

        /// <summary>
        /// Change this blok mapping to be of type spawner blok
        /// If it already is a spwaner blok, change it to nothing
        /// </summary>
        /// <param name="blokMapping"></param>
        private void AttemptChangeToSpawnerBlok(BlokMapping blokMapping)
        {
            //TODO: Do not like hard coding the position of the spawner blok in the passed possible blok type list. Will change this eventually
            if (blokMapping.BlokType != possibleBlokDatas[possibleBlokDatas.Count - 1])
            {
                blokMapping.SetType(possibleBlokDatas[possibleBlokDatas.Count - 1]);
                generatedContentDataParent.AddSpawnerBlok(blokMapping.Value.x, blokMapping.Value.y, possibleBlokDatas[possibleBlokDatas.Count - 1]);
            }
            else
            {
                blokMapping.SetType(possibleBlokDatas[0]);
                generatedContentDataParent.RemoveSpawnerBlok(blokMapping);
            }
        }

        /// <summary>
        /// Public access to create instances of this scriptable object. Internally, initializes everything needed for
        /// the data.
        /// </summary>
        /// <param name="contentLevelMapping"></param>
        /// <param name="mapSize"></param>
        /// <returns></returns>
        public static ContentMappingArrayEditorData CreateInstance(GeneratedContentData generatedContentData, List<IndividualBlokPoolingData> blokDatas)
        {
            var data = ScriptableObject.CreateInstance<ContentMappingArrayEditorData>();
            data.Init(generatedContentData, blokDatas);
            return data;
        }

        /// <summary>
        /// Called by "DrawElementMethod" in the blok cell drawing 2D Array
        /// Rect will have the x and y values positions
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="blokMapping"></param>
        /// <returns></returns>
        private static BlokMapping DrawColoredEnumElement(Rect rect, BlokMapping blokMapping)
        {
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                blokMapping.InvokeOnChangeToSpawnerBlok();
                GUI.changed = true;
                Event.current.Use();
            }

            EditorGUI.DrawRect(rect.Padding(1), blokMapping.BlokType.BlokEditorMappingColor);

            return blokMapping;
        }

        /// <summary>
        /// Unsubscribe all cells of the blok cell structure from the spawner blok changing function IOT prevent memory leaks.
        /// </summary>
        public void ClearAllSpawnerBlokChangeSubscriptions()
        {
            ArrayUtilities.UnsubscribeBlokMappingsSpawnerChange(BlokCellDrawing, AttemptChangeToSpawnerBlok);
        }
    }
}
#endif
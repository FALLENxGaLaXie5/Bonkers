// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2026 Kybernetik //

#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Animancer.Editor
{
    /// <summary>[Editor-Only] A custom Inspector for <see cref="NamedAnimancerComponent"/>s.</summary>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/NamedAnimancerComponentEditor
    /// 
    [CustomEditor(typeof(NamedAnimancerComponent), true), CanEditMultipleObjects]
    public class NamedAnimancerComponentEditor : AnimancerComponentEditor
    {
        /************************************************************************************************************************/

        /// <inheritdoc/>
        protected override bool DoOverridePropertyGUI(string path, SerializedProperty property, GUIContent label)
        {
            switch (path)
            {
                case NamedAnimancerComponent.PlayAutomaticallyField:
                    if (ShouldShowAnimationFields())
                        DoDefaultAnimationField(property);
                    return true;

                case NamedAnimancerComponent.NamesField:
                    // Names are drawn in the Animations list.
                    return true;

                case NamedAnimancerComponent.AnimationsField:
                    if (ShouldShowAnimationFields())
                        DoAnimationsField(property);
                    return true;

                default:
                    return base.DoOverridePropertyGUI(path, property, label);
            }
        }

        /************************************************************************************************************************/

        /// <summary>
        /// The <see cref="NamedAnimancerComponent.PlayAutomatically"/> and
        /// <see cref="NamedAnimancerComponent.Animations"/> fields are only used on startup, so we don't need to show
        /// them in Play Mode after the object is already enabled.
        /// </summary>
        private bool ShouldShowAnimationFields()
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                return true;

            for (int i = 0; i < Targets.Length; i++)
                if (!Targets[i].IsGraphInitialized)
                    return true;

            return false;
        }

        /************************************************************************************************************************/

        private void DoDefaultAnimationField(SerializedProperty playAutomatically)
        {
            var area = AnimancerGUI.LayoutSingleLineRect();

            var playAutomaticallyWidth = EditorGUIUtility.labelWidth + AnimancerGUI.ToggleWidth;
            var playAutomaticallyArea = AnimancerGUI.StealFromLeft(ref area, playAutomaticallyWidth);

            using (var label = PooledGUIContent.Acquire(playAutomatically))
                EditorGUI.PropertyField(playAutomaticallyArea, playAutomatically, label);

            SerializedProperty firstAnimation;
            AnimationClip clip;

            var animations = serializedObject.FindProperty(NamedAnimancerComponent.AnimationsField);
            if (animations.arraySize > 0)
            {
                firstAnimation = animations.GetArrayElementAtIndex(0);
                clip = (AnimationClip)firstAnimation.objectReferenceValue;
                EditorGUI.BeginProperty(area, null, firstAnimation);
            }
            else
            {
                firstAnimation = null;
                clip = null;
                EditorGUI.BeginProperty(area, null, animations);
            }

            EditorGUI.BeginChangeCheck();

            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            clip = AnimancerGUI.DoObjectFieldGUI(area, GUIContent.none, clip, true);

            EditorGUI.indentLevel = indentLevel;

            if (EditorGUI.EndChangeCheck())
            {
                if (clip != null)
                {
                    if (firstAnimation == null)
                    {
                        animations.arraySize = 1;
                        firstAnimation = animations.GetArrayElementAtIndex(0);
                    }

                    firstAnimation.objectReferenceValue = clip;
                }
                else
                {
                    if (firstAnimation == null || animations.arraySize == 1)
                        animations.arraySize = 0;
                    else
                        firstAnimation.objectReferenceValue = clip;
                }
            }

            EditorGUI.EndProperty();
        }

        /************************************************************************************************************************/

        private ReorderableList _Animations;
        private SerializedProperty _Names;

        private static int _RemoveAnimationIndex;

        private void DoAnimationsField(SerializedProperty property)
        {
            GUILayout.Space(AnimancerGUI.StandardSpacing - 1);

            var serializedObject = property.serializedObject;

            _Names = serializedObject.FindProperty(NamedAnimancerComponent.NamesField);

            _Animations ??= new(serializedObject, property.Copy())
            {
                drawHeaderCallback = DrawAnimationsHeader,
                drawElementCallback = DrawAnimationElement,
                elementHeight = AnimancerGUI.LineHeight,
                onAddCallback = AddNullElement,
                onRemoveCallback = RemoveSelectedAnimation,
            };

            _RemoveAnimationIndex = -1;

            GUILayout.BeginVertical();
            _Animations.DoLayoutList();
            GUILayout.EndVertical();

            if (_RemoveAnimationIndex >= 0)
                property.DeleteArrayElementAtIndex(_RemoveAnimationIndex);

            HandleDragAndDropToAddAnimations(GUILayoutUtility.GetLastRect(), property);
        }

        /************************************************************************************************************************/

        private SerializedProperty _AnimationsArraySize;

        private void DrawAnimationsHeader(Rect area)
        {
            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth -= 6;

            area.width += 5;

            var property = _Animations.serializedProperty;
            using (var label = PooledGUIContent.Acquire(property))
            {
                var propertyLabel = EditorGUI.BeginProperty(area, label, property);

                if (_AnimationsArraySize == null)
                {
                    _AnimationsArraySize = property.Copy();
                    _AnimationsArraySize.Next(true);
                    _AnimationsArraySize.Next(true);
                }

                var oldSize = _AnimationsArraySize.intValue;
                EditorGUI.PropertyField(area, _AnimationsArraySize, propertyLabel);
                var newSize = _AnimationsArraySize.intValue;

                if (oldSize < newSize)
                    for (int i = oldSize; i < newSize; i++)
                        property.GetArrayElementAtIndex(i).objectReferenceValue = null;

                EditorGUI.EndProperty();
            }

            EditorGUIUtility.labelWidth = labelWidth;
        }

        /************************************************************************************************************************/

        private static readonly HashSet<Object>
            PreviousAnimations = new();

        private void DrawAnimationElement(Rect area, int index, bool isActive, bool isFocused)
        {
            if (index == 0)
                PreviousAnimations.Clear();

            var labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth -= 20;

            DrawNameField(ref area, index);

            var animation = _Animations.serializedProperty.GetArrayElementAtIndex(index);

            var color = GUI.color;
            var clip = animation.objectReferenceValue;
            if (clip == null || PreviousAnimations.Contains(clip))
                GUI.color = AnimancerGUI.WarningFieldColor;
            else
                PreviousAnimations.Add(clip);

            EditorGUI.BeginChangeCheck();

            EditorGUI.ObjectField(area, animation, GUIContent.none);

            if (EditorGUI.EndChangeCheck() && animation.objectReferenceValue == null)
                _RemoveAnimationIndex = index;

            GUI.color = color;
            EditorGUIUtility.labelWidth = labelWidth;
        }

        /************************************************************************************************************************/

        private void DrawNameField(ref Rect area, int index)
        {
            EditorGUI.BeginChangeCheck();

            var nameCount = _Names.arraySize;
            var name = index < nameCount
                ? _Names.GetArrayElementAtIndex(index)
                : null;

            var nameArea = AnimancerGUI.StealFromLeft(
                ref area,
                EditorGUIUtility.labelWidth,
                AnimancerGUI.StandardSpacing);

            if (name != null)
                EditorGUI.BeginProperty(nameArea, null, name);

            var nameAsset = name?.objectReferenceValue;
            var allowSceneObjects = !EditorUtility.IsPersistent(target);
            nameAsset = EditorGUI.ObjectField(
                nameArea,
                GUIContent.none,
                nameAsset,
                typeof(StringAsset),
                allowSceneObjects);

            if (name != null)
                EditorGUI.EndProperty();

            if (EditorGUI.EndChangeCheck())
            {
                if (nameAsset != null)// Set.
                {
                    ExpandWithNullsForNewItems(_Names, index + 1);

                    name ??= _Names.GetArrayElementAtIndex(index);

                    name.objectReferenceValue = nameAsset;
                }
                else// Remove.
                {
                    name.objectReferenceValue = null;
                    TrimTrailingNulls(_Names);
                }
            }
        }

        /************************************************************************************************************************/

        private static void ExpandWithNullsForNewItems(SerializedProperty array, int newCount)
        {
            var oldCount = array.arraySize;
            if (newCount <= oldCount)
                return;

            // If we expand more than 1 at a time, clear the first new item before doing the full expansion
            // so that all the new items are cleared.

            array.arraySize = oldCount + 1;
            if (newCount > oldCount + 1)
            {
                array.GetArrayElementAtIndex(oldCount).objectReferenceValue = null;
                array.arraySize = newCount;
            }
        }

        /************************************************************************************************************************/

        private static void TrimTrailingNulls(SerializedProperty array)
        {
            var oldCount = array.arraySize;
            var newCount = oldCount;
            while (newCount > 0)
            {
                if (array.GetArrayElementAtIndex(newCount - 1).objectReferenceValue != null)
                    break;

                newCount--;
            }

            if (newCount < oldCount)
                array.arraySize = newCount;
        }

        /************************************************************************************************************************/

        private static void AddNullElement(ReorderableList list)
        {
            var property = list.serializedProperty;
            var count = list.count;

            property.arraySize = count + 1;
            list.index = count;

            property.GetArrayElementAtIndex(count).objectReferenceValue = null;
        }

        /************************************************************************************************************************/

        private void RemoveSelectedAnimation(ReorderableList list)
        {
            var property = list.serializedProperty;
            var index = list.index;

            if (index < _Names.arraySize)
                RemoveElement(_Names, index);

            RemoveElement(property, index);

            if (index >= property.arraySize - 1)
                list.index = property.arraySize - 1;
        }

        private static void RemoveElement(SerializedProperty array, int index)
        {
            var element = array.GetArrayElementAtIndex(index);

            // Deleting a non-null element sets it to null, so we make sure it's null to actually remove it.
            if (element.objectReferenceValue != null)
                element.objectReferenceValue = null;

            array.DeleteArrayElementAtIndex(index);
        }

        /************************************************************************************************************************/

        private static DragAndDropHandler<object> _DropToAddAnimations;
        private static SerializedProperty _DropToAddAnimationsProperty;
        private static void HandleDragAndDropToAddAnimations(Rect area, SerializedProperty property)
        {
            _DropToAddAnimationsProperty = property;

            _DropToAddAnimations ??= (obj, isDrop) =>
            {
                using (ListPool<AnimationClip>.Instance.Acquire(out var clips))
                {
                    clips.GatherFromSource(obj);

                    var anyValid = false;

                    for (int i = 0; i < clips.Count; i++)
                    {
                        var clip = clips[i];
                        if (clip.legacy)
                            continue;

                        if (!isDrop)
                            return true;

                        anyValid = true;

                        var targetProperty = _DropToAddAnimationsProperty;
                        var index = targetProperty.arraySize;
                        targetProperty.arraySize = index + 1;
                        var element = targetProperty.GetArrayElementAtIndex(index);
                        element.objectReferenceValue = clip;
                        targetProperty.serializedObject.ApplyModifiedProperties();
                    }

                    return anyValid;
                }
            };

            _DropToAddAnimations.Handle(area);
        }

        /************************************************************************************************************************/
    }
}

#endif


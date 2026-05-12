// Animancer // https://kybernetik.com.au/animancer // Copyright 2018-2026 Kybernetik //

#if UNITY_EDITOR && UNITY_IMGUI

using UnityEditor;

namespace Animancer.Editor
{
    /// <inheritdoc/>
    /// https://kybernetik.com.au/animancer/api/Animancer.Editor/TransitionAssetReferenceDrawer
    [CustomPropertyDrawer(typeof(TransitionAssetReference), true)]
    public class TransitionAssetReferenceDrawer : TransitionDrawer
    {
    }
}

#endif


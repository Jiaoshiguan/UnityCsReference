// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace UnityEditor;

sealed class AudioContainerWindowState
{
    AudioRandomContainer m_AudioContainer;
    AudioSource m_PreviewAudioSource;
    SerializedObject m_SerializedObject;
    VisualElement m_ResourceTrackerElement;
    AudioSource m_TrackedSource;

    // Need this flag to track transport state changes immediately, as there could be a
    // one-frame delay to get the correct value from AudioSource.isContainerPlaying.
    bool m_IsPlayingOrPausedLocalFlag;

    internal event EventHandler TargetChanged;
    internal event EventHandler TransportStateChanged;
    internal event EventHandler EditorPauseStateChanged;

    internal AudioContainerWindowState()
    {
        EditorApplication.playModeStateChanged += OnEditorPlayModeStateChanged;
        EditorApplication.pauseStateChanged += OnEditorPauseStateChanged;
        Selection.selectionChanged += OnSelectionChanged;
    }

    internal AudioRandomContainer AudioContainer
    {
        get
        {
            if (m_AudioContainer == null && m_TrackedSource == null)
                UpdateTarget();

            return m_AudioContainer;
        }
    }

    internal SerializedObject SerializedObject
    {
        get
        {
            if (m_AudioContainer != null && (m_SerializedObject == null || m_SerializedObject.targetObject != m_AudioContainer))
                m_SerializedObject = new SerializedObject(m_AudioContainer);

            return m_SerializedObject;
        }
    }

    internal string TargetPath { get; private set; }

    internal void Reset()
    {
        Stop();
        m_AudioContainer = null;
        m_SerializedObject = null;
        m_IsPlayingOrPausedLocalFlag = false;
        TargetPath = null;
    }

    internal VisualElement GetResourceTrackerElement()
    {
        m_ResourceTrackerElement = new VisualElement();
        return m_ResourceTrackerElement;
    }


    internal void OnDestroy()
    {
        Stop();

        if (m_PreviewAudioSource != null)
            Object.DestroyImmediate(m_PreviewAudioSource.gameObject);

        EditorApplication.playModeStateChanged -= OnEditorPlayModeStateChanged;
        Selection.selectionChanged -= OnSelectionChanged;
        EditorApplication.pauseStateChanged -= OnEditorPauseStateChanged;
    }

    /// <summary>
    /// Updates the current target based on the currently selected object in the editor.
    /// </summary>
    internal void UpdateTarget()
    {
        AudioRandomContainer newTarget = null;
        AudioSource audioSource = null;
        var selectedObject = Selection.activeObject;

        // The logic below deals with selecting our new ARC target, whatever we set m_AudioContainer to below will be
        // used by AudioContainerWindow to display the ARC if the target is valid or a day0 state if the target is null.
        // If the selection is a GameObject, we always want to swap the target, a user selecting GameObjects in the
        // scene hierarchy should always see what ARC is on a particular object, this includes the scenario of not
        // having an AudioSource and the value of the resource property on an AudioSource being null/not an ARC.
        // If the selected object is not a GameObject, we only swap targets if it is an ARC - meaning if you are
        // selecting objects in the project browser it holds on to the last ARC selected.

        if (selectedObject != null)
        {
            if (selectedObject is GameObject go)
            {
                audioSource = go.GetComponent<AudioSource>();

                if (audioSource != null)
                {
                    newTarget = audioSource.resource as AudioRandomContainer;
                }
            }
            else
            {
                if (selectedObject is AudioRandomContainer container)
                {
                    newTarget = container;
                }
                else
                {
                    newTarget = m_AudioContainer;
                }
            }
        }
        else
        {
            newTarget = m_AudioContainer;
        }

        if (m_TrackedSource == audioSource && m_AudioContainer == newTarget)
            return;

        Reset();

        m_TrackedSource = audioSource;
        m_AudioContainer = newTarget;

        if (m_AudioContainer != null)
            TargetPath = AssetDatabase.GetAssetPath(m_AudioContainer);

        TargetChanged?.Invoke(this, EventArgs.Empty);

        if (m_TrackedSource == null)
            return;

        var trackedSourceSO = new SerializedObject(m_TrackedSource);
        var trackedSourceResourceProperty = trackedSourceSO.FindProperty("m_Resource");
        m_ResourceTrackerElement.TrackPropertyValue(trackedSourceResourceProperty, OnResourceChanged);
    }
    void OnResourceChanged(SerializedProperty property)
    {
        var container = property.objectReferenceValue as AudioRandomContainer;

        if (m_AudioContainer == container)
            return;

        Reset();
        m_AudioContainer = container;

        if (m_AudioContainer != null)
            TargetPath = AssetDatabase.GetAssetPath(m_AudioContainer);

        TargetChanged?.Invoke(this, EventArgs.Empty);
    }

    internal void Play()
    {
        if (IsPlayingOrPaused() || !IsReadyToPlay())
            return;

        if (m_PreviewAudioSource == null)
        {
            // Create a hidden game object in the scene with an AudioSource for editor previewing purposes.
            // The preview object is created on play and destroyed on stop.
            // This means that this object is a hidden part of the user's scene during play/pause.
            var gameObject = new GameObject
            {
                name = "PreviewAudioSource595651",

                hideFlags = HideFlags.HideInHierarchy | HideFlags.DontSaveInEditor | HideFlags.DontSaveInBuild
            };

            m_PreviewAudioSource = gameObject.AddComponent<AudioSource>();
            m_PreviewAudioSource.playOnAwake = false;
        }

        m_PreviewAudioSource.resource = m_AudioContainer;
        m_PreviewAudioSource.Play();
        m_IsPlayingOrPausedLocalFlag = true;
        TransportStateChanged?.Invoke(this, EventArgs.Empty);
        EditorApplication.update += OnEditorApplicationUpdate;
    }

    internal void Stop()
    {
        if (!IsPlayingOrPaused())
            return;

        m_PreviewAudioSource.Stop();
        m_PreviewAudioSource.resource = null;
        m_IsPlayingOrPausedLocalFlag = false;
        TransportStateChanged?.Invoke(this, EventArgs.Empty);
        EditorApplication.update -= OnEditorApplicationUpdate;
    }

    internal void Skip()
    {
        if (!IsPlayingOrPaused())
            return;

        m_PreviewAudioSource.SkipToNextElementIfHasContainer();
    }

    internal bool IsPlayingOrPaused()
    {
        return m_IsPlayingOrPausedLocalFlag || (m_PreviewAudioSource != null && m_PreviewAudioSource.isContainerPlaying);
    }

    /// <summary>
    /// Checks if the window has a current target with at least one enabled audio clip assigned.
    /// </summary>
    /// <returns>Whether or not there are valid audio clips to play</returns>
    internal bool IsReadyToPlay()
    {
        if (m_AudioContainer == null)
            return false;

        var elements = m_AudioContainer.elements;

        for (var i = 0; i < elements.Length; ++i)
            if (elements[i] != null && elements[i].audioClip != null && elements[i].enabled)
                return true;

        return false;
    }

    internal ActivePlayable[] GetActivePlayables()
    {
        return IsPlayingOrPaused() ? m_PreviewAudioSource.containerActivePlayables : null;
    }

    internal float GetMeterValue()
    {
        return m_PreviewAudioSource.GetAudioRandomContainerRuntimeMeterValue();
    }

    internal bool IsDirty()
    {
        return m_AudioContainer != null && EditorUtility.IsDirty(m_AudioContainer);
    }

    void OnEditorApplicationUpdate()
    {
        if (m_PreviewAudioSource != null && m_PreviewAudioSource.isContainerPlaying)
            return;

        m_IsPlayingOrPausedLocalFlag = false;
        TransportStateChanged?.Invoke(this, EventArgs.Empty);
        EditorApplication.update -= OnEditorApplicationUpdate;
    }

    void OnEditorPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state is PlayModeStateChange.ExitingEditMode or PlayModeStateChange.ExitingPlayMode)
        {
            Stop();

            if (m_PreviewAudioSource != null)
            {
                Object.DestroyImmediate(m_PreviewAudioSource.gameObject);
            }
        }
    }

    void OnEditorPauseStateChanged(PauseState state)
    {
        EditorPauseStateChanged?.Invoke(this, EventArgs.Empty);
    }

    void OnSelectionChanged()
    {
        UpdateTarget();
    }
}

// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityEditor.UIElements.Samples
{
    internal class MinMaxSliderSnippet : ElementSnippet<MinMaxSliderSnippet>
    {
        internal override void Apply(VisualElement container)
        {
            #region sample
            // Get a reference to the field from UXML and assign a value to it.
            var uxmlField = container.Q<MinMaxSlider>("the-uxml-field");
            uxmlField.value = new Vector2(10, 12);

            // Create a new field, disable it, and give it a style class.
            var csharpField = new MinMaxSlider("C# Field", 0, 20, -10, 40);
            csharpField.SetEnabled(false);
            csharpField.AddToClassList("some-styled-field");
            csharpField.value = uxmlField.value;
            container.Add(csharpField);

            // Mirror the value of the UXML field into the C# field.
            uxmlField.RegisterCallback<ChangeEvent<Vector2>>((evt) =>
            {
                csharpField.value = evt.newValue;
            });
            #endregion
        }
    }
}

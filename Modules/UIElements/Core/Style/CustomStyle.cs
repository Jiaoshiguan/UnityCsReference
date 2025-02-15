// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;

namespace UnityEngine.UIElements
{
    // 研究一下是什么东西，该怎么用。试试能不能与 [hover音乐]、[hover事件埋点] 所关联

    /// <summary>
    /// Define a custom style property for an element to be retrieved with <see cref="CustomStyleResolvedEvent"/>.
    /// </summary>
    public struct CustomStyleProperty<T> : IEquatable<CustomStyleProperty<T>>
    {
        /// <summary>
        /// Name of the custom property.
        /// </summary>
        /// <remarks>
        /// Custom style property name must start with a -- prefix.
        /// </remarks>
        public string name { get; private set; }

        /// <summary>
        /// Creates custom property from a string.
        /// </summary>
        /// <param name="propertyName">Name of the property. Must start with a -- prefix.</param>
        public CustomStyleProperty(string propertyName)
        {
            if (!String.IsNullOrEmpty(propertyName) && !propertyName.StartsWith("--"))
                throw new ArgumentException($"Custom style property \"{propertyName}\" must start with \"--\" prefix.");

            name = propertyName;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CustomStyleProperty<T>))
                return false;

            return Equals((CustomStyleProperty<T>)obj);
        }

        /// <undoc/>
        public bool Equals(CustomStyleProperty<T> other)
        {
            return name == other.name;
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        /// <undoc/>
        public static bool operator==(CustomStyleProperty<T> a, CustomStyleProperty<T> b)
        {
            return a.Equals(b);
        }

        /// <undoc/>
        public static bool operator!=(CustomStyleProperty<T> a, CustomStyleProperty<T> b)
        {
            return !(a == b);
        }
    }

    /// <summary>
    /// This interface exposes methods to read custom style properties applied from USS files to visual elements.
    /// </summary>
    public interface ICustomStyle
    {
        /// <summary>
        /// Gets the value associated with the specified <see cref="CustomStyleProperty{T}"/>.
        /// </summary>
        /// <returns>True if the property is found, false if not.</returns>
        bool TryGetValue(CustomStyleProperty<float> property, out float value);
        /// <summary>
        /// Gets the value associated with the specified <see cref="CustomStyleProperty{T}"/>.
        /// </summary>
        /// <returns>True if the property is found, false if not.</returns>
        bool TryGetValue(CustomStyleProperty<int> property, out int value);
        /// <summary>
        /// Gets the value associated with the specified <see cref="CustomStyleProperty{T}"/>.
        /// </summary>
        /// <returns>True if the property is found, false if not.</returns>
        bool TryGetValue(CustomStyleProperty<bool> property, out bool value);
        /// <summary>
        /// Gets the value associated with the specified <see cref="CustomStyleProperty{T}"/>.
        /// </summary>
        /// <returns>True if the property is found, false if not.</returns>
        bool TryGetValue(CustomStyleProperty<Color> property, out Color value);
        /// <summary>
        /// Gets the value associated with the specified <see cref="CustomStyleProperty{T}"/>.
        /// </summary>
        /// <returns>True if the property is found, false if not.</returns>
        bool TryGetValue(CustomStyleProperty<Texture2D> property, out Texture2D value);
        /// <summary>
        /// Gets the value associated with the specified <see cref="CustomStyleProperty{T}"/>.
        /// </summary>
        /// <returns>True if the property is found, false if not.</returns>
        bool TryGetValue(CustomStyleProperty<Sprite> property, out Sprite value);
        /// <summary>
        /// Gets the value associated with the specified <see cref="CustomStyleProperty{T}"/>.
        /// </summary>
        /// <returns>True if the property is found, false if not.</returns>
        bool TryGetValue(CustomStyleProperty<VectorImage> property, out VectorImage value);
        /// <summary>
        /// Gets the value associated with the specified <see cref="CustomStyleProperty{T}"/>.
        /// </summary>
        /// <returns>True if the property is found, false if not.</returns>
        bool TryGetValue<T>(CustomStyleProperty<T> property, out T value) where T : Object;
        /// <summary>
        /// Gets the value associated with the specified <see cref="CustomStyleProperty{T}"/>.
        /// </summary>
        /// <returns>True if the property is found, false if not.</returns>
        bool TryGetValue(CustomStyleProperty<string> property, out string value);
    }
}

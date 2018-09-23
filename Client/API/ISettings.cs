using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public delegate void OnSettingsChanged<T>(T newValue);

    /// <summary>
    /// Setting interface.
    /// </summary>
    /// <typeparam name="T">The type of the given setting.</typeparam>
    public interface ISettingsClass<T>
    {
        /// <summary>
        /// Gets and sets the setting with the provided key.
        /// </summary>
        /// <param name="key">The key to the setting.</param>
        /// <returns>The current value of the given key.</returns>
        T this[string key] { get; set; }

        /// <summary>
        /// Does this setting exist?
        /// </summary>
        /// <param name="key">The key to check on a setting.</param>
        /// <returns>Whether the setting exists or not.</returns>
        bool Exists(string key);

        /// <summary>
        /// Setting watcher for changes in values for a given setting.
        /// </summary>
        /// <param name="key">Key to the setting</param>
        /// <param name="OnValueChanged">the OnValueChanged event fired.</param>
        void AddWatcher(string key, OnSettingsChanged<T> OnValueChanged);
    }

    /// <summary>
    /// Setting interface for multiple settings.
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Setting collection for boolean values.
        /// </summary>
        ISettingsClass<bool> Bool { get; }

        /// <summary>
        /// Setting collection for integer values.
        /// </summary>
        ISettingsClass<int> Int { get; }

        /// <summary>
        /// Setting collection for float values.
        /// </summary>
        ISettingsClass<float> Float { get; }

        /// <summary>
        /// Setting collection for string values.
        /// </summary>
        ISettingsClass<string> String { get; }

        /// <summary>
        /// Setting watcher for changes in values for a given setting.
        /// </summary>
        /// <typeparam name="T">The type of the value that was changed.</typeparam>
        /// <param name="key">Key to the setting</param>
        /// <param name="OnValueChanged">the OnValueChanged event fired.</param>
        void AddWatcher<T>(string key, OnSettingsChanged<T> OnValueChanged);
    }
}

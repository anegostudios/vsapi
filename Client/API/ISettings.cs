using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vintagestory.API.Client
{
    public delegate void OnSettingsChanged<T>(T newValue);

    public interface ISettingsClasss<T>
    {
        T this[string key] { get; set; }

        bool Exists(string key);

        void AddWatcher(string key, OnSettingsChanged<T> OnValueChanged);
    }

    public interface ISettings
    {
        ISettingsClasss<bool> Bool { get; }
        ISettingsClasss<int> Int { get; }
        ISettingsClasss<float> Float { get; }
        ISettingsClasss<string> String { get; }

        void AddWatcher<T>(string key, OnSettingsChanged<T> OnValueChanged);
    }
}

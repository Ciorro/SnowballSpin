using SnowballSpin.Saving.Converters;
using System;
using System.Collections.Generic;
using System.IO;

namespace SnowballSpin.Saving
{
    class Save
    {
        public bool SaveOnSet { get; set; }
        public string Path { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;

        private Dictionary<Type, ISaveValueTypeConverter> _converters =
            new Dictionary<Type, ISaveValueTypeConverter>()
            {
                { typeof(int), new IntConverter() },
                { typeof(string), new StringConverter() },
                { typeof(bool), new BoolConverter() }
            };

        private Dictionary<string, string> _values =
            new Dictionary<string, string>();

        public void Read()
        {
            string[] lines = File.ReadAllLines(System.IO.Path.Combine(Path, Filename));

            foreach (var line in lines)
            {
                string name = line.Split('=')[0].Trim();
                string value = line.Split('=')[1].Trim();

                _values.Add(name, value);
            }
        }

        public void Write()
        {
            List<string> lines = new List<string>();

            foreach (var value in _values)
            {
                lines.Add($"{value.Key}={value.Value}");
            }

            File.WriteAllLines(System.IO.Path.Combine(Path, Filename), lines.ToArray());
        }

        public T Get<T>(string name, T defaultValue = default, bool createOnFail = false)
        {
            if (!_values.ContainsKey(name))
            {
                if (createOnFail)
                {
                    Set(name, defaultValue.ToString());
                }

                return defaultValue;
            }

            if (_converters.ContainsKey(typeof(T)))
            {
                return (T)_converters[typeof(T)].Convert(_values[name]);
            }

            return default;
        }

        public void Set(string name, string value)
        {
            if (_values.ContainsKey(name))
            {
                _values[name] = value;
            }
            else
            {
                _values.Add(name, value);
            }

            if (SaveOnSet)
            {
                Write();
            }
        }
    }
}

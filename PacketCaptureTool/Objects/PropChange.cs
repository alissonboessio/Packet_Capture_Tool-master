using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace PacketCaptureTool.Objects
{
    public abstract class PropChange : INotifyPropertyChanged
    {
        public PropChange()
        {
            campos_trim = [];
        }
        public virtual void Clear()
        {
            var k = valores.Keys.ToList();
            this.valores.Clear();
            k.ForEach(k1 => OnPropertyChanged(k1));
        }
        public string[] campos_trim { get; set; }
        internal Dictionary<string, object?> valores = new Dictionary<string, object?>();
        public void SetValues(Dictionary<string, object?> valores, bool clear_originais = false)
        {
            if (valores is null) return;

            var keys = this.valores.Keys.ToList();
            if (clear_originais)
            {
                this.valores.Clear();
            }
            foreach (var k in valores.Keys)
            {
                if (!keys.Contains(k)) keys.Add(k);
            }

            foreach (var v in valores)
            {
                this.valores[v.Key] = v.Value;
            }
            foreach (var k in keys)
            {
                this.OnPropertyChanged(k);
            }
        }
        public List<string> Keys
        {
            get => valores.Keys.ToList();
        }
        public bool ContainsValue(string key)
        {
            return valores.ContainsKey(key);
        }
        public T Get<T>([CallerMemberName] string campo = "")
        {
            if (valores.ContainsKey(campo))
                return (T)valores[campo];
            return default;
        }
        public void Set(object? valor, [CallerMemberName] string campo = "")
        {
            if (campo == null || campo.Length < 1)
                return;

            if (campos_trim != null && campos_trim.Contains(campo) && valor != null)
            {
                valor = $"{valor}".Trim();
            }
            valores[campo] = valor;
            OnPropertyChanged(campo);
        }
        public void SetProp(object? valor, [CallerMemberName] string campo = "")
        {
            var p = GetType().GetProperty(campo);
            p?.SetValue(this, valor, null);
        }
        public object? this[string campo]
        {
            get
            {
                return Get<object?>(campo);
            }
            set
            {
                Set(value, campo);
            }
        }

        private Dictionary<string, List<string>> list_prop_on = new Dictionary<string, List<string>>();
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string campo = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(campo));
            if (list_prop_on.ContainsKey(campo))
            {
                foreach (string key in list_prop_on[campo])
                {
                    this.OnPropertyChanged(key);
                }
            }
        }
    }
}


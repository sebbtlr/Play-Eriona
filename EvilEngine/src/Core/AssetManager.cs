using System;
using System.Collections;
using System.Collections.Generic;

namespace EvilEngine.Core
{
    public sealed class AssetManager : IDisposable
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public bool Disposed = false;

        private readonly Hashtable _resourcesList;

        public AssetManager()
        {
            _resourcesList = new Hashtable();
        }

        public T Get<T>(string id) where T : class, IDisposable
        {
            if (_resourcesList.ContainsKey(id))
            {
                return _resourcesList[id] as T;
            }
            return default(T);
        }

        public string GetId<T>(T asset) where T : class, IDisposable
        {
            foreach (KeyValuePair<string, object> element in _resourcesList.Values)
            {
                T value = element.Value as T;
                if (value != null && value == asset)
                {
                    return element.Key;
                }
            }
            return String.Empty;
        }

        public void Add<T>(string id, T asset) where T : class, IDisposable
        {
            if (!_resourcesList.ContainsKey(id))
            {
                _resourcesList.Add(id, asset);
            }
        }

        public void LoadAndAdd<T>(string id) where T : class, IDisposable
        {
            try
            {
                Add(id, GameCore.Instance.Content.Load<T>(id));
            }
            catch (Exception e)
            {
                // -TODO-: Replace by log
                Console.WriteLine($"{e} \'{id}\'");
            }
        }

        public void Remove<T>(string id) where T : class, IDisposable
        {
            if (_resourcesList.ContainsKey(id))
            {
                var o = _resourcesList[id] as T;
                o?.Dispose();
                _resourcesList.Remove(id);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void Dispose(bool disposing)
        {
            if (Disposed)
                return;

            if (disposing)
            {
                foreach (IDisposable element in _resourcesList.Values)
                {
                    element.Dispose();
                }
                _resourcesList.Clear();
            }
            
            Disposed = true;
        }
    }
}
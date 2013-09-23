using System.Collections.Generic;
using System.Linq;

namespace HelperLib.Caching {
    /// <summary>
    /// An alternative way for caching objects based on their usage (access).
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <seealso cref="http://msdn.microsoft.com/en-us/library/system.runtime.caching.aspx"/>
    public class UsageCache<K, V> {
        public delegate V Create(K key);

        /// <summary>
        /// key -> (value, isUsed)
        /// </summary>
        IDictionary<K, UseValue<V>> cache = new Dictionary<K, UseValue<V>>();
        protected Create create;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="create">Factory method</param>
        public UsageCache(Create create) {
            this.create = create;
        }

        //public abstract V Create(K key); // using delegate instead

        /// <summary>
        /// Get Object from cache or create a new instance if not found.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public V Get(K key) {
            if (cache.ContainsKey(key)) {
                //cache[key].isUsed = true;
                //return cache[key].v;
                UseValue<V> value = cache[key];
                value.isUsed = true;
                return value.v;
            } else {
                var v = create(key);
                cache[key] = new UseValue<V>(v);
                return v;
            }
        }

        /// <summary>
        /// Reset "used" flag of 
        /// </summary>
        public void ResetUsed() {
            UseValue<V> value;
            foreach (K key in cache.Keys) {
                value = cache[key];
                value.isUsed = false;
            }
        }

        /// <summary>
        /// Remove unused values from the cache.
        /// </summary>
        /// <returns>true if at least one element has bean removed from the cache</returns>
        public bool ClearUnUsed() {
            bool isChanged = false;
            KeyValuePair<K, UseValue<V>> e;
            for (int i = cache.Count-1; i >= 0; i--) {
                e = cache.ElementAt(i);
                if (!e.Value.isUsed) {
                    isChanged = true;
                    cache.Remove(e.Key); // TODO remove directly the item at the current index
                }
            }
            return isChanged;
        }

        /// <summary>
        /// Clear object cache.
        /// </summary>
        public void Clear() {
            cache.Clear();
        }
    }

    /// <summary>
    /// The value type of UsageCache
    /// </summary>
    /// <typeparam name="V"></typeparam>
    struct UseValue<V> {
        public V v;
        public bool isUsed;
        public UseValue(V v) {
            this.v = v;
            isUsed = true;
        }
    }
}

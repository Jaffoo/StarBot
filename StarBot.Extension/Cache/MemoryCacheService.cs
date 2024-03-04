using Microsoft.Extensions.Caching.Memory;

namespace StarBot.Extension
{
    /// <summary>
    /// dotnet 自带内存缓存
    /// </summary>
    /// <remarks>
    /// 构造函数
    /// </remarks>
    /// <param name="cache"></param>
    public class MemoryCacheService(IMemoryCache cache) : ICacheService
    {
        protected IMemoryCache _cache = cache;

        /// <summary>
        /// 是否存在此缓存
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            return _cache.TryGetValue<object>(key, out _);
        }

        /// <summary>
        /// 取得缓存数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetCache<T>(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            bool b = _cache.TryGetValue(key, out T? v);

            //判断是否存在
            if (b)
            {
                return v!;
            }
            else
            {
                return default!;
            }
        }

        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetCache(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            ArgumentNullException.ThrowIfNull(value);
            if (_cache.TryGetValue(key, out _))
                _cache.Remove(key);
            _cache.Set(key, value);
        }

        /// <summary>
        /// 设置缓存,绝对过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationMinute">间隔分钟</param>
        /// MemoryCacheService.Default.SetCache("test", "RedisCache works!", 30);
        public void SetCache(string key, object value, double expirationMinute)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            ArgumentNullException.ThrowIfNull(value);
            if (_cache.TryGetValue(key, out _))
                _cache.Remove(key);
            DateTime now = DateTime.Now;
            TimeSpan ts = now.AddMinutes(expirationMinute) - DateTime.Now;
            _cache.Set(key, value, ts);
        }

        /// <summary>
        /// 设置缓存,绝对过期
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expirationTime">DateTimeOffset 结束时间</param>
        /// MemoryCacheService.Default.SetCache("test", "RedisCache works!", DateTimeOffset.Now.AddSeconds(30));
        public void SetCache(string key, object value, DateTimeOffset expirationTime)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            ArgumentNullException.ThrowIfNull(value);
            if (_cache.TryGetValue(key, out _))
                _cache.Remove(key);

            _cache.Set(key, value, expirationTime);
        }

        /// <summary>
        /// 设置缓存,相对过期时间
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="t"></param>
        /// MemoryCacheService.Default.SetCache("test", "MemoryCache works!",TimeSpan.FromSeconds(30));
        public void SetSlidingCache(string key, object value, TimeSpan t)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            ArgumentNullException.ThrowIfNull(value);
            if (_cache.TryGetValue(key, out _))
                _cache.Remove(key);

            _cache.Set(key, value, new MemoryCacheEntryOptions()
            {
                SlidingExpiration = t
            });
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        /// <param name="key"></param>
        public void RemoveCache(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            _cache.Remove(key);
        }

        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            _cache?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

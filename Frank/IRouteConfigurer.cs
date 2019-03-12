using System;

namespace Frank
{
    public interface IRouteConfigurer
    {
        void Get(string path, Func<int> func);
    }
}
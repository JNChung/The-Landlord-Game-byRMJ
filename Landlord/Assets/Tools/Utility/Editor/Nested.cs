using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ron.Tools
{
    public static class Nested
    {
        public static void _(Action begin, Action content, Action end)
        {
            begin();
            content();
            end();
        }
        public static T _<T>(Func<T> func, Action content, Action end)
        {
            T r = func();
            content();
            end();
            return r;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace UOKO.SSO.Server.Utils
{
    public class JsonConfigHelper<T>
    {
        public static T Load(string fileName)
        {
            T t = default(T);
            if (File.Exists(fileName))
            {
                t = JsonConvert.DeserializeObject<T>(File.ReadAllText(fileName));
            }      
            return t;
        }
    }
}
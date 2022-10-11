//css_nuget Newtonsoft.Json

using System;
using Newtonsoft.Json;

public static class Util
{
    public static dynamic FromJson(string json)
    {
        return JsonConvert.DeserializeObject(json);
    }
}

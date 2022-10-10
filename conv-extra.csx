#! "netcoreapp1.1"
#r "nuget:Newtonsoft.Json,13.0.0.0"
#r "nuget:SimpleExec,11.0.0"

using System;
using Newtonsoft.Json.Linq;
using static SimpleExec.Command;
using System.Xml.Linq;

Console.WriteLine("Version: {0}", Environment.Version.ToString());
Run("ls.exe", new[] { "-l", "-t", "-r" });
var json = File.ReadAllText("extra.json");
Console.WriteLine(json);
//JObject jsonObj = JObject.Parse(json);
var d = ((dynamic)JObject.Parse(json)).software;
var o = (JObject)d;
XElement root = new XElement("root");
foreach(var x in o)
{
    Console.WriteLine(x.Key);
    Console.WriteLine(x.Value);
    var e = new XElement("item", new XAttribute("name", (string)x.Key),
     new XElement("version", (string)x.Value["version"]),
     new XElement("ext", (string)x.Value["ext"]),
     new XElement("path", (string)x.Value["path"]),
     new XElement("url", (string)x.Value["url"])
     );
    Console.WriteLine(e);
    root.Add(e);
}
Console.WriteLine(root);
string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + root.ToString(SaveOptions.None);
Console.WriteLine(xml);
File.WriteAllText("extra.xml", xml);

#! "netcoreapp1.1"
#r "nuget:Newtonsoft.Json,13.0.0.0"
//#r "nuget:JavaCommons,2022.1011.215.36"
#r "nuget:SimpleExec,11.0.0"

using System;
// JObjectを使用するのに必要です
using Newtonsoft.Json.Linq;
//using JavaCommons;
using static SimpleExec.Command;
using System.Xml.Linq;

Console.WriteLine("Version: {0}", Environment.Version.ToString());
Console.WriteLine("Hello!");
Run("ls.exe", new[] { "-l", "-t", "-r" });
var json = File.ReadAllText("programs.json");
Console.WriteLine(json);
//JObject jsonObj = JObject.Parse(json);
var ary = JArray.Parse(json);
Console.WriteLine(ary.Count);
XElement root = new XElement("root");
foreach(var x in ary)
{
    Console.WriteLine(x);
    var name = (string)x[0];
    var path = (string)x[1];
    if (path == null) path = "";
    XElement e = new XElement("item", new XAttribute("name", name), new XAttribute("path", path));
    Console.WriteLine(e);
    root.Add(e);
}
Console.WriteLine(root);
string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" + root.ToString(SaveOptions.None);
Console.WriteLine(xml);
File.WriteAllText("programs.xml", xml);

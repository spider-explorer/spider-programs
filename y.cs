//css_nuget dotnetzip
//css_nuget Newtonsoft.Json
//css_import funcs.cs

//using Ionic.Zip;
using System;
using Newtonsoft.Json;

/*
using(ZipFile zip = new ZipFile())
{
	zip.AddFile("extra.json");
	zip.AddFile("programs.json");
	zip.Save("y.zip");
}
*/

var ary = Util.FromJson("[1, 2, 3]");
Console.WriteLine(ary);

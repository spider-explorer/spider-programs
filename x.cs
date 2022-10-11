//css_nuget dotnetzip
using Ionic.Zip;
using System;
 
class Script
{
    static public void Main()
    {
        using(ZipFile zip = new ZipFile())
        {
            zip.AddFile("extra.json");
            zip.AddFile("programs.json");
            zip.Save("x.zip");
        }
    }
}

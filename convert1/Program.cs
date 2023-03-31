using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using Microsoft.Extensions.Logging.Abstractions;

public static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        try
        {
            Print(Environment.Version.ToString(), "Runtime Version");
            Print(Assembly.GetExecutingAssembly().GetName().Version.ToString(), "Assembly Version");
            Print(args, "args");
            var engine = JavaScript.CreateEngine();
            engine.Execute("Util.Print(123.45);");
            engine.Execute("function add(a, b) { return a + b; }");
            var sum = engine.Invoke("add", 11, 22);
            Print(sum);
            Print(Environment.CurrentDirectory);
            //Environment.CurrentDirectory = @"C:\Users\Public\root\.repo\base2\work\spider-programs";
            //RunCommand("gh", "auth", "login", "--hostname", "github.com", "--git-protocol", "https", "--web");
            string buildDir = Environment.CurrentDirectory + "\\.build";
            Log(buildDir);
            Dirs.Prepare(buildDir);

            XElement root = new XElement("root");

            var extra = ReadTextFileAsJson("extra.json")["software"];
            Log(extra);
            foreach (var x in extra)
            {
                Log(x.Name);
                Log(x.Value);
                XElement url = new XElement("url", (string)x.Value.url);
                XElement item = new XElement("item", url);
                root.Add(item);
            }

            XDocument doc = new XDocument(root);
            string xml = doc.ToStringWithDeclaration();
            Log(xml);

            string output = RunWithOutput("ls.exe", "-ltr");
            Console.WriteLine($"[{output}]");

#if false
            var doc2 = ReadTextFileAsXml("cdata.xml");
            Log(doc2);
            Print(doc2.XPathSelectElement("//food/detail"));
            //Print("["+doc2.XPathSelectElement("//food/detail").Value+"]");
            string script = doc2.XPathSelectElement("//food/detail").Value;
            script = TrimNewLines(script);
            Print("[" + script + "]");
#endif
            ConvertExtra();
            ConvertPrograms();
        }
        catch (Exception e)
        {
            Log(e.ToString());
        }
    }

    private static void ConvertPrograms()
    {
        var programsXml = ReadTextFileAsXml("programs.xml");
        var jarr = new JArray();
        Log(programsXml);
        foreach (var item in programsXml.XPathSelectElements("//item"))
        {
            Log(item);
            var name = item.XPathSelectElement("./name").Value;
            var path = item.XPathSelectElement("./path").Value;
            if (path == "[extra]") path = null;
            var script = TrimNewLines(item.XPathSelectElement("./script").Value);
            if (script == "") script = null;
            jarr.Add(FromObject(new
            {
                name = name,
                path = path,
                script = script
            }));
#if false
            var version = item.XPathSelectElement("./version").Value;
            var ext = item.XPathSelectElement("./ext").Value;
            var url = item.XPathSelectElement("./url").Value;
            //var itemObj = new JObject();
            //itemObj["url"] = url;
            Log(url);
            jobj[name] = FromObject(new
            {
                version = version,
                ext = ext,
                path = path,
                url = url,
                script = script
            });
#endif
        }

        Log(jarr);
        WriteTextFileToFile("programs.json.new", ToJson(jarr, true));
    }

    private static void ConvertExtra()
    {
        var extraXml = ReadTextFileAsXml("extra.xml");
        var jobj = new JObject();
        Log(extraXml);
        foreach (var item in extraXml.XPathSelectElements("//item"))
        {
            Log(item);
            var name = item.XPathSelectElement("./name").Value;
            var version = item.XPathSelectElement("./version").Value;
            var ext = item.XPathSelectElement("./ext").Value;
            var path = item.XPathSelectElement("./path").Value;
            var url = item.XPathSelectElement("./url").Value;
            var script = TrimNewLines(item.XPathSelectElement("./script").Value);
            if (script == "") script = null;
            //var itemObj = new JObject();
            //itemObj["url"] = url;
            Log(url);
            jobj[name] = FromObject(new
            {
                version = version,
                ext = ext,
                path = path,
                url = url,
                script = script
            });
        }

        Log(jobj);
        var extraJson = new JObject();
        extraJson["software"] = jobj;
        WriteTextFileToFile("extra.json.new", ToJson(extraJson, true));
    }

    private static string TrimNewLines(string text)
    {
        //return text.TrimStart('\r', '\n').TrimEnd('\r', '\n');
        return Regex.Replace(text, @"^\s*[\r\n]+|\s*[\r\n]*$", "");
    }

    private static dynamic ReadTextFileAsJson(string filePath)
    {
        string fileContent;
        using (StreamReader reader = new StreamReader(filePath))
        {
            fileContent = reader.ReadToEnd();
        }

        return FromJson(fileContent);
    }

    private static XDocument ReadTextFileAsXml(string filePath)
    {
        string fileContent;
        using (StreamReader reader = new StreamReader(filePath))
        {
            fileContent = reader.ReadToEnd();
        }

        return FromXml(fileContent);
    }

    private static void WriteTextFileToFile(string filePath, string text)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.Write(text);
        }
    }

    private static void RunCommand(string cmd, params string[] args)
    {
        Process process = new Process();
        process.StartInfo.FileName = cmd;
        process.StartInfo.Arguments = String.Join(" ", args.Select(s => $"\"{s}\""));
        process.StartInfo.UseShellExecute = false;
        try
        {
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                Log(
                    $"`{process.StartInfo.FileName} {process.StartInfo.Arguments}` exited with code {process.ExitCode}. Aborting.");
                throw new Exception();
            }
        }
        catch (Exception e)
        {
            Log($"`{process.StartInfo.FileName} {process.StartInfo.Arguments}` got an exception. Aborting.");
            throw e;
            throw;
        }
    }

    private static string RunWithOutput(string cmd, params string[] args)
    {
        return RunWithOutput(false, cmd, args);
    }

    private static string RunWithOutput(bool allowError, string cmd, params string[] args)
    {
        Process process = new Process();
        process.StartInfo.FileName = cmd;
        process.StartInfo.Arguments = String.Join(" ", args.Select(s => $"\"{s}\""));
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        try
        {
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            if (!allowError && process.ExitCode != 0)
            {
                Log(
                    $"`{process.StartInfo.FileName} {process.StartInfo.Arguments}` exited with code {process.ExitCode}. Aborting.");
                throw new Exception();
            }

            return output;
        }
        catch (Exception e)
        {
            Log($"`{process.StartInfo.FileName} {process.StartInfo.Arguments}` got an exception. Aborting.");
            throw e;
            throw;
        }
    }
}
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.XPath;

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
            Print(Environment.CurrentDirectory);
            Environment.CurrentDirectory = @"C:\Users\Public\root\.repo\base2\work\spider-programs";
            ConvertExtraJson();
            ConvertProgramsJson();
        }
        catch (Exception e)
        {
            Log(e.ToString());
        }
    }

    private static void ConvertProgramsJson()
    {
        XElement root = new XElement("root");

        var programs = ReadTextFileAsJson("programs.json");
        Log(programs);
        foreach (var x in programs)
        {
            Log(x);
            XElement item = new XElement("item",
                new XElement("name", (string)x[0]),
                new XElement("path", x[1]==null ? "[extra]" : (string)x[1]),
                new XElement("script", "")
            );
            root.Add(item);
        }

        XDocument doc = new XDocument(root);
        string xml = doc.ToStringWithDeclaration();
        Log(xml);

        WriteTextFileToFile("programs.xml", xml);
    }

    private static void ConvertExtraJson()
    {
        XElement root = new XElement("root");

        var extra = ReadTextFileAsJson("extra.json")["software"];
        Log(extra);
        foreach (var x in extra)
        {
            Log(x.Name);
            Log(x.Value);
            XElement item = new XElement("item",
                new XElement("name", (string)x.Name),
                new XElement("version", (string)x.Value.version),
                new XElement("ext", (string)x.Value.ext),
                new XElement("path", (string)x.Value.path),
                new XElement("url", (string)x.Value.url),
                new XElement("script", "")
            );
            root.Add(item);
        }

        XDocument doc = new XDocument(root);
        string xml = doc.ToStringWithDeclaration();
        Log(xml);

        WriteTextFileToFile("extra.xml", xml);
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
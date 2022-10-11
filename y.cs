//css_nuget Newtonsoft.Json -ver:13.0.1
//css_nuget ProcessX -ver:1.5.4
//css_nuget ShellProgressBar -ver:5.2.0
//css_nuget JavaCommons -ver:2022.1012.147.9-beta
//XXXcss_import funcs.cs

using System;
using System.Threading;
using Newtonsoft.Json;
using ShellProgressBar;
using Zx;
using static Zx.Env;
using JavaCommons;

var ary = Util.FromJson("[1, 2, 3]");
Console.WriteLine(ary);
const int totalTicks = 10;
var options = new ProgressBarOptions
{
    ProgressCharacter = '─',
    ProgressBarOnBottom = true
};
using (var pbar = new ProgressBar(totalTicks, "Initial message", options))
{
    Thread.Sleep(500);
    pbar.Tick(); //will advance pbar to 1 out of 10.
    Thread.Sleep(500);
    //we can also advance and update the progressbar text
    pbar.Tick("Step 2 of 10");
    Thread.Sleep(500);
}

var cwd = await "pwd";
Console.WriteLine($"cwd={cwd}");

var currentBranch = await "git branch --show-current";
log($"現在のブランチは {currentBranch} です！！");

Util.Print(new { a = 11, b = 22 });

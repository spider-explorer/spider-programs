using static JavaCommons.Util;

var cwd = await "pwd";
Print(cwd, "cwd");
Print("(1)");
//await "cat extra.json | grep ext || true";
await "cat extra.json | grep ext || true > tmp.txt";
Print("(2)");
await "echo (3)";
await "cd C:/";
await "pwd";
Print("(4)");
await $"cd {cwd}";
await "pwd";
Print("(5)");

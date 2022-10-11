using static JCS.jcs;

var cwd = await "pwd";
print(cwd, "cwd");
print("(1)");
//await "cat extra.json | grep ext || true";
await "cat extra.json | grep ext || true > tmp.txt";
print("(2)");
await "echo (3)";
await "cd C:/";
await "pwd";
print("(4)");
await $"cd {cwd}";
await "pwd";
print("(5)");
print("print(ハロー©)!");
async function execute(v) {
    const p = Deno.run({
        cmd: v
    });
    const { success, code } = await p.status();
    let result = {};
    result.success = success;
    result.code = code;
    return result;
}

async function executePipe(v) {
    const p = Deno.run({
        cmd: v,
        stdout: "piped",
        stderr: "piped",
    });
    const { success, code } = await p.status();
    let result = {};
    result.success = success;
    result.code = code;
    const rawOutput = await p.output();
    result.stdout = new TextDecoder("shift-jis").decode(rawOutput);
    const rawError = await p.stderrOutput();
    result.stderr = new TextDecoder("shift-jis").decode(rawError);
    return result;
}

async function scoopAppInfo(key, path) {
    await execute(["cmd.exe", "/c", "scoop", "install", key]);
    await execute(["cmd.exe", "/c", "scoop", "update", key]);
    let st = await executePipe(["scoop-console-x86_64-static.exe", "--latest", key]);
    let list = st.stdout.trim().split(" ");
    let version = list[0];
    let dir = list[1];
    let url = `https://github.com/spider-explorer/spider-programs/releases/download/64bit/${key}-${version}.7z`;
    st = await executePipe(["curl.exe", url, "-o", "/dev/null", "-w", "%{http_code}", "-s"]);
    return { "name": key, "path": path, "version": version, "dir": dir, "url": url, "exists": st.stdout != "404" };
}

let cwd = Deno.cwd();

await execute(["cmd.exe", "/c", "scoop", "install", "git"]);
await execute(["cmd.exe", "/c", "scoop", "bucket", "add", "main"]);
await execute(["cmd.exe", "/c", "scoop", "bucket", "add", "extras"]);
await execute(["cmd.exe", "/c", "scoop", "bucket", "add", "java"]);

console.log("hello!");

console.log(await executePipe(["echo", "hello漢字©"]));

let investigate = JSON.parse(await Deno.readTextFile('investigate.json'));
console.log(investigate);

let programs = JSON.parse(await Deno.readTextFile('programs.json'));
let keys = Object.keys(programs);
console.log(keys);

let buildDir = cwd + "\\build";
Deno.mkdir(buildDir, { recursive: true });

//Deno.exit(777);

for (let key of keys)
{
    let app = await scoopAppInfo(key, programs[key]);
    console.log(app);
    investigate["software"][app.name] = { "version": app.version, "path": app.path, "url": app.url };
    if (!app.exists)
    {
        Deno.chdir(app.dir);
        await execute(["cmd.exe", "/c", "dir"]);
        let archive = buildDir + `/${app.name}-${app.version}.7z`;
        await execute(["7z.exe", "a", "-r", archive, "*", "-x!User Data", "-x!profile", "-x!distribution"]);
        console.log("(1)");
        Deno.chdir(cwd);
        //await execute(["gh.exe", "auth", "login", "--with-token", Deno.env.get("GITHUB_ALL")]);
        //console.log("(2)");
        await execute(["gh.exe", "release", "upload", "64bit", archive]);
        console.log("(3)");
        //Deno.exit(123);
    }
}

Deno.writeTextFile("spider-programs.json", JSON.stringify(investigate, null, 2));
//console.log(Deno.cwd());

//Deno.chdir(Deno.env.get("HOME"));

//console.log(Deno.cwd());

#! deno run --allow-all --unstable
import * as JSONC from "https://deno.land/std@0.177.1/encoding/jsonc.ts";
import * as sys from "npm:open-system@2023.1012.195138";

/*
async function fileExists(filepath) {
    try {
        const file = await Deno.stat(filepath);   
        return file.isFile();
    } catch (e) {
        return false
    }
}

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
*/

//let cwd = Deno.cwd();
let cwd = sys.cwd();

//await execute(["gh", "auth", "login", "--hostname", "github.com", "--git-protocol", "https", "--web"]);
await sys.run(["gh", "auth", "login", "--hostname", "github.com", "--git-protocol", "https", "--web"]);

let buildDir = cwd + "\\.build";
Deno.mkdir(buildDir, { recursive: true });

let extra = JSONC.parse(await Deno.readTextFile('extra.json'))["software"];
console.log(extra);

async function scoopAppInfo(rec /*key, path*/) {
    if (rec.path == null) {
      return { "name": rec.name, "path": extra[rec.name]["path"], "version": extra[rec.name]["version"], "dir": null, "ext": extra[rec.name]["ext"], "url": extra[rec.name]["url"], "script": extra[rec.name]["script"], "exists": true };
    }
    await sys.run(["cmd.exe", "/c", "scoop", "install", rec.name]);
    await sys.run(["cmd.exe", "/c", "scoop", "update", rec.name]);
    let st = await sys.runWithOutput(["scoop-console-x86_64-static.exe", "--latest", rec.name]);
    let list = st.stdout.trim().split(" ");
    let version = list[0];
    let dir = list[1];
    let url = `https://github.com/spider-explorer/spider-programs/releases/download/64bit/${rec.name}-${version}.zip`;
    st = await sys.runWithOutput(["curl.exe", url, "-o", "/dev/null", "-w", "%{http_code}", "-s"], true/*ignoreErrors*/);
    return { "name": rec.name, "path": rec.path, "version": version, "dir": dir, "ext": "zip", "url": url, "script": rec.script, "exists": st.stdout != "404" };
}

await sys.run(["cmd.exe", "/c", "scoop", "install", "git"]);
await sys.run(["cmd.exe", "/c", "scoop", "bucket", "add", "main"]);
await sys.run(["cmd.exe", "/c", "scoop", "bucket", "add", "extras"]);
await sys.run(["cmd.exe", "/c", "scoop", "bucket", "add", "java"]);

let programs = JSONC.parse(await Deno.readTextFile('programs.json'));

let result = [];

for (var rec of programs)
{
	console.log(rec);
    let app = await scoopAppInfo(rec /*rec.name, rec.path*/);
    console.log(app);
    //let url_parts = app.url.split(".");
    //let ext = url_parts[url_parts.length - 1];
    result.push({ "name": app.name, "version": app.version, "path": app.path, "url": app.url, "ext": app.ext, "script": app.script });
    if (!app.exists)
    {
        Deno.chdir(app.dir);
        await execute(["cmd.exe", "/c", "dir"]);
		if (await fileExists("IDE/bin/idea.properties")) {
           await execute(["sed", "-i",
            "-e", "s/^idea[.]config[.]path=/#\\\\0/g",
            "-e", "s/^idea[.]system[.]path=/#\\\\0/g",
            "-e", "s/^idea[.]plugins[.]path=/#\\\\0/g",
            "-e", "s/^idea[.]log[.]path=/#\\\\0/g", "IDE/bin/idea.properties"]);
		}
        let archive = buildDir + `/${app.name}-${app.version}.zip`;
        if (!await fileExists(archive)) await execute(["7z.exe", "a", "-r", "-tzip", "-mcu=on", archive, "*", "-x!User Data", "-x!profile", "-x!data", "-x!distribution"]);
        console.log("(1)");
        Deno.chdir(cwd);
        console.log("(2)");
        await sys.run(["gh.exe", "release", "upload", "64bit", archive]);
        console.log("(3)");
    }
}
Deno.chdir(cwd);
Deno.writeTextFile("00-software.json", JSON.stringify({ "software" : result }, null, 2));
await sys.run(["gh.exe", "release", "upload", "64bit", "00-software.json", "--clobber"]);

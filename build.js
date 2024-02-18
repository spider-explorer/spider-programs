#! deno run --allow-all --unstable
import * as JSONC from "https://deno.land/std@0.177.1/encoding/jsonc.ts";
import * as sys from "npm:open-system@2023.1012.230524";

let cwd = sys.cwd();
let home = Deno.env.get("HOME");

//await sys.run(["gh", "auth", "login", "--hostname", "github.com", "--git-protocol", "https", "--web"]);

await sys.run(["bash", cwd + "/gh-login.sh"]);

let buildDir = cwd + "\\.build";
sys.mkdir(buildDir);

let extra = JSONC.parse(sys.readTextFileSync('extra.json'))["software"];
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

let programs = JSONC.parse(sys.readTextFileSync('programs.json'));

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
        sys.chdir(app.dir);
        if (app.name == "git") {
           await sys.run(["cmd.exe", "/c", "cp", cwd + "/gitconfig", app.dir + "/etc/gitconfig"]);
        }
        await sys.run(["cmd.exe", "/c", "dir"]);
		if (sys.exists("IDE/bin/idea.properties")) {
           await sys.run(["sed", "-i",
            "-e", "s/^idea[.]config[.]path=/#\\\\0/g",
            "-e", "s/^idea[.]system[.]path=/#\\\\0/g",
            "-e", "s/^idea[.]plugins[.]path=/#\\\\0/g",
            "-e", "s/^idea[.]log[.]path=/#\\\\0/g", "IDE/bin/idea.properties"]);
		}
        let archive = buildDir + `/${app.name}-${app.version}.zip`;
        if (!sys.exists(archive)) await sys.run(["7z.exe", "a", "-r", "-tzip", "-mcu=on", archive, "*", "-x!User Data", "-x!profile", "-x!data", "-x!distribution"]);
        console.log("(1)");
        sys.chdir(cwd);
        console.log("(2)");
        await sys.run(["gh.exe", "release", "upload", "64bit", archive]);
        console.log("(3)");
    }
}
sys.chdir(cwd);
sys.writeTextFileSync("00-software.json", JSON.stringify({ "software" : result }, null, 2));
await sys.run(["gh.exe", "release", "upload", "64bit", "00-software.json", "--clobber"]);

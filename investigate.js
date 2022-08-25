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

console.log("hello!");

console.log(await executePipe(["echo", "hello漢字©"]));
/*
const p = Deno.run({
    cmd: ["echo", "hello漢字©"],
});

// 完了を待つ
let st = await p.status();
console.log(st);
*/

let json = await Deno.readTextFile('data.json');
console.log(json);

console.log(JSON.parse(json));

console.log(Deno.cwd());

Deno.chdir(Deno.env.get("HOME"));

console.log(Deno.cwd());

let st = await executePipe(["scoop-console-x86_64-static.exe", "--latest", "vscode"]);
console.log(st);
console.log(st.stdout.trim().split(" "));

await execute(["cmd.exe", "/c", "scoop", "install", "vscode"]);

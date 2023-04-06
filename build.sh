#! /usr/bin/env bash
set -uvx
set -e
cwd=`pwd`
cd $cwd/convert1
dotnet build
cd $cwd
$cwd/convert1/bin/x64/Debug/net462/build.exe
deno.exe run --allow-all --unstable ./build.js

#! /usr/bin/env bash
set -uvx
set -e
cwd=`pwd`
cd $cwd/convert1
dotnet build
cd $cwd
$cwd/convert1/bin/Debug/net462/build.exe
#$cwd/convert1/bin/Debug/net462/build.exe
deno.exe run --allow-all --unstable ./build.js

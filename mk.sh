set -uvx
set -e
rm -rf tmp
dotnet script publish setup.csx -o tmp -c Release -r win-x64
ls -l tmp

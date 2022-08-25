set -e
curl "https://github.com/spider-explorer/spider-programs/releases/download/64bit/busybox-2022.08.25.16.50.31.7z" -o /dev/null -w '%{http_code}\n' -s

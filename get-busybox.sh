set -uvx
set -e
cwd=`pwd`
rm -rf tmp
mkdir tmp
cd tmp
rm -rf busybox64.exe
wget.exe --no-check-certificate https://frippery.org/files/busybox/busybox64.exe
if diff busybox64.exe ../busybox64.exe ; then
    echo "No Diff."
    exit 0
fi

echo "Diff Found."
cp busybox64.exe ../busybox64.exe
ts=`date "+%Y.%m.%d.%H.%M.%S"`
rm -rf busybox.tmp
mkdir -p busybox.tmp
./busybox64.exe --install ./busybox.tmp
cd busybox.tmp
rm -rf ar.exe strings.exe unzip.exe sh.exe sed.exe wget.exe
7z a -r ../busybox-$ts.7z *
cd ..
echo $GITHUB_ALL | gh auth login --with-token
gh release upload 64bit busybox-$ts.7z
cd $cwd
cat << EOS > investigate.json
{
  "version": "?",
  "software": {
    "busybox": {
      "version": "$ts",
      "ext": "7z",
      "path": ".",
      "url": "https://github.com/spider-explorer/spider-programs/releases/download/64bit/busybox-$ts.7z"
    },
    "SimpleBrowser": {
      "version": "v2022.07.26.23.20.00",
      "ext": "zip",
      "path": ".",
      "url": "https://github.com/spider-explorer/spiderbrowser/releases/download/v2022.07.26.19.39.00/SimpleBrowser-v2022.07.26.23.20.00.zip"
    },
    "SpiderBrowser": {
      "version": "v2022.07.26.23.20.00",
      "ext": "zip",
      "path": ".",
      "url": "https://github.com/spider-explorer/spiderbrowser/releases/download/v2022.07.26.19.39.00/SpiderBrowser-v2022.07.26.23.20.00.zip"
    },
    "AirExplorer": {
      "version": "4.6.2",
      "ext": "7z",
      "path": ".",
      "url": "https://gitlab.com/spider-explorer/spider-software/-/raw/main/.software/AirExplorer/AirExplorer-4.6.2.7z"
    },
    "Everything": {
      "version": "1.4.1.1015.x64",
      "ext": "zip",
      "path": ".",
      "url": "https://gitlab.com/spider-explorer/spider-software/-/raw/main/.software/Everything/Everything-1.4.1.1015.x64.zip"
    },
    "FileSum": {
      "version": "3.01",
      "ext": "zip",
      "path": ".",
      "url": "https://gitlab.com/spider-explorer/spider-software/-/raw/main/.software/FileSum/FileSum-3.01.zip"
    },
    "FolderSizePortable": {
      "version": "4.9.5.0",
      "ext": "zip",
      "path": ".",
      "url": "https://gitlab.com/spider-explorer/spider-software/-/raw/main/.software/FolderSizePortable/FolderSizePortable-4.9.5.0.zip"
    },
    "office_x64": {
      "version": "7.3.2",
      "ext": "7z",
      "path": "/program",
      "url": "https://gitlab.com/spider-explorer/spider-software/-/raw/main/.software/office_x64/office_x64-7.3.2.7z"
    },
    "winpython": {
      "version": "3.10.5",
      "ext": "7z",
      "path": "/Scripts;.",
      "url": "https://gitlab.com/spider-explorer/spider-software/-/raw/main/.software/winpython/winpython-3.10.5.7z"
    },
    "Arora": {
      "version": "0.10.0",
      "ext": "7z",
      "path": ".",
      "url": "https://gitlab.com/spider-explorer/spider-software/-/raw/main/.software/Arora/Arora-0.10.0.7z"
    },
    "Falkon": {
      "version": "3.1.0",
      "ext": "7z",
      "path": ".",
      "url": "https://gitlab.com/spider-explorer/spider-software/-/raw/main/.software/Falkon/Falkon-3.1.0.7z"
    },
    "nwjs-sdk": {
      "version": "v0.66.1",
      "ext": "7z",
      "path": ".",
      "url": "https://gitlab.com/spider-explorer/spider-software/-/raw/main/.software/nwjs-sdk/nwjs-sdk-v0.66.1.7z"
    }
  }
}
EOS

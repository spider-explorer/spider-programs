#! /usr/bin/env -S busybox ash
set -uvx
set -e
cwd=`pwd`
rm -rf tmp
mkdir tmp
cd tmp
rm -rf busybox64.exe
wget.exe --no-check-certificate https://frippery.org/files/busybox/busybox64.exe
#wget.exe --no-check-certificate https://frippery.org/files/busybox/prerelease/busybox_pre64.exe -O busybox64.exe
if diff busybox64.exe ../busybox64.exe ; then
    echo "No Diff."
    exit 0
fi

echo "Diff Found."
cp busybox64.exe ../busybox64.exe
ts=`date "+%Y.%m.%d.%H.%M.%S"`
ts=bbox.$ts
rm -rf busybox.tmp
mkdir -p busybox.tmp
./busybox64.exe --install ./busybox.tmp
cd busybox.tmp
#rm -rf ar.exe strings.exe unzip.exe ash.exe bash.exe sh.exe sed.exe wget.exe make.exe
rm -rf sh.exe
7z a -r -tzip -mcu=on ../_busybox-$ts.zip *
cd ..
gh auth login --hostname github.com --git-protocol https --web
gh release upload 64bit _busybox-$ts.zip
cd $cwd
#sed -i -e "s/bbox[.][0-9]*[.][0-9]*[.][0-9]*[.][0-9]*[.][0-9]*[.][0-9]*/${ts}/g" extra.xml
sed -i -e "s/bbox[.][0-9]*[.][0-9]*[.][0-9]*[.][0-9]*[.][0-9]*[.][0-9]*/${ts}/g" extra.json

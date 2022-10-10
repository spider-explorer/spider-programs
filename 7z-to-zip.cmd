rmdir /s /q tmp
mkdir tmp
cd tmp
7z x ..\%1.7z
7z a -r -tzip -mcu=on ..\_%1.zip *
cd ..

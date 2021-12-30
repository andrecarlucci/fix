dotnet publish -c Release --self-contained true -p:PublishTrimmed=true -p:PublishSingleFile=true -r win-x64

copy C:\dev\personal\Fix\Fix\bin\Release\net5.0\win-x64\publish\Fix.exe c:\path /y

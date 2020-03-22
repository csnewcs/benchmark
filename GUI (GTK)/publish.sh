dotnet publish -r linux-x64 --self-contained false
dotnet publish -r linux-arm --self-contained false
dotnet publish -r win-x64 --self-contained false
dotnet publish -r osx-x64 --self-contained false
cd bin/Debug/netcoreapp3.1
./zip.sh

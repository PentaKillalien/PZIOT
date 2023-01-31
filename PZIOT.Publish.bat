color B

del  .PublishFiles\*.*   /s /q

dotnet restore

dotnet build

cd PZIOT.Api

dotnet publish -o ..\PZIOT.Api\bin\Debug\net6.0\

md ..\.PublishFiles

xcopy ..\PZIOT.Api\bin\Debug\net6.0\*.* ..\.PublishFiles\ /s /e 

echo "Successfully!!!! ^ please see the file .PublishFiles"

cmd
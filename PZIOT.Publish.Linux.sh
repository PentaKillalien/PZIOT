git pull;
find .PublishFiles/ -type f -and ! -path '*/wwwroot/images/*' ! -name 'appsettings.*' |xargs rm -rf
dotnet build;
rm -rf /home/PZIOT/PZIOT.Api/bin/Debug/.PublishFiles;
dotnet publish -o /home/PZIOT/PZIOT.Api/bin/Debug/.PublishFiles;
# cp -r /home/PZIOT/PZIOT.Api/bin/Debug/.PublishFiles ./;
awk 'BEGIN { cmd="cp -ri /home/PZIOT/PZIOT.Api/bin/Debug/.PublishFiles ./"; print "n" |cmd; }'
echo "Successfully!!!! ^ please see the file .PublishFiles";
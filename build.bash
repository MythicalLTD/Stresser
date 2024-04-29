#!/bin/bash
rm -r Stresser*/obj
rm -r Stresser*/bin
rm /var/www/Stresser/StresserAMD
rm /var/www/Stresser/StresserARM
rm /var/www/Stresser/StresserARM64
rm /var/www/Stresser/StresserHTTP_AMD
rm /var/www/Stresser/StresserHTTP_ARM
rm /var/www/Stresser/StresserHTTP_ARM64
rm /var/www/Stresser/StresserTCP_AMD
rm /var/www/Stresser/StresserTCP_ARM
rm /var/www/Stresser/StresserTCP_ARM64
rm /var/www/Stresser/StresserUDP_AMD
rm /var/www/Stresser/StresserUDP_ARM
rm /var/www/Stresser/StresserUDP_ARM64

runtimes=("linux-x64" "linux-arm64" "linux-arm")

dotnet clean

for runtime in "${runtimes[@]}"; do
    echo "Publishing for runtime: $runtime"
    dotnet publish -c Release -r "$runtime" /p:PublishSingleFile=true -p:Version=1.0.0.1
    echo "----------------------------------"
done

mv /var/www/Stresser/bin/Release/net8.0/linux-arm/publish/Stresser /var/www/Stresser/StresserARM
mv /var/www/Stresser/bin/Release/net8.0/linux-arm64/publish/Stresser /var/www/Stresser/StresserARM64
mv /var/www/Stresser/bin/Release/net8.0/linux-x64/publish/Stresser /var/www/Stresser/StresserAMD
cd /var/www/Stresser/StresserHTTP
bash build.bash
rm -r Stresser*/obj
rm -r Stresser*/bin
cd /var/www/Stresser/StresserUDP
bash build.bash
rm -r Stresser*/obj
rm -r Stresser*/bin
cd /var/www/Stresser/StresserTCP
bash build.bash
rm -r Stresser*/obj
rm -r Stresser*/bin

#!/bin/bash

runtimes=("linux-x64" "linux-arm64" "linux-arm")

dotnet clean

for runtime in "${runtimes[@]}"; do
    echo "Publishing for runtime: $runtime"
    dotnet publish -c Release -r "$runtime" /p:PublishSingleFile=true -p:Version=1.0.0.1 
    echo "----------------------------------"
done

mv /var/www/Stresser/StresserUDP/bin/Release/net8.0/linux-arm/publish/StresserUDP /var/www/Stresser/StresserUDP_ARM
mv /var/www/Stresser/StresserUDP/bin/Release/net8.0/linux-arm64/publish/StresserUDP /var/www/Stresser/StresserUDP_ARM64
mv /var/www/Stresser/StresserUDP/bin/Release/net8.0/linux-x64/publish/StresserUDP /var/www/Stresser/StresserUDP_AMD

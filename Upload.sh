#!/bin/bash
VERSION=`cat /home/ubuntu/Terminals/Terminals/Properties/AssemblyInfo.cs | grep "AssemblyVersion (" | cut -d \" -f 2`
ARTIFACTS="/home/ubuntu/Terminals/Terminals/bin/x86/Release/"

######## DOWNLOAD AND INVOKE "GHR" - a tool capable of uploading a release to GitHub

echo Grabbing GHR

#go get -u -f -v github.com/tcnksm/ghr
#ghr -t $GITHUB_TOKEN -u $CIRCLE_PROJECT_USERNAME -r $CIRCLE_PROJECT_REPONAME --replace $VERSION $ARTIFACTS
#ghr -t $GITHUB_TOKEN -u $CIRCLE_PROJECT_USERNAME -r $CIRCLE_PROJECT_REPONAME --replace $VERSION "/home/ubuntu/Terminals/Terminals.Setup/Output/"

wget https://github.com/tcnksm/ghr/releases/download/v0.5.3/ghr_v0.5.3_linux_amd64.zip && unzip ghr_v0.5.3_linux_amd64.zip -d . && echo Finished downloading GHR. Progressing with the upload.

./ghr -t $GITHUB_TOKEN -u $CIRCLE_PROJECT_USERNAME -r $CIRCLE_PROJECT_REPONAME --replace $VERSION $ARTIFACTS && ./ghr -t $GITHUB_TOKEN -u $CIRCLE_PROJECT_USERNAME -r $CIRCLE_PROJECT_REPONAME --replace $VERSION "/home/ubuntu/Terminals/Terminals.Setup/Output/" && echo Uploads have been completed successfully.

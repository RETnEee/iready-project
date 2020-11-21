#!/bin/sh
image_version=`date +%Y%m%d%H%M`;
echo $image_version;
cd ~/mypro/project/iready-project;
git pull origin master;
docker stop iready-project;
docker rm iready-project;
docker build -t iready-project:$image_version .;
docker images;
docker run -p 8088:80 --restart=always --name iready-project -d iready-project:$image_version;
docker logs iready-project;

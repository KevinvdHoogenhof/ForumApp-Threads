# ForumApp-Threads

Threadservice of the forum application

## SSL Certificate

dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p mypass123  

## Database

Get image:

Docker run:

## ThreadService
### Build docker image
docker build -t threadservice . (in project folder)

### Pull image from Dockerhub

docker pull kvdhoogenhof/forumapp-threads

## Run image

docker run -d -e ASPNETCORE_ENVIRONMENT=Development –e ASPNETCORE_URLS="https://+:8080;http://+:8081" –e ASPNETCORE_HTTPS_PORT=9000  -e ASPNETCORE_Kestrel__Certificates__Default__Password=mypass123  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v %USERPROFILE%/.aspnet/https:/https:ro -p 9001:8080 --name api kvdhoogenhof/forumapp-threads

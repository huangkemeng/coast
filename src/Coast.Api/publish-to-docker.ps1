$networks = docker network ls
$networkExists = $networks | Where-Object { $_ -like "*docker-network*" }
if(!$networkExists )
{
   docker network create docker-network
}
docker  container stop Coast.Api
docker  container rm Coast.Api
docker  image rm Coast.Api:latest
docker build  --tag Coast.Api:latest . 
docker run -id --name Coast.Api --restart=no  --network=docker-network  -p 8006:80 -p 8106:443  -e ASPNETCORE_ENVIRONMENT=Development Coast.Api:latest 
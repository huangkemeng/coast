$networks = docker network ls
$networkExists = $networks | Where-Object { $_ -like "*docker-network*" }
if(!$networkExists )
{
   docker network create docker-network
}
docker  container stop RequirementTrackingSystem
docker  container rm RequirementTrackingSystem
docker  image rm RequirementTrackingSystem:latest
docker build  --tag RequirementTrackingSystem:latest . 
docker run -id --name RequirementTrackingSystem --restart=no  --network=docker-network  -p 8006:80 -p 8106:443  -e ASPNETCORE_ENVIRONMENT=Development RequirementTrackingSystem:latest 
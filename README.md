# docker-g1

# Images
Images are templates for containers.
Containers execute the code.
Images != Containers.
Images are READ ONLY. Images are locked in once you build it. You cannot modify them from the outside.
Containers are READ-WRITE.

We can use existing images or create one ourserlves. 

## Dockerfile
FROM xxx : allow us to create an image on top of another one 
COPY x y : copy files (x) outside the image to a location (y) inside the image
RUN npm install : run a command in the working directory
WORKDIR xxx : set the working directory
CMD ["node",  "server.js"] : command that will be executed when a container execute the image
EXPOSE 80 : expose a port to our local machine 

In a dockerfile, if an instruction is invalidated (for instance : code change and so the COPY . . should be executed again), all the instructions after will be reexecuted. It could be nice to take that into account and move the package.json file first into the container, run npm install and then move all the code into the container. Like that, you do not run npm install each time we change a file in the code base.

## Usefull commands 
cmd $  docker build . : build an image
cmd $ docker run -p 3000:80 containerId/Name : create a new container from the specified image, and map the container port (80) to a local port (attached mode)
cmd $ docker stop containerId/Name : stop a running container
cmd $ docker ps : show all running containers 
cmd $ docker ps -a : show all containers 
cmd $ docker start containerId/Name : start an existing container (detached mode)
cmd $ docket attach containerId/Name : attach to a container

cmd $ docker logs -f containerId/Name : fetch the logs from the container

cmd $ docker rm containerId/Name : delete  a stopped container
cmd $ docker container prune : delete all stopped containers 
cmd $ docker rmi imageId : delete an image (if it is not used by a container, even if it is stopped)
cmd $ docker image prune : remove all unused images

cmd $ docker run -p 3000:80 -d --rm containerId/Name : this container will be removed once stopped

## Copy files into and from a container
cmd $ docker cp local-path container-path : copy a file to a container
cmd $ docker cp container-path local-path : copy a file from a container

## Naming and tagging containers 
cmd $ docker run -p 3000:80 -d --rm --name customName containerId/Name : define the container name

Images have a name and a tag --> name:tag, the tag is usually used for the version.
cmd $ docker build -t name:tag : build an image and specify the name and the tag


# Volumes
Volumes help to persist data.
Volumes are folders on host machines that are mapped to folders inside a docker container.

Volumes persist even if a container is shutted down. A container can write and read thata from a volume.

cmd $ docker run -p 3000:80 -d --rm --name customName -v volumeName:path-inside-container containerId/Name : create a named volume that will not be deleted by docker when the container stops

## Bind mounts
When you use a bind mount, a file or a directory on the host machine is mounted into a container. It is really usefull to mount our code folder to the container in order to not rebuild an image each time we make changes in our code. 
BE CAREFULL, bind-mounts overwrite containers folders. 

cmd $ docker run -p 3000:80 -d --rm --name customName -v volumeNameForData:path-inside-container -v "folder-path-on-local-machine":path-inside-container containerId/Name : create a bind mount directory.

BE CAREFULL, if you are mixing named volumes and bind mounts. You can erase results of dockerfile commands (like erasing the node modules directory after the npm install) by binding your code to the container. In order to solve this problem, you have to add an anonymous volume to keep necessary data.
cmd $ docker run -p 3000:80 -d --rm --name customName -v dataVolume:/app/data -v "C:/Machin-chouette":/app -v /app/node_modules containerId/Name : create a bind mount directory.

Anonymous volumes are removed when the container is stopped (if you used --rm).


# Arguments & Env variables 

## ENV
You can add a .env file to your project. 

Inside the file you can write : 
PORT = 8000

In you dockerfile, you can then write : 
ENV PORT 80 (port by default for instance)
EXPOSE $PORT

And then, when running the container, you can write : 
cmd $ docker run -p 3000:80 -d --rm --name customName -v dataVolume:/app/data -v "C:/Machin-chouette":/app -v /app/node_modules --env-file env-file-location containerId/Name

## ARG
You can add an argument in the dockerfile : 
ARG DEFAULT_PORT = 80
…
EXPOSE $PORT

And then, when running the container, you can write : 
cmd $ docker run -p 3000:80 -d --rm --name customName -v dataVolume:/app/data -v "C:/Machin-chouette":/app -v /app/node_modules --build-arg DEFAULT_PORT=8000 containerId/Name


# Networking 
Networking enable communications :
	- Container to WWW
	- Container to local machine
	- Container to container

## Contacting the web
Enable by default

## Contacting the local machine
For example, if you want to contact you local database and you have a connection string like : 
'mongodb://localhost:27017/xxx'

You juste have to replace localhost by host.docker.internal : 
'mongodb://host.docker.internal:27017/xxx'

## Contacting another container
You can create your network like this : 
cmd $ docker network create myNetwork

You must specify the network when running containers : 
cmd $ docker run -p 3000:80 -d --rm --name customName -v dataVolume:/app/data -v "C:/Machin-chouette":/app -v /app/node_modules --network myNetwork containerId/Name

cmd $ docker run -d --name mongodb --network myNetwork mongo

You juste have edit the connection string with the container name : 
'mongodb://mongodb:27017/xxx'


# Docker compose
Initial problem : working with several containers consist in writing a long script with docker commands to run them. A more convenient and easy way to do that is to use docker compose.

Docker compose : one configuration file that describe your application. With one command, you can run or stop your entire application.

Docker compose does not replace dockerfiles.
Docker compose does not replace images or containers.
Docker compose is not suited for managing multiples containers on different hosts (ie, it is for dev purpose).

Docker create by default a network for all the services registered in the docker compose file (the name of the network will be YourProjectFolderName-default-. 

## docker-compose.yaml example
```bash
version: "3.8"
services:
  mongodb:
    image: 'mongo'
    volumes: 
      - data:/data/db
    # environment: 
    #   MONGO_INITDB_ROOT_USERNAME: max
    #   MONGO_INITDB_ROOT_PASSWORD: secret
      # - MONGO_INITDB_ROOT_USERNAME=max
    env_file: 
      - ./env/mongo.env
  backend:
    build: ./backend
    # build:
    #   context: ./backend
    #   dockerfile: Dockerfile
    #   args:
    #     some-arg: 1
    ports:
      - '80:80'
    volumes: 
      - logs:/app/logs
      - ./backend:/app
      - /app/node_modules
    env_file: 
      - ./env/backend.env
    depends_on:
      - mongodb
  frontend:
    build: ./frontend
    ports: 
      - '3000:3000'
    volumes: 
      - ./frontend/src:/app/src
    stdin_open: true #  necessary to run the container in interactive (-it) mode
    tty: true #  necessary to run the container in interactive (-it) mode
    depends_on: 
      - backend

volumes: 
  data:
  logs:
```

## Usefull commands
cmd $ docker-compose up : start all services in the docker-compose.yaml file (you can start in detach mode with -d)
cmd $ docker-compose down :  shuts all containers down and deletes all containers/networks
cmd $ docker-compose up --build : build images before running containers
cmd $ docker-compose build : build all images referenced in the docker-compose.yaml

# K8s

## Usefull commands to push your image to your hub
 docker-compose build
 docker tag api:latest hughze/api:1.0
 sudo docker push hughze/api:1.0  

## Usefull commands if you are using minikube 
cmd $ minikube dashboard
cmd $ minikube start
cmd $ minikube service api-service
cmd $ kubectl apply -f=Kubernetes/cache.yaml
cmd $ kubectl logs api-deployment-6f4cd76854-q4mln
cmd $ kubectl rollout restart deployment api-deployment


## Deployment commands
cmd $ kubectl apply -f=cache.yaml
cmd $ kubectl apply -f=mongodb.yaml
cmd $ kubectl apply -f=api.yaml 

(si problème pour le StatefulSet -> minikube addons enable default-storageclass)
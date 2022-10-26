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

## Getting started with K8s
K8s is a tool that help us deploy, maintain and scale containers. K8s is like docker-compose for multiple machines.

We might want to use a tool like that for several situations : 
	- Containers might crash or go down and need to be replaced
	- We might need more container instances upon traffic spikes
	- Incoming traffic should be distributed equally

K8s allow us to be independent of the cloud provider we want to deploy to. We just have to provide the K8s configuration file.

!! K8s will not manage your infrastructure !! (-> Terraform / Set up on cloud provider)

### Worker Node
Runs the containers of your application. Worker Nodes are your machines/virtual instances. You can have more than one pod running on the same worker node.
It contains : 
	- Pod : container(s) we want to deploy. Multiple pods can be created or removed to scale your app. Pods are managed by K8s.
	- Kubelet : interracts with the Master Node
	- Kube-proxy : controls the network traffic of a Worker Node.

### Master Node
Controls your deployment (i.e. all Worker Nodes).
It contains : 
	- Control Plane : control center that interract with Worker Nodes.
	- API Server : API for the kubelets to communicate.
	- Scheduler : Watches for new Pods, selects Worker Nodes to run them on.
	- Kube-Controller-Manager : Watches and controls Worker Nodes, correct number of Pods & more
	- Cloud-Controller-Manager : translates instructions to a specific cloud provider.


Cluster : contains the Master Node and Worker Nodes. The cluster interracts with the cloud provider.

## Diving into core concepts
kubectl tool : interracts with the cluster.

In order to test K8s on a local machine, you have to install Minikube.
cmd $ minikube dashboard will provide a UI to see your cluster.
cmd $ minikube start
cmd $ minikube service serviceName


K8s works with objects (Pods, Deployments, Services, Volumes, …). 


### Pod Object
Pods are ephmeral : Kubernetes will start, stop and replace them as needed.
Pods contain shaired resources (e.g. volumes) for all Pod containers.
A Pod has a cluster-internal IP by default -> Containers inside a Pod can communicate via localhost.

### Deployment Object
Can control a pod.
We can set a desired state for a pod and K8s will change the state for us.
Deployments can be paused, deleted and rolled back.
Deployments can be scalled dynamically and automatically.
!! You don't directly control pods, you use Deployments to set up the desired state !!


cmd $ kubectl create deployment deploymentName --image=DockerHubRepoName/newImageName
!! imageName est le nom d'une image provenant de docker hub !!

cmd $ kubectl get deployments
cmd $ kubectl deployment delete deploymentName
cmd $ kubectl get pods 
cmd $ kubectl scale deployment/deploymentName --replicas=3 

### Service Object
To reach a pod and the container running in a pod, we need a service. Remember, each time a pod is rerun etc, it changes its ip address so we need something reliable to contact a pod.

Expose Pods to the Cluster or Externally.
A Service group Pods with a shared IP.

cmd $ kubectl expose deployment deploymentName --type=LoadBalancer --port=8080 : expose a pod and create a service 
cmd $ kubectl get services
cmd $ kubectl service delete serviceName 

### Update deployments
cmd $ kubectl set image deployment/deploymentName oldImageName=DockerHubRepoName/newImageName:tag (tag of the docker image has to be different in order to be interpreted)

cmd $ kubectl rollout status deployment/deploymentName : get the status of the update of the deployment

### Deployment rollbacks & history
cmd $ kubectl rollout undo deployment/deploymentName

cmd $ kubectl rollout undo deployment/deploymentName --to-revision=1

cmd $ kubectl rollout history deployment/deploymentName --revision=x

### K8s declarative approach
cmd $ kubectl apply -f deployment.yaml

The file name is totally up to you, it just have to be a .yaml file.

Example for a deployment : 

```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: second-app-deployment
spec:
  replicas: 1
  selector:
    matchLabels:
      app: second-app
      tier: backend
  template:
    metadata: 
      labels:
        app: second-app
        tier: backend
    spec: 
      containers:
        - name: second-node
          image: academind/kub-first-app:2
        # - name: ...
        #   image: ...
```

The template of a deployment always describes a pod.
Selectors are mandatory. A Salector specify which pods should be controlled by the deployment. 

Example for a service : 

```
apiVersion: v1
kind: Service
metadata:
  name: backend
spec:
  selector: 
    app: second-app
  ports:
    - protocol: 'TCP'
      port: 80
      targetPort: 8080
    # - protocol: 'TCP'
    #   port: 443
    #   targetPort: 443
  type: LoadBalancer
```

cmd $ kubectl apply -f service.yaml
cmd $ minikube service backend

Updating and deleting resources
Change the file, save the file and run the apply command again.
For deleting, use cmd $ kubectl delete -f deployment.yaml.

Single file vs multiple files. 
You can use one file and combine all the above files if you use the '---' separator.

##  Managing Data & Volumes

State = data created and used by your application which must not be lost. We use volumes to save the state.
K8s must be configured to add volumes to our containers.

Volumes lifetime depends on the pod lifetime => volumes are pod's specific. Volumes survived restarting and reloading pods but if a pod is removed, the volume is removed.

emptyDir Volume Type
Create a new empty directory when a pod starts and keep this directory as long as the pod is alive. When a pod is removed, the directory is removed. When a pod is recreated, the directory is recreated. This directory survives container restarts.

```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: story-deployment
spec: 
  replicas: 1
  selector:
    matchLabels:
      app: story
  template:
    metadata:
      labels:
        app: story
    spec:
      containers:
        - name: story
          image: academind/kub-data-demo:1
          volumeMounts:
            - mountPath: /app/story #path inside the container
              name: story-volume
      volumes:
        - name: story-volume
          emptyDir: {}
```

hostPath Volume Type
This volume type works well with multiple replicas if you have ONE host machine.

```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: story-deployment
spec: 
  replicas: 2
  selector:
    matchLabels:
      app: story
  template:
    metadata:
      labels:
        app: story
    spec:
      containers:
        - name: story
          image: academind/kub-data-demo:1
          volumeMounts:
            - mountPath: /app/story
              name: story-volume
      volumes:
        - name: story-volume
          hostPath:
            path: /data
            type: DirectoryOrCreate
```

CSI Volume Type
CSI stands for Container Storage Interface. Enable custom implementations.

Persistent Volumes
You often need pod and node independent volumes -> persistent volumes. A persistent volume is a separate resource. It is not contained inside a node. Node have Persistent Volume Claims in order to contact Persistend Volumes. 


Host-pv.yaml
```
apiVersion: v1
kind: PersistentVolume
metadata:
  name: host-pv
spec:
  capacity: 
    storage: 1Gi
  volumeMode: Filesystem
  storageClassName: standard
  accessModes:
    - ReadWriteOnce
  hostPath:
    path: /data
    type: DirectoryOrCreate
```


We need a claim in order to use the Persistent Volume.
Host-pvc.yaml
```
apiVersion: v1
kind: PersistentVolumeClaim
metadata:
  name: host-pvc
spec:
  volumeName: host-pv
  accessModes:
    - ReadWriteOnce
  storageClassName: standard
  resources:
    requests: 
      storage: 1Gi
```

Deployment.yaml
```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: story-deployment
spec: 
  replicas: 2
  selector:
    matchLabels:
      app: story
  template:
    metadata:
      labels:
        app: story
    spec:
      containers:
        - name: story
          image: academind/kub-data-demo:1
          volumeMounts:
            - mountPath: /app/story
              name: story-volume
      volumes:
        - name: story-volume
          persistentVolumeClaim:
            claimName: host-pvc
```


Env variables

Deployment.yaml
```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: story-deployment
spec: 
  replicas: 2
  selector:
    matchLabels:
      app: story
  template:
    metadata:
      labels:
        app: story
    spec:
      containers:
        - name: story
          image: academind/kub-data-demo:2
          env:
            - name: STORY_FOLDER
              # value: 'story'
              valueFrom: 
                configMapKeyRef:
                  name: data-store-env
                  key: folder
          volumeMounts:
            - mountPath: /app/story
              name: story-volume
      volumes:
        - name: story-volume
          persistentVolumeClaim:
            claimName: host-pvc
```

Environment.yaml
```
apiVersion: v1
kind: ConfigMap
metadata:
  name: data-store-env
data:
  folder: 'story'
  # key: value..
```

## Networking
Pod-internal communication
Use localhost to contact other containers in the same pods (you should not have multiple containers in a single pod though …)

Pod-to-Pods communication
To contact another pod, you can use in the code the automaticaly generated variable POD_NAME_SERVICE_HOST to get its ip.

You can also use the K8s DNS.
You can use the automaticaly generated domain "service-name.namespace". 

auth-service.yaml
```
apiVersion: v1
kind: Service
metadata:
  name: auth-service
spec:
  selector:
    app: auth
  type: ClusterIP #ClusterIp = not public
  ports:
    - protocol: TCP
      port: 80
      targetPort: 80
```

users-service.yaml
```
apiVersion: v1
kind: Service
metadata:
  name: users-service
spec:
  selector:
    app: users
  type: LoadBalancer
  ports:
    - protocol: TCP
      port: 8080
      targetPort: 8080
```

users-deployment.yaml
```
apiVersion: apps/v1
kind: Deployment
metadata:
  name: users-deployment
spec:
  replicas: 1
  selector: 
    matchLabels:
      app: users
  template:
    metadata:
      labels:
        app: users
    spec:
      containers:
        - name: users
          image: academind/kub-demo-users:latest
          env:
            - name: AUTH_ADDRESS
              # value: "10.99.104.252"
              value: "auth-service.default"
```

In the users api code : 
const hashedPW = await axios.get(`http://${process.env.AUTH_ADDRESS}/hashed-password/` + password);


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
minikube addons enable ingress
minikube addons enable default-storageclass

cmd $ kubectl apply -f=cache.yaml

cmd $ kubectl apply -f=mongodb.yaml
cmd $ kubectl apply -f=api.yaml 

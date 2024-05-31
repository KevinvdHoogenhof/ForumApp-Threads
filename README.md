# ForumApp-Threads

Threadservice of the forum application

## Testing this application

Local development: Use the localhost connectionstring

Docker compose: Use the threaddb connectionstring

# Kubernetes

### Run

In k8s folder:

```kubectl apply -f mongopvc.yaml``` 

```kubectl apply -f mongodb.yaml``` 

```kubectl apply -f threadservice.yaml``` (Should be accessible at localhost:30007)

```kubectl apply -f hpa.yaml```

```kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.10.1/deploy/static/provider/cloud/deploy.yaml```

```kubectl apply -f ingress.yaml``` (localhost:80)

```kubectl apply -f https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml```

1 ```kubectl -n kube-system edit deploy metrics-server```

2 Copy-paste part from metrics-service.yaml

### Prometheus + Grafana monitoring

```kubectl apply -f ingress-nginx-controller.yaml```

```kubectl apply --kustomize github.com/kubernetes/ingress-nginx/deploy/prometheus/```

```kubectl apply --kustomize github.com/kubernetes/ingress-nginx/deploy/grafana/```

```kubectl apply -f configmap.yaml```

```kubectl apply -f nginx-configmap.yaml```

### Status

```kubectl get deployments```

```kubectl get pods```

```kubectl get hpa threadservice-hpa```

```kubectl get services```

### Delete resources

```kubectl delete deployment --all --namespace=default```

```kubectl delete all --all -n {namespace}```

```kubectl delete svc <YourServiceName>```

# Docker

## Database

Get image: ```docker pull mongo```

Docker run: ```docker run -d -p 27017:27017 -v data:/data/db --name threaddb mongo```

## ThreadService 

### Build docker image

```docker build -t threadservice .``` (in project folder)

### Pull image from Dockerhub

```docker pull kvdhoogenhof/forumapp-threads```

## Run image

```docker run -d -e ASPNETCORE_ENVIRONMENT=Development –e ASPNETCORE_URLS="https://+:8080;http://+:8081" –e ASPNETCORE_HTTPS_PORT=9000  -e ASPNETCORE_Kestrel__Certificates__Default__Password=mypass123  -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/aspnetapp.pfx -v %USERPROFILE%/.aspnet/https:/https:ro -p 9001:8080 --name api kvdhoogenhof/forumapp-threads```

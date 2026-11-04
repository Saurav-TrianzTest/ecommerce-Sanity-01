# EcommerceWebApi — Deployment Guide

## Overview

This guide covers building, containerizing, and deploying the **EcommerceWebApi** (.NET 6 ASP.NET Core Web API) to **AWS EKS (Elastic Kubernetes Service)**.

- **Technology**: .NET 6, ASP.NET Core Web API
- **Runtime Image**: `mcr.microsoft.com/dotnet/runtime:6.0`
- **Application Port**: `5000` (Kestrel, configurable via `SIGNALR_PORT`)
- **Health Endpoint**: `GET /health`
- **Target Platform**: AWS EKS

---

## Prerequisites

### Local Development
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) 20.10+
- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)

### AWS EKS Deployment
- [AWS CLI v2](https://docs.aws.amazon.com/cli/latest/userguide/install-cliv2.html) configured with appropriate IAM permissions
- [kubectl](https://kubernetes.io/docs/tasks/tools/) 1.24+
- [eksctl](https://eksctl.io/) (optional, for cluster creation)
- An existing EKS cluster with the **AWS Load Balancer Controller** installed
- An ECR repository (auto-created by `build-push.sh`)

### Required IAM Permissions
```
ecr:GetAuthorizationToken
ecr:BatchCheckLayerAvailability
ecr:GetDownloadUrlForLayer
ecr:PutImage
ecr:InitiateLayerUpload
ecr:UploadLayerPart
ecr:CompleteLayerUpload
ecr:CreateRepository
ecr:DescribeRepositories
eks:DescribeCluster
```

---

## Project Structure

```
Test dotnet/
├── Dockerfile                  # Multi-stage Docker build
├── docker-compose.yml          # Local development compose
├── .dockerignore               # Docker build exclusions
├── kubernetes/
│   ├── namespace.yaml          # Kubernetes namespace
│   ├── deployment.yaml         # Application deployment
│   ├── service.yaml            # ClusterIP service
│   └── ingress.yaml            # ALB ingress
├── scripts/
│   ├── build-push.sh           # Linux/macOS build & push
│   ├── build-push.bat          # Windows build & push
│   ├── deploy-image.sh         # Linux/macOS EKS deploy
│   └── deploy-image.bat        # Windows EKS deploy
└── docs/
    └── DEPLOYMENT.md           # This file
```

---

## Local Development with Docker Compose

### 1. Configure Environment Variables

Create a `.env` file in the project root:

```env
APPLICATION_SECRET=your-jwt-secret-key-here
REDIS_CONNECTION_STRING=your-redis-endpoint:6379
```

### 2. Build and Start

```bash
docker compose up --build
```

### 3. Access the Application

- **API Base URL**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health
- **SignalR Hub**: ws://localhost:5000/notificationHub

### 4. Stop the Application

```bash
docker compose down
```

---

## Building and Pushing the Docker Image

### Linux / macOS

```bash
chmod +x scripts/build-push.sh
./scripts/build-push.sh
```

### Windows

```cmd
scripts\build-push.bat
```

The script will prompt you to:
1. Select registry type (AWS ECR or Docker Hub)
2. Enter registry credentials/details
3. Enter an image tag (defaults to `latest`)

The script automatically:
- Sanitizes the image name to lowercase with hyphens
- Creates the ECR repository if it does not exist (ECR only)
- Builds the Docker image from the project root
- Pushes the image to the selected registry

---

## AWS EKS Deployment

### Step 1: Configure kubectl

```bash
aws eks update-kubeconfig --region <AWS_REGION> --name <CLUSTER_NAME>
kubectl cluster-info
```

### Step 2: Install AWS Load Balancer Controller (if not installed)

```bash
# Add the EKS chart repo
helm repo add eks https://aws.github.io/eks-charts
helm repo update

# Install the controller
helm install aws-load-balancer-controller eks/aws-load-balancer-controller \
  -n kube-system \
  --set clusterName=<CLUSTER_NAME> \
  --set serviceAccountAnnotations."eks\.amazonaws\.com/role-arn"=<LBC_IAM_ROLE_ARN>
```

### Step 3: Run the Deployment Script

#### Linux / macOS

```bash
chmod +x scripts/deploy-image.sh
./scripts/deploy-image.sh
```

#### Windows

```cmd
scripts\deploy-image.bat
```

The script will prompt for:
- AWS Region
- EKS Cluster Name
- Full Docker Image URI (e.g., `123456789.dkr.ecr.us-east-1.amazonaws.com/ecommercewebapi:latest`)
- `APPLICATION_SECRET` — JWT signing secret
- `REDIS_CONNECTION_STRING` — Amazon ElastiCache for Redis endpoint (optional)

### Step 4: Verify Deployment

```bash
# Check all resources
kubectl get pods,svc,ingress -n ecommercewebapi

# Check pod logs
kubectl logs -l app=ecommercewebapi -n ecommercewebapi --tail=100

# Describe deployment
kubectl describe deployment ecommercewebapi -n ecommercewebapi
```

### Step 5: Access the Application

```bash
# Get the ALB hostname
kubectl get ingress ecommercewebapi-ingress -n ecommercewebapi \
  -o jsonpath='{.status.loadBalancer.ingress[0].hostname}'
```

Navigate to `http://<ALB_HOSTNAME>/health` to verify the application is running.

---

## Kubernetes Manifest Descriptions

| File | Description |
|------|-------------|
| `namespace.yaml` | Creates the `ecommercewebapi` namespace |
| `deployment.yaml` | Deploys 2 replicas with liveness/readiness probes on `/health` |
| `service.yaml` | ClusterIP service exposing port 80 → container port 5000 |
| `ingress.yaml` | AWS ALB Ingress with internet-facing scheme |

### Resource Limits

| Resource | Request | Limit |
|----------|---------|-------|
| CPU | 250m | 500m |
| Memory | 512Mi | 1Gi |

---

## Configuration Management

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Production` |
| `ASPNETCORE_URLS` | Kestrel listen URL | `http://+:5000` |
| `SIGNALR_PORT` | SignalR/Kestrel port | `5000` |
| `ApplicationSettings__Secret` | JWT signing secret | *(required)* |
| `REDIS_CONNECTION_STRING` | ElastiCache Redis endpoint | *(optional)* |

### Using Kubernetes Secrets (Recommended for Production)

```bash
kubectl create secret generic ecommercewebapi-secrets \
  --from-literal=APPLICATION_SECRET=<your-secret> \
  --from-literal=REDIS_CONNECTION_STRING=<redis-endpoint> \
  -n ecommercewebapi
```

Then reference in `deployment.yaml`:

```yaml
env:
  - name: ApplicationSettings__Secret
    valueFrom:
      secretKeyRef:
        name: ecommercewebapi-secrets
        key: APPLICATION_SECRET
```

---

## Scaling and Management

### Manual Scaling

```bash
kubectl scale deployment ecommercewebapi --replicas=4 -n ecommercewebapi
```

### Horizontal Pod Autoscaler

```bash
kubectl autoscale deployment ecommercewebapi \
  --cpu-percent=70 \
  --min=2 \
  --max=10 \
  -n ecommercewebapi
```

### Rolling Update

```bash
kubectl set image deployment/ecommercewebapi \
  ecommercewebapi=<NEW_IMAGE_URI> \
  -n ecommercewebapi

kubectl rollout status deployment/ecommercewebapi -n ecommercewebapi
```

### Rollback

```bash
kubectl rollout undo deployment/ecommercewebapi -n ecommercewebapi
```

---

## Troubleshooting

### Pod Not Starting

```bash
kubectl describe pod -l app=ecommercewebapi -n ecommercewebapi
kubectl logs -l app=ecommercewebapi -n ecommercewebapi --previous
```

### Health Check Failing

```bash
# Test health endpoint from within the cluster
kubectl run -it --rm debug --image=busybox --restart=Never -n ecommercewebapi -- \
  wget -qO- http://ecommercewebapi-service/health
```

### Ingress Not Provisioning

```bash
# Check ALB controller logs
kubectl logs -n kube-system -l app.kubernetes.io/name=aws-load-balancer-controller

# Verify ingress annotations
kubectl describe ingress ecommercewebapi-ingress -n ecommercewebapi
```

### Redis Connection Issues

```bash
# Verify the Redis connection string is set
kubectl exec -it <pod-name> -n ecommercewebapi -- \
  printenv REDIS_CONNECTION_STRING
```

### Image Pull Errors

```bash
# Verify ECR permissions on the node IAM role
aws ecr get-login-password --region <region> | \
  docker login --username AWS --password-stdin <account>.dkr.ecr.<region>.amazonaws.com
```

---

## Security Considerations

1. **Non-root container**: The application runs as UID 1001 (non-root)
2. **Secrets management**: Use Kubernetes Secrets or AWS Secrets Manager for sensitive values
3. **JWT Secret**: Rotate `ApplicationSettings__Secret` regularly and store in Secrets Manager
4. **Network policies**: Consider adding Kubernetes NetworkPolicy to restrict pod-to-pod traffic
5. **Image scanning**: Enable ECR image scanning on push for vulnerability detection
6. **HTTPS**: Configure ACM certificate ARN in ingress annotations for TLS termination:
   ```yaml
   alb.ingress.kubernetes.io/certificate-arn: arn:aws:acm:<region>:<account>:certificate/<id>
   alb.ingress.kubernetes.io/listen-ports: '[{"HTTPS": 443}]'
   ```

---

## .NET-Specific Notes

- **Serilog**: Logs are written to `/app/Logs/log-<date>.txt` (mapped to `emptyDir` volume in Kubernetes)
- **SignalR**: The `/notificationHub` WebSocket endpoint requires sticky sessions or Redis backplane for multi-replica deployments
- **Redis Backplane**: Set `REDIS_CONNECTION_STRING` to an Amazon ElastiCache for Redis endpoint to enable distributed SignalR
- **JSON Flat File Store**: `database.json` is mounted via a ConfigMap; for production, migrate to a managed database (Amazon RDS or DynamoDB)
- **Swagger**: Disabled in Production by default; enable by setting `ASPNETCORE_ENVIRONMENT=Development` if needed for debugging
- **Culture**: The container uses `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false` to support full globalization

---

*Generated for EcommerceWebApi — .NET 6 ASP.NET Core Web API targeting AWS EKS*

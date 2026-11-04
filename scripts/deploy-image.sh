#!/bin/bash
set -e
set -o pipefail

APP_NAME="ecommercewebapi"
NAMESPACE="ecommercewebapi"
K8S_DIR="kubernetes"

echo "=============================================="
echo "  Deploy EcommerceWebApi to AWS EKS"
echo "=============================================="

# Prompt for AWS and cluster details
echo ""
read -rp "Enter AWS Region (e.g. us-east-1): " AWS_REGION
if [ -z "$AWS_REGION" ]; then
  echo "ERROR: AWS Region is required."
  exit 1
fi

read -rp "Enter EKS Cluster Name: " CLUSTER_NAME
if [ -z "$CLUSTER_NAME" ]; then
  echo "ERROR: EKS Cluster Name is required."
  exit 1
fi

read -rp "Enter full Docker Image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/ecommercewebapi:latest): " IMAGE_URI
if [ -z "$IMAGE_URI" ]; then
  echo "ERROR: Docker Image URI is required."
  exit 1
fi

# Prompt for application-specific environment variables
echo ""
echo "--- Application Environment Variables ---"
echo "(Press Enter to skip any variable)"

read -rp "Enter value for APPLICATION_SECRET (JWT secret key): " APPLICATION_SECRET
if [ -z "$APPLICATION_SECRET" ]; then
  APPLICATION_SECRET="CHANGE_ME_JWT_SECRET"
fi

read -rp "Enter value for REDIS_CONNECTION_STRING (Amazon ElastiCache endpoint, or leave blank): " REDIS_CONNECTION_STRING

echo ""
echo "Configuring kubectl for EKS cluster: $CLUSTER_NAME in $AWS_REGION ..."
aws eks update-kubeconfig --region "$AWS_REGION" --name "$CLUSTER_NAME"

echo "Verifying cluster connectivity..."
kubectl cluster-info || { echo "ERROR: Cannot connect to EKS cluster."; exit 1; }

echo ""
echo "Updating Kubernetes manifests with deployment values..."

# Create working copies of manifests
cp "$K8S_DIR/deployment.yaml" "$K8S_DIR/deployment.yaml.bak"

# Replace placeholders using pipe delimiter
sed -i 's|{{IMAGE_URI}}|'"$IMAGE_URI"'|g' "$K8S_DIR/deployment.yaml"
sed -i 's|{{APPLICATION_SECRET}}|'"$APPLICATION_SECRET"'|g' "$K8S_DIR/deployment.yaml"
sed -i 's|{{REDIS_CONNECTION_STRING}}|'"$REDIS_CONNECTION_STRING"'|g' "$K8S_DIR/deployment.yaml"

echo ""
echo "Applying Kubernetes manifests..."

echo "  [1/4] Applying namespace..."
kubectl apply -f "$K8S_DIR/namespace.yaml"

echo "  [2/4] Applying deployment..."
kubectl apply -f "$K8S_DIR/deployment.yaml"

echo "  [3/4] Applying service..."
kubectl apply -f "$K8S_DIR/service.yaml"

echo "  [4/4] Applying ingress..."
kubectl apply -f "$K8S_DIR/ingress.yaml"

# Restore original deployment.yaml
mv "$K8S_DIR/deployment.yaml.bak" "$K8S_DIR/deployment.yaml"

echo ""
echo "Waiting for deployment rollout..."
kubectl rollout status deployment/"$APP_NAME" -n "$NAMESPACE" --timeout=300s

echo ""
echo "Verifying deployed resources..."
kubectl get pods,svc,ingress -n "$NAMESPACE"

echo ""
echo "Fetching application URL from ingress..."
INGRESS_HOST=$(kubectl get ingress "${APP_NAME}-ingress" -n "$NAMESPACE" -o jsonpath='{.status.loadBalancer.ingress[0].hostname}' 2>/dev/null || echo "pending")
if [ "$INGRESS_HOST" != "pending" ] && [ -n "$INGRESS_HOST" ]; then
  echo "  Application URL: http://$INGRESS_HOST"
else
  echo "  Ingress hostname is still provisioning. Run the following to check:"
  echo "  kubectl get ingress -n $NAMESPACE"
fi

echo ""
echo "=============================================="
echo "  Deployment Complete!"
echo "  Namespace : $NAMESPACE"
echo "  Image     : $IMAGE_URI"
echo "=============================================="
echo ""
echo "Rollback command (if needed):"
echo "  kubectl rollout undo deployment/$APP_NAME -n $NAMESPACE"

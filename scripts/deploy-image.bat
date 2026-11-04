@echo off
setlocal enabledelayedexpansion

set APP_NAME=ecommercewebapi
set NAMESPACE=ecommercewebapi
set K8S_DIR=kubernetes

echo ==============================================
echo   Deploy EcommerceWebApi to AWS EKS
echo ==============================================

echo.
set /p AWS_REGION="Enter AWS Region (e.g. us-east-1): "
if "!AWS_REGION!"=="" (
    echo ERROR: AWS Region is required.
    exit /b 1
)

set /p CLUSTER_NAME="Enter EKS Cluster Name: "
if "!CLUSTER_NAME!"=="" (
    echo ERROR: EKS Cluster Name is required.
    exit /b 1
)

set /p IMAGE_URI="Enter full Docker Image URI (e.g. 123456789.dkr.ecr.us-east-1.amazonaws.com/ecommercewebapi:latest): "
if "!IMAGE_URI!"=="" (
    echo ERROR: Docker Image URI is required.
    exit /b 1
)

echo.
echo --- Application Environment Variables ---
echo (Press Enter to skip any variable)

set /p APPLICATION_SECRET="Enter value for APPLICATION_SECRET (JWT secret key): "
if "!APPLICATION_SECRET!"=="" set APPLICATION_SECRET=CHANGE_ME_JWT_SECRET

set /p REDIS_CONNECTION_STRING="Enter value for REDIS_CONNECTION_STRING (Amazon ElastiCache endpoint, or leave blank): "

echo.
echo Configuring kubectl for EKS cluster: !CLUSTER_NAME! in !AWS_REGION! ...
aws eks update-kubeconfig --region !AWS_REGION! --name !CLUSTER_NAME!
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to configure kubectl.
    exit /b 1
)

echo Verifying cluster connectivity...
kubectl cluster-info
if !ERRORLEVEL! neq 0 (
    echo ERROR: Cannot connect to EKS cluster.
    exit /b 1
)

echo.
echo Updating Kubernetes manifests with deployment values...

copy /Y "%K8S_DIR%\deployment.yaml" "%K8S_DIR%\deployment.yaml.bak" >nul

powershell -Command "(Get-Content '%K8S_DIR%\deployment.yaml') -replace '{{IMAGE_URI}}', '!IMAGE_URI!' -replace '{{APPLICATION_SECRET}}', '!APPLICATION_SECRET!' -replace '{{REDIS_CONNECTION_STRING}}', '!REDIS_CONNECTION_STRING!' | Set-Content '%K8S_DIR%\deployment.yaml'"
if !ERRORLEVEL! neq 0 (
    echo ERROR: Failed to update deployment manifest.
    copy /Y "%K8S_DIR%\deployment.yaml.bak" "%K8S_DIR%\deployment.yaml" >nul
    exit /b 1
)

echo.
echo Applying Kubernetes manifests...

echo   [1/4] Applying namespace...
kubectl apply -f %K8S_DIR%\namespace.yaml
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply namespace. goto :restore )

echo   [2/4] Applying deployment...
kubectl apply -f %K8S_DIR%\deployment.yaml
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply deployment. goto :restore )

echo   [3/4] Applying service...
kubectl apply -f %K8S_DIR%\service.yaml
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply service. goto :restore )

echo   [4/4] Applying ingress...
kubectl apply -f %K8S_DIR%\ingress.yaml
if !ERRORLEVEL! neq 0 ( echo ERROR: Failed to apply ingress. goto :restore )

copy /Y "%K8S_DIR%\deployment.yaml.bak" "%K8S_DIR%\deployment.yaml" >nul
del "%K8S_DIR%\deployment.yaml.bak" >nul 2>&1

echo.
echo Waiting for deployment rollout...
kubectl rollout status deployment/%APP_NAME% -n %NAMESPACE% --timeout=300s
if !ERRORLEVEL! neq 0 (
    echo WARNING: Rollout did not complete within timeout.
)

echo.
echo Verifying deployed resources...
kubectl get pods,svc,ingress -n %NAMESPACE%

echo.
echo ==============================================
echo   Deployment Complete!
echo   Namespace : %NAMESPACE%
echo   Image     : !IMAGE_URI!
echo ==============================================
echo.
echo Rollback command (if needed):
echo   kubectl rollout undo deployment/%APP_NAME% -n %NAMESPACE%

endlocal
goto :eof

:restore
copy /Y "%K8S_DIR%\deployment.yaml.bak" "%K8S_DIR%\deployment.yaml" >nul
del "%K8S_DIR%\deployment.yaml.bak" >nul 2>&1
exit /b 1

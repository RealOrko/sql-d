#!/usr/bin/env bash

set -euo pipefail

echo ''
echo '[SQL-D]:INSTALL/HELM/PROMETHEUS'
echo ''

kubectl -n default apply -f $PWD/scripts/dashboards/aspnetcore.yaml
kubectl -n default apply -f $PWD/scripts/dashboards/aspnetcore-endpoints.yaml

helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo update

# helm install prometheus prometheus-community/kube-prometheus-stack --set prometheus.prometheusSpec.podMonitorSelectorNilUsesHelmValues=false --set prometheus.prometheusSpec.serviceMonitorSelectorNilUsesHelmValues=false 
helm install prometheus prometheus-community/kube-prometheus-stack

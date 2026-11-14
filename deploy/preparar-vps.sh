#!/bin/sh
# Uma vez, como root, antes do primeiro deploy:
#   sh deploy/preparar-vps.sh
set -eu

if [ "$(id -u)" -ne 0 ]; then
  echo "Rode como root."
  exit 1
fi

apt-get update
apt-get install -y rsync ca-certificates curl

if ! command -v docker >/dev/null 2>&1; then
  curl -fsSL https://get.docker.com | sh
fi

if ! docker compose version >/dev/null 2>&1; then
  echo "O plugin docker compose não ficou disponível."
  exit 1
fi

mkdir -p /opt/gevents

if [ "$(awk '/SwapTotal/ {print $2}' /proc/meminfo)" = "0" ]; then
  fallocate -l 2G /swapfile || dd if=/dev/zero of=/swapfile bs=1M count=2048
  chmod 600 /swapfile
  mkswap /swapfile
  swapon /swapfile
  grep -q '/swapfile' /etc/fstab || echo '/swapfile none swap sw 0 0' >> /etc/fstab
fi

echo "Docker pronto. O deploy usa /opt/gevents."

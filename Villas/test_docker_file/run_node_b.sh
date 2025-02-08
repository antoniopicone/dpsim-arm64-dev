docker rm -f villas_node_test

docker rmi test_villas_node:1.0

sudo docker build -t test_villas_node:1.0 -f Dockerfile_node_b .

docker run --cpuset-cpus="0-1" \
  --ulimit rtprio=99 \
  --cap-add=sys_nice \
  --security-opt seccomp=unconfined \
  --privileged \
  --name villas_node_test -it \
  --rm \
  --volume $(pwd):/configs \
  -p 12000:12000/udp test_villas_node:1.0
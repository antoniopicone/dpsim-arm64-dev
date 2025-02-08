docker rm -f villas_node_test

docker rmi test_villas_node:1.0

sudo docker build -t test_villas_node:1.0 -f Dockerfile_node_a .

docker run --name villas_node_test --rm -p 12000:12000/udp --volume $(pwd):/configs test_villas_node:1.0


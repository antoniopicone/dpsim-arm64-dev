# dpsim-arm64-dev
DPSim development environment on Jupyter Lab for arm64

To start the Jupyter Lab environment, run:

```sh
docker run -v ./:/dpsim/jupyterlab -p 8888:8888 -it antoniopicone/dpsim-arm64-dev
```

or, if you have time, build from source with the Dockerfile provided:
```bash
docker build -t dpsim-arm64-dev .

# and, once image is ready:
docker run -v ./:/dpsim/jupyterlab -p 8888:8888 -it dpsim-arm64-dev
```


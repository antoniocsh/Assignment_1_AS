﻿How to run:
Make sure you have docker and docker compose installed, then:

```
docker compose up --build
```

Go to http://localhost

Setup the store with an admin, and in the database section put:

```
server name: nopcommerce_database
db name: nopcommerce
username: sa
password: nopCommerce_db_password
```

Then wait some time (~ 2min) for the installation, stop the containers (Ctrl + C), and then again.

```
docker compose up
```

Grafana:

Go to http://localhost:3001 and there is the Grafana dashboard.

Use the search feature and go to product's pages in http://localhost and then check the dashboard.

Jaeger:

Go to http://localhost:16686/ to see jaeger traces.

To run the load tests:

docker run --rm --add-host=host.docker.internal:host-gateway -i grafana/k6 run - < loadtest/load-test.js
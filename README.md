﻿How to run:
Make sure you have docker and docker compose installed, then:

docker compose up --build

go to http://localhost

setup the store with an admin, and in the database section put:

server name: nopcommerce_database
db name: nopcommerce
username: sa
password: nopCommerce_db_password

then wait a bit for the installation, do docker compose down and then docker compose up again

then, go to http://localhost:3001 and there is the grafana dashboard

use the search feature in http://localhost and then check the dashboard.

To run the load tests:

docker run --rm --add-host=host.docker.internal:host-gateway -i grafana/k6 run - < loadtest/load-test.js
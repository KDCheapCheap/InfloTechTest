# Local MySQL

## Requirements

- Docker Desktop installed and running, with Docker Compose available.

## Start

From this folder, run `start-db.bat`. The script:

1. Runs `docker compose up -d` using the Compose file in this folder. Docker builds the configured MySQL image if needed and starts the `local-mysql` container in the background. The container publishes port `3306` on your machine and stores database files in the `mysql-data` Docker volume.
2. If the Compose command fails, prints an error, pauses so the message remains visible, and exits with status `1`.
3. If Compose succeeds, runs `mysqladmin ping` inside `local-mysql` with the configured root password. It repeats this check every two seconds until MySQL responds. There is no timeout, so the script keeps waiting if the container never becomes ready. Press `Ctrl+C` to stop waiting.
4. Prints the connection settings below and pauses for a keypress before closing.

The script does not create application tables or run Entity Framework migrations. It only starts the database and waits for MySQL to accept its readiness check. The first time MySQL initializes the empty data volume, the Compose environment settings create the `myapp` database and `myappuser` account.

Connect using:

| Setting | Value |
| --- | --- |
| Host | `localhost` |
| Port | `3306` |
| Database | `myapp` |
| Username | `myappuser` |
| Password | `myapppassword` |

## Stop and Keep Data

To stop and remove the container and Compose network while preserving the database volume, run this from this folder:

```bat
docker compose down
```

Run `start-db.bat` again to recreate and start the container with the existing database data.

## Remove Everything

From this folder, run `teardown-db.bat`. The script asks you to confirm:

- Enter `Y` to run `docker compose down --volumes --rmi local`. This stops and removes the Compose containers and network, removes the database volume, and removes local images built for the Compose services. Removing the volume permanently deletes the database data. **This cannot be undone.**
- Enter `N` to cancel. No teardown command is run.

If the teardown command fails, the script prints an error, pauses, and exits with status `1`. On success, it prints a completion message and pauses before closing. The script does not remove unrelated containers, volumes, or images.
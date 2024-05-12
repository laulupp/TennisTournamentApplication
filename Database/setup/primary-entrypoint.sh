#!/bin/bash
#PG_HBA_CONF_PATH="/etc/postgresql/16/main/pg_hba.conf"
#echo "Path to pg_hba.conf: $PG_HBA_CONF_PATH"

# Replace all peer and ident entries with md5, excluding comments
#sed -ri "s/(peer|ident)$/trust/" "$PG_HBA_CONF_PATH"
#sed -ri "s/(replication|ident)$/all/" "$PG_HBA_CONF_PATH"

cp /setup/templates/conf.d/10replication.main.conf.template /etc/postgresql/16/main/conf.d/10replication.main.conf
sudo -u postgres cp /etc/postgresql/16/main/conf.d/10replication.main.conf /etc/postgresql/16/main/conf.d/10replication.conf

cp /setup/templates/pg_hba.main.conf.template /etc/postgresql/16/main/pg_hba.main.conf
sudo -u postgres cp /etc/postgresql/16/main/pg_hba.main.conf /etc/postgresql/16/main/pg_hba.conf

service postgresql start

echo 1 | /bin/bash /setup/setup_db.sh
#
# pg_conftool set log_statement all
#
#service postgresql restart
#
touch /tmp/db_ready
#
tail -F /var/log/postgresql/postgresql-16-main.log
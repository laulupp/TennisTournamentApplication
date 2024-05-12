FROM db_base:latest

COPY setup /setup
COPY setup/primary-entrypoint.sh /bin
COPY setup/wait-db-ready.sh /bin

RUN sed -i 's/\r$//' /setup/setup_db.sh
RUN sed -i 's/\r$//' /setup/templates/conf.d/10replication.main.conf.template
RUN sed -i 's/\r$//' /setup/templates/pg_hba.main.conf.template
RUN sed -i 's/\r$//' /bin/primary-entrypoint.sh
RUN sed -i 's/\r$//' /bin/wait-db-ready.sh

ENTRYPOINT ["/bin/bash", "/bin/primary-entrypoint.sh"]
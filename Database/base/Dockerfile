FROM ubuntu:22.04
RUN apt-get update && \
	apt-get -y install wget gnupg sudo && \
	echo "deb http://apt.postgresql.org/pub/repos/apt jammy-pgdg main" > /etc/apt/sources.list.d/pgdg.list && \
	wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | apt-key add - && \
	apt-get update && \
	DEBIAN_FRONTEND=noninteractive apt-get -y install postgresql-16
EXPOSE 5432
sudo apt-get update
sudo apt install  openjdk-8-jdk-headless
sudo apt-get install postgresql postgresql-contrib

sudo -u postgres psql

sudo wget -O /etc/apt/sources.list.d/ambari.list http://public-repo-1.hortonworks.com/ambari/ubuntu16/2.x/updates/2.6.1.5/ambari.list
sudo apt-key adv --recv-keys --keyserver keyserver.ubuntu.com B9733A7A07513CAD
sudo apt-get update
sudo apt-get install ambari-server
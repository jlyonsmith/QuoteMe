# Install Service Instance

## Adding an Ubuntu User on non-EC2 instances

We use an `ubuntu` user which has root access, like Amazon does with EC2.  First, `ssh` to the system as `root`, logging on with the provided password.

Create the `ubuntu` user:

    adduser  ubuntu 

Use [Strong Password Generator](http://strongpasswordgenerator.com/) to generate a password.

Now add the ubuntu user as a sudoer:

    cd /etc/sudoers.d
    vi ubuntu

Add the following:

```
# Add sudo permission for ubuntu user
ubuntu ALL=(ALL) NOPASSWD:ALL

```

Now change the permissions on the file:

    chmod 440 ubuntu

See the [Sudo Manual](http://www.sudo.ws/sudoers.man.html) for more information.

Create a `.pem` file without a password for SSH'ing into your system:

    ssh-keygen -t rsa -b 2048 -f ubuntu 
    mv ubuntu ubuntu.pem

Or, if you already have a .pem file with no public key:

    ssh-keygen -f ubuntu.pem -y > ubuntu.pub

SSH in again as `ubuntu`.  Create the `.ssh` directory:

    mkdir .ssh
    chmod u=rwx,go= .ssh

Copy up the `.pub` file:

    scp ubuntu.pub ubuntu@10.10.10.10:.ssh/

Concat the `.pub` file to the authorized keys file:

    cat ubuntu.pub >> ~/.ssh/authorized_keys
    chmod u=r,go= ~/.ssh/authorized_keys

Exit and re-connect as `ubuntu`.  You should not require a password.  Check you can `sudo` without a password.

## Install MongoDB and Mono

These are instructions for installing MongoDB, Mono and Apache2 for ServiceStack based services.

For long running operations where you might lose `ssh` connectivity (or you just want to disconnect)you can use `screen`:

	screen
	
After logging on and before running the long running operation, then:

	screen -r
	
After reconnecting.

First, become super-user permanently:

	sudo -s

Install Git:

	apt-get install git

### MongoDB

Install MongoDB:

	apt-key adv --keyserver keyserver.ubuntu.com --recv 7F0CEB10
	echo 'deb http://downloads-distro.mongodb.org/repo/ubuntu-upstart dist 10gen' | tee /etc/apt/sources.list.d/10gen.list
	apt-get update
	apt-get install mongodb-10gen 
 	
### Redis

Install Redis:

    apt-get update
    apt-get upgrade
    apt-get redis-server

### Mono

Install Mono build pre-requisites:

	apt-get install vim libperl-dev libgtk2.0-dev autoconf automake libtool g++ gettext mono-gmcs git make apache2 apache2-threaded-dev


Build LibGdiPlus/Mono/XSP/Mod-Mono:

	cd /opt
	
To make `libgdiplus` (needed by `mono`):
	
	git clone git://github.com/mono/libgdiplus.git 
	cd libgdiplus 
	apt-get install libjpeg-dev libexif-dev
	./autogen.sh --prefix=/usr
	make 
	make install 
	make clean 

To make `mono`:	

	git clone git://github.com/mono/mono.git 
	cd mono
	./autogen.sh --prefix=/usr 
	make 
	make install 
	make clean 
	cd ..

To make `xsp`:

	git clone git://github.com/mono/xsp.git 
	cd ../xsp 
	./autogen.sh --prefix=/usr 
	make 
	make install 
	make clean 
	cd ..

To make `mod_mono`:

	git clone git://github.com/mono/mod_mono.git 
	cd mod_mono 
	./autogen.sh --prefix=/usr 
	make 
	make install 
	make clean 
		
## Firewall

### EC2

Set the EC2 security group ensuring that a policy that only allows access to the following ports is in place:

Service | Port
:-- | :--
HTTP | 80
HTTPS | 443
SSH | 22
MongoDB | 27017
MosaicService | 1337 - 1347
ICMP | 0 - 65535

### Ubuntu

Use [Uncomplicated Firewall](https://help.ubuntu.com/12.10/serverguide/firewall.html):

    sudu -s
    ufw allow 22
    ufw enable 
    ufw insert 1 allow 80
    exit

Add additional rules as necessary.

## Redis

The service requires Redis to be up and running.  If it is running locally, check it with:

    redis info

and ensure that it returns information about the local instance.

## MongoDB 

Configuring MongoDB for the first requires setting up an `admin` database.  On newly installed system, MongoDB security should be off by default:

	cat /etc/mongodb.conf` | grep auth

Should contain `noauth true`.  Now create an `admin` database

	mongodb

Then:

	use admin
	db.addUser({user:"root",pwd:"...",roles:["readWrite", "dbAdmin", "userAdminAnyDatabase", "clusterAdmin"]})
	db.system.users.find()
	use capsule-vM-m
	db.addUser({user:"admin",pwd:"...",roles:["readWrite", "dbAdmin", "userAdmin"]})
	db.addUser({user:"user",pwd:"...",roles:["readWrite", "dbAdmin"]})
	db.system.users.find()

For system security, use a new password for each database.

Now enable security on the MongoDB instance by adding `auth true` to the `mongodb.conf` file, then do:

	sudo service mongodb restart

You will require authentication to connect to the database from now on.

## Apache

Configure Apache using `mod_proxy` by enabling it:

	sudo a2enmod proxy
	
Create or edit the web site at `/etc/apache2/sites-available/mosaic-api`

	ProxyPass /v1.2 http://127.0.0.1:1338/ retry=0 max=50
	ProxyPassReverse /v1.2 http://127.0.0.1:1338/
	
	ProxyPass /v1.1 http://127.0.0.1:1337/ retry=0 max=50
	ProxyPassReverse /v1.1 http://127.0.0.1:1337/

	<VirtualHost *:80>
		ServerName api.artifacttech.com
		Redirect / http://api.artifacttech.com/v1.2
    	ErrorLog ${APACHE_LOG_DIR}/error.log
    	LogLevel warn
		CustomLog ${APACHE_LOG_DIR}/access.log combined
	</VirtualHost>
	
Ensure that it is enabled:

	sudo a2ensite mosaic-api
	sudo service apache2 restart

## MosaicService

Add a new non-interactive user called `mosaic` to run the service as:

	sudo useradd -G mosaic mosaic
	sudo passwd mosaic
	sudo chsh -s /bin/false mosaic

Do **not** give this user `sudo` rights.  

Add the public key for the `mosaic.pem` to the `~/.ssh/authorized_keys` file.

Ubuntu uses [Upstart](http://upstart.ubuntu.com/getting-started.html) to manage daemons.  Configuration files for Upstart daemons are in the `/etc/init.d` directory.  Upstart currently works alongside the existing Unix `init` daemon process on Ubuntu.

Create the following Upstart file in `/etc/init/mosaic-vM.m.conf`:

	kill timeout 300

	description "Artifact Mosaic Service vM.m"
	author "Artifact Technologies"

	start on runlevel [2345]
	stop on runlevel [06]
	
	setuid mosaic

	script
	  /home/mosaic/bin/mosaicservice-vM-m.sh
	end script

Create the following Upstart file in `/etc/init/publish-vM.m.conf`:

	kill timeout 300

	description "Artifact Publish Service vM.m"
	author "Artifact Technologies"

	start on runlevel [2345]
	stop on runlevel [06]
	
	setuid mosaic

	script
	  /home/mosaic/bin/publishservice-vM.m
	end script

Passwords are kept in the [Redmine Infrastructure Wiki](http://redmine.artifacttech.com/projects/infrastructure/wiki/Passwords).  Genarate new strong 16 character passwords for each instance using [Password Generator](http://www.newpasswordgenerator.com/).

Next, build and deploy the software to the system using the `mosaic` user.

	cd MosaicService
	bin/deploy machine

Where `machine` is a name configured in `~/.ssh/config`:

	Host machine
	  User ubuntu
	  HostName ec2-10-10-10-10.us-west-2.compute.amazonaws.com
	  IdentityFile ~/.ssh/yourprivatekey.pem

Test the service by going to `http://api.artifacttech.com/vM.m/info` in your browser.

Make sure that you configure the database name to have a version number in the `app.config` file (use dashes not dots).

	<?xml version="1.0" encoding="utf-8"?>
	<configuration>
	    <appSettings>
    	    <add key="MongoDbUrl" value="mongodb://admin:fPJH3277uaT115O@localhost:27017/capsule-v1-2" />
	        ...
    	</appSettings>
	</configuration>


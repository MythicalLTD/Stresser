# Stresser
A free l7 stresser for educational and testing purposes only!


# !! WARRNING !!
This is just for educational and testing purposes only!
NEVER USE IT ON SERVERS THAT ARE NOT YOURS!

Me (NaysKutzu) and (MythicalSystems) are not responsible for any damage.


# Why no proxy support?
Proxy makes it be slow and laggy asf thats why you have suppport for an api based attack so you can use it to attack with real machines and real power!


# Installtion
To install this just login inside your server and follow those commands 

```bash
cd /root
wget https://github.com/MythicalLTD/Stresser/releases/latest/download/StresserAMD

# Set the permissions
chmod u+x StresserAMD

# Create a systemd service to make it run in the background 
sudo nano /etc/systemd/system/stresser.service

###########################################################

[Unit]
Description=Stresser Service
After=network.target

[Service]
Type=simple
ExecStart=/root/StresserAMD
Restart=always

[Install]
WantedBy=multi-user.target

###########################################################

sudo systemctl daemon-reload
sudo systemctl enable stresser.service
sudo systemctl start stresser.service
```

And now just make sure you configure your settings inside `/root/config.ini`!


# Are there any nodes i can get for free for testing?
Sure
```text
141.144.253.29:1492 Z4kok2LVferBPQ39rBhkywJjthFmehik
158.178.196.227 Z4kok2LVferBPQ39rBhkywJjthFmehik
158.180.32.161 Z4kok2LVferBPQ39rBhkywJjthFmehik
```

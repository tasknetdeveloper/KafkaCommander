# KafkaCommander
KafkaCommander - this is example of work with command line on remote computer using Kafka

![screen1](https://user-images.githubusercontent.com/56368289/166828176-a22f0a2e-20cc-497d-a9f1-0551bab6b46a.png)

You need to use two clients.
Theese should send messages each other.
Theese must tohave different config files (look appsettings.json file).

For example, first client have config:


  "Settings": {
    "urlKafka": "..insert here url of kafka server",
    "userName": "user0",
    "isConsole": "false",
    "password": "user0pswd",
    "token": "sdfsdfsdf",
    "destination": [
      {
        "name": "user1",
        "password": "user1pswd", 
        "token": "asd#@sdfddfgdfg"
      }
    ]
  }


and second client have config:

  "Settings": {
    "urlKafka": "..insert here url of kafka server",
    "userName": "user1",
    "isConsole": "true",
    "password": "user1pswd",
    "token": "sdfsdfsdf",
    "destination": [
      {
        "name": "user0",
        "password": "user0pswd", 
        "token": "asd#@sdfddfgdfg"
      }
    ]
  }


P.S. Destination is clients, which will recieve message from sender client.
Only one user can use the command line, for this in the config, you must specify
"isConsole" = "true"
The rest of the clients must have
"isConsole" = "false"

For example, you can use two destiantions:

    "destination": [
      {
        "name": "user0",
        "password": "user0pswd", 
        "token": "asd#@sdfddfgdfg"
      },
      {
        "name": "user2",
        "password": "user2pswd", 
        "token": "asd#@sdfddfgdfg"
      }
    ]


Now, release have two commands.
getDrive - it show all drives of remote computer.
getDir - it show all folders and files by path of remote computer.
In future the list of commands will have more...

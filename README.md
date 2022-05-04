# KafkaCommander
KafkaCommander - this is example of work with command line on remote computer using Kafka

You need to use two clients.
Theese should send messages each other.
Theese must tohave different config files (look appsettings.json file).

For example, first client have config:

{
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
}

and second client have config:

{
  "Settings": {
    "urlKafka": "..insert here url of kafka server",
    "userName": "user1",
    "isConsole": "false",
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
}


p.s. destination is client, which will recieve message from sender client

Now, release have two commands.
getDrive - it show all drives of remote computer.
getDir - it show all folders and files by path of remote computer.
In future the list of commands will have more

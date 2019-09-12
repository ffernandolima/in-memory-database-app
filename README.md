# in-memory-database-app

In-memory database app is an implementation of a database which uses only memory for storing data. 

It implements some commands from the very known in-memory database called Redis (https://redis.io/).

It supports the following commands:

SET
GET
DEL
DBSIZE
INCR
ZADD
ZCARD
ZRANK
ZRANGE

Simplyfing the way other applications consume the database, it was designed to receive and reply back to the consumer always using strings:

- All keys and values are ASCII strings from the set [a-zA-Z0-9_]
- Commands are ASCII strings delimited by white-space
- Responses are ASCII strings delimited by white-spaces when it's needed

There's a web server built with docker which allows many consumers at the same time. The database is exposed through http protocol and connections should be established using the port 8080

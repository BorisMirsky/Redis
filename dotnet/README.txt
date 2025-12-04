


Swagger:   http://localhost:5138/swagger/index.html
Browser:   https://localhost:7200


CredentialsSettings
- Содержит password и строку доступа вида "redis-..................cloud.redislabs.com:..."
- занесен в gitignore (скрыт)


Есть 2 подхода в работе с Redis: 
- Если Redis используется просто как кеш то IDistributedCache.
- Если Redis используется как хранилище данных и нужны его расширенные возможности (хотя бы даже CRUD) то ConnectionMultiplexor.

У меня простейший случай, но сделано с IConnectionMultiplexer.



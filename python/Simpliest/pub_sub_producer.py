import redis
from hostname import hostname, password



"""
«Pub/Sub» (Публикация/Подписка).
Один сервис (или несколько сервисов) публикует в отдельную очередь свое сообщение,
после чего его могут обработать только те получатели, которые подписаны на эту очередь.
В том случае, если на очередь никто не подписан, сообщение будет утеряно.

Сначала запуск отправителя консамера, потом продюсера
Запуск из двух разных консолей
"""


connection = redis.Redis(
    host=hostname,
    port=15653,
    decode_responses=True,
    username="default",
    password=password
)



connection.publish('channelFirst',
                   'Данное сообщение было отправлено в первый канал')

connection.publish('channelSecond',
                   'Данное сообщение было отправлено во второй канал')



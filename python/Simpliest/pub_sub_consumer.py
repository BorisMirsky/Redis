
import redis
from hostname import hostname, password
import time


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


queue = connection.pubsub() # создаем очередь типа Pub/Sub
queue.subscribe("channelFirst", "channelSecond") # подписываемся на указанные каналы

# бесконечный цикл обработки очереди сообщений
while True:
    time.sleep(1)
    msg = queue.get_message() # извлекаем сообщение
    if msg:
        if not isinstance(msg["data"], int):
            print(msg["data"]) 
    else:
        print("...")

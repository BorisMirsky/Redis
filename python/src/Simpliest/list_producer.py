
import redis
from hostname import hostname, password
import time
import random


"""
List» (Список). Очередь типа FIFO (первым пришел — первым ушел).
После отправки сервисом сообщения его получит только один подписчик.

Сначала запуск продюсера, потом консамера
"""


connection = redis.Redis(
    host=hostname,
    port=15653,
    decode_responses=True,
    username="default",
    password=password
)


for i in range(0,10):
    msg = "Сообщение №" + str(random.randint(0, 100))
    print(msg)
    connection.lpush("listQueue", msg)


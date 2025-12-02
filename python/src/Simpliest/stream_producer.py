
import redis
from hostname import hostname, password
import time
import random


"""
«Stream» (Поток). Тоже самое, что и Pub/Sub, но с гарантией доставки сообщения.
То есть при отсутствии чтения сообщение остается в очереди и ждет обработки.

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
    connection.xadd("queueStream", { "data":"Сообщение №" + str(random.randint(0, 100))})
print("Длина очереди: " + str(connection.xlen("queueStream"))) 

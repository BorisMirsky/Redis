
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


#queue = connection.pubsub() # создаем очередь типа Pub/Sub
#queue.subscribe("channelFirst", "channelSecond") # подписываемся на указанные каналы

# бесконечный цикл обработки очереди сообщений
#len = connection.llen("listQueue") # получаем размер листа очереди сообщений

# читаем сообщения из листа до тех пор, пока размер листа не станет равен нулю
while connection.llen("listQueue") != 0:
    msg = connection.rpop("listQueue") # читаем сообщение, которое является типом данных словаря
    if msg:
        print(msg) 






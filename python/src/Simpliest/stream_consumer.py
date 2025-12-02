
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

"""
# Вывод в консоль всех сообщений из потока
len_ = connection.xlen("queueStream") 
if len_ > 0:
    messages = connection.xread(count=len_, streams={"queueStream":0})
    for msg in messages:
        print(msg) 
"""

# Вывод только новых сообщений
# создаем переменную Redis для хранения ID последнего сообщения
#(если эта переменная еще не существует)
if connection.get("last") == None:
	connection.set("last", 0)

len_ = connection.xlen("queueStream") 

if len_ > 0:
    messages = connection.xread(count=len_, block=1000, streams={"queueStream":connection.get("last")})
    print(connection.get("last")) 
    for msg in messages:
        print(msg)
        connection.set("last", msg[-1][-1][0])


#  не отработал?!






















import redis
import time
import sys
from hostname import hostname, password




r = redis.Redis(
    host=hostname, 
    port=15653,
    decode_responses=True,
    username="default",
    password=password
)

r.set('ip_address', '127.0.0.0')
r.set('timestamp', int(time.time()))
r.set('user_agent', 'Mozilla/5.0 (Macintosh; Intel Mac OS X 11)')
r.set('last_page_visited', 'home')
#result = r.get('last_page_visited')
#print(result)

record = {
    "name": "PythonRu",
    "description": "Redis tutorials",
    "website": "https://google.com/"
}
#r.hset('business', record)
#print(f"business: {r.hgetall('business')}")

r.set('index', '666')
print(f"index: {r.get('index')}")


"""
5 типов данных и операции с ними.
Создаётся подключение 'r' и все операции через 'r'
"""



from datetime import datetime
import random
import redis
import time
from time import sleep
from hostname import hostname, password


connection = redis.Redis(
    host=hostname,
    port=15653,
    decode_responses=True,
    username="default",
    password=password
)


# mock data
districts = ['Arbat', 'Basmannyi', 'Zamoskvoreche', 'Khamovniki', 'Yakimanka', 'Tverskoy','Taganskyi', 'Sokol', 'Khovrino', 'Aeroport', 'Bibirevo', 
            'Lianozovo', 'Sviblovo', 'Rostokino', 'Ostankino','Perovo', 'Izmailovo', 'Sokolniki', 'Golyanovo','Strogino','Mitino', 'Kurkino',
            'Vnukovo', 'Ramenki', 'Butovo','Vikhino', 'Lefortovo', 'Capotnya', 'Brateevo','Donskoy','Orekhovo-Borisovo']

# случайный выбор района и координат клиента - имитация клиентского запроса        
point = random.choice(districts)
coordinate_x = 55+(random.randint(1, 10)/10)+(random.randint(1, 100)/100)
coordinate_y = 37+(random.randint(1, 10)/10)+(random.randint(1, 100)/100)
dist=0
dist_unit='km'

now=datetime.now()

print('На ', now.strftime("%m/%d/%Y %H:%M:%S"),' запрос из района ', point, ', примерные координаты клиента ', coordinate_x, coordinate_y, '. Ищем машины в радиусе ', dist, ' км')

#ищем варианты такси в Redis для клиента со сгенерированным случайным образом районом и координатами 
result = []
for i in range(10): # 10 попыток расширения радиуса поиска
    dist = dist+random.randint(i, 10)
    result = connection.georadius(point, coordinate_x, coordinate_y, dist, unit=dist_unit, withdist=True, withcoord=True)
    print('Радиус поиска: ', dist, dist_unit)
    if result: # если есть min 1 результат
        result_str = []
        for item in result:
            if isinstance(item, bytes):
                result_str.append(item.decode('utf-8'))
            else:
                result_str.append(item)
        result_str = str(result).replace("b",' ').replace('\'', '\"')
        print('Найдены варианты такси: ', result_str)
        break
if not result: # результатов нет за 10 попыток расширения радиуса
    print('Машин в радиусе ', dist, ' км не найдено')



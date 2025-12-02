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
taxis = ['Honda', 'Ford', 'Renault', 'Mazda', 'Haval', 'Jeep', 'Toyota', 'Lexus', 'Opel', 'Niva', 'Volga', 'Nissan', 'Lada','Tesla']


while True:
    now=datetime.now()  
    district = random.choice(districts)
    cars = random.randint(0, 10)
    if cars > 0 :
        print('На ', now.strftime("%m/%d/%Y %H:%M:%S"), ' в районе ', district, ' находится ', cars, ' машин такси')
        for i in range(cars):
            taxi = random.choice(taxis)+'_'+str(random.randint(100, 999))
            coordinate_x = 55+(random.randint(1, 10)/10)+(random.randint(1, 100)/100)
            coordinate_y = 37+(random.randint(1, 10)/10)+(random.randint(1, 100)/100)
            #запись данных в Redis
            connection.geoadd(district, [coordinate_x, coordinate_y, taxi], nx=False)
            connection.expire(district, 600)
            print(i+1,'-ое такси ', taxi, ' с координатами ', coordinate_x, coordinate_y)
  
    time.sleep(3)

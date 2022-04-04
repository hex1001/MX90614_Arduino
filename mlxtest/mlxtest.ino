#include <Adafruit_MLX90614.h>
#include <ArduinoRS485.h> 
#include <ArduinoModbus.h>

Adafruit_MLX90614 mlx = Adafruit_MLX90614();
typedef union temp
{
    float t;
    uint16_t *t_t;    
};
union temp tA, tO;
//float temp1[2] = {0, 0};
//float temp2[2] = {0, 0};
uint16_t temp[2] = {0, 0};

const uint8_t numInputRegisters = 2;


void setup() {
  Serial.begin(9600);
  while (!Serial);
  if (!ModbusRTUServer.begin(1, 9600)) {
    //Serial.println("Failed to start Modbus RTU Server!");
    while (1);
  }  
  ModbusRTUServer.configureInputRegisters(0x00, numInputRegisters);

  //Serial.println("Adafruit MLX90614 test");

  if (!mlx.begin()) {
    //Serial.println("Error connecting to MLX sensor. Check wiring.");
    while (1);
  };

  //Serial.print("Emissivity = "); Serial.println(mlx.readEmissivity());
  //Serial.println("================================================");
}

void loop() {
  //tA.t = mlx.readAmbientTempC(); 
  temp[0] = mlx.readAmbientTempC()*100; 
  //tO.t = mlx.readObjectTempC(); 
  temp[1] = mlx.readObjectTempC()*100; 
  //Serial.print("Ambient = "); Serial.print(temp[0]);
  //Serial.print("*C\tObject = "); Serial.print(temp[1]); Serial.println("*C");
  //Serial.print("Ambient = "); Serial.print(mlx.readAmbientTempF());
  //Serial.print("*F\tObject = "); Serial.print(mlx.readObjectTempF()); Serial.println("*F");
  for (int i = 0; i < 2; i++)
  {  
      ModbusRTUServer.poll();
      ModbusRTUServer.inputRegisterWrite(i, temp[i]); 
      //ModbusRTUServer.inputRegisterWrite(0, tO.t_t[0]); 
      //ModbusRTUServer.poll();
      //ModbusRTUServer.inputRegisterWrite(1, tO.t_t[1]); 
  } 
  //Serial.println(tO.t_t[0]);
  //Serial.println(tO.t_t[1]);
  //delay(500);
}

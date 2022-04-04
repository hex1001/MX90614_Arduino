using System;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using EasyModbus;
namespace MLX90614_ARDUINO
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int COUNT = 100000;
        string com;
        volatile int count = 0;
        SerialPort serial;
        string[] myPort;
        ModbusClient mb;
        bool start = false;
        public MainWindow() {
            InitializeComponent();
            
            myPort = SerialPort.GetPortNames();
            foreach (string ss in myPort)
                comboPort.Items.Add(ss);
            
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            btnStop.IsEnabled = true;
            btnStart.IsEnabled = false;
            Reading();

        }
        
        async public void Reading() {            
            String[] str = new String[2];
            try {
                com = comboPort.SelectedValue.ToString();
                serial = new SerialPort(com);               
                mb = new ModbusClient(com);
                mb.UnitIdentifier = 1;
                mb.Baudrate = 9600;
                mb.Parity = System.IO.Ports.Parity.None;
                mb.StopBits = System.IO.Ports.StopBits.One;
                mb.ConnectionTimeout = 500;
                mb.Connect();
                start = true;
               
                //MessageBox.Show(com);

            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            await Task.Delay(100);
            while (start) { 
                if (count > COUNT) {
                    mb.Disconnect();
                    serial = new SerialPort(com);
                    mb = new ModbusClient(com);
                    mb.UnitIdentifier = 1;
                    mb.Baudrate = 9600;
                    mb.Parity = System.IO.Ports.Parity.None;
                    mb.StopBits = System.IO.Ports.StopBits.One;
                    mb.ConnectionTimeout = 500;
                    mb.Connect();
                    count = 0;
                }
                str[0] = @"Object temprature: " + (mb.ReadInputRegisters(1, 1)[0]/100.0).ToString();
                str[1] = @"Ambient temprature: " + (mb.ReadInputRegisters(0, 1)[0]/100.0).ToString();
               
                tAmbient.Content = str[1];
                tObject.Content = str[0];
                tObject_Copy.Content = @"Count: " + count.ToString();
                await Task.Delay(100);
                count++;
            }           
            count = 0;
            mb.Disconnect();
        }

        private void Button_STOP_Click(object sender, RoutedEventArgs e) {
            start = false;
            mb.Disconnect();
            btnStop.IsEnabled = false;
            btnStart.IsEnabled = true;
            if (serial.IsOpen) {
                MessageBox.Show("Com port is NOT closed!");
            } else {
                //MessageBox.Show("Com port is closed!");
            }
        }
    }
}

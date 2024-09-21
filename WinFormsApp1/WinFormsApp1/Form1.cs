using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        private TcpListener server;
        private Thread serverThread;
        private TcpClient client;
        private NetworkStream stream;
        bool flag_on = false;
        byte[] dataToProcess = new byte[39];
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Tạo và khởi động luồng để lắng nghe kết nối từ client
            serverThread = new Thread(StartServer);
            serverThread.Start();
            //timer1.Tick += new EventHandler(timer1_Tick);
            //timer1.Enabled = true;
        }

        private void StartServer()
        {

                try
                {
                    // Địa chỉ IP và cổng mà server sẽ lắng nghe
                    int port = 502;
                    IPAddress localAddr = IPAddress.Parse("192.168.0.5");

                    // Tạo đối tượng TcpListener để lắng nghe kết nối từ client
                    server = new TcpListener(localAddr, port);

                    // Bắt đầu lắng nghe
                    server.Start();
                    AppendText("Server đang lắng nghe trên cổng " + port);

                    // Chấp nhận kết nối từ client
                    client = server.AcceptTcpClient();
                    AppendText("Client đã kết nối.");

                    // Lấy đối tượng NetworkStream để đọc dữ liệu
                    using (stream = client.GetStream())
                    {
                        // Tạo mảng byte để lưu dữ liệu đọc được
                        byte[] receivedData = new byte[100];
                        while (true)
                        {
                            // Đọc dữ liệu từ stream
                            int bytesRead = stream.Read(receivedData, 0, receivedData.Length);

                            // Xử lý dữ liệu đọc được (chỉ đọc 39 byte đầu tiên như trong ví dụ)
                            
                            Array.Copy(receivedData, 0, dataToProcess, 0, Math.Min(39, bytesRead));

                            // Hiển thị dữ liệu đọc được dưới dạng hex
                            StringBuilder hexData = new StringBuilder();
                            for (int i = 0; i < dataToProcess.Length; i++)
                            {
                                hexData.Append(dataToProcess[i].ToString("X2") + " ");
                            }
                            AppendText("Dữ liệu nhận được: " + hexData.ToString());
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendText("Lỗi: " + ex.Message);
                }
                finally
                {
                    // Đóng server

                }
        }

        private void AppendText(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendText), text);
                return;
            }
            textBox1.AppendText(text + Environment.NewLine);
        }

        private void AppendText2(string text)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(AppendText), text);
                return;
            }
            textBox2.AppendText(text + Environment.NewLine);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            flag_on = false;
            AppendText("Đóng kết nối.");
            client.Close();
            server?.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            flag_on = true;
            byte[] sendData = dataToProcess;
//            byte[] sendData = new byte[]{
//    0x01, 0x01, 0x00, 0x00, 0x00, 0x06, 0x01, 0xE1, 0x52, 0xD6, 0x22, 0x85, 0x76, 0x2B, 0xD0, 0xC7,
//    0x7A, 0xD9, 0xCC, 0x95, 0x17, 0x47, 0xD1, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
//    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
//};
            stream.Write(sendData, 0, 39);
            stream.Flush();
            AppendText("Gui xong");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (flag_on)
            {
                try
                {
                    byte[] receivedData = new byte[100];
                    int bytesRead = stream.Read(receivedData, 0, receivedData.Length);

                    // Xử lý dữ liệu đọc được (chỉ đọc 39 byte đầu tiên như trong ví dụ)
                    byte[] dataToProcess = new byte[39];
                    Array.Copy(receivedData, 0, dataToProcess, 0, Math.Min(39, bytesRead));

                    // Hiển thị dữ liệu đọc được dưới dạng hex
                    StringBuilder hexData = new StringBuilder();
                    for (int i = 0; i < dataToProcess.Length; i++)
                    {
                        hexData.Append(dataToProcess[i].ToString("X2") + " ");
                    }
                    AppendText2("Dữ liệu nhận được: " + hexData.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Lỗi cập nhật đồ thị: " + ex.Message);
                }
            }
        }
    }
}

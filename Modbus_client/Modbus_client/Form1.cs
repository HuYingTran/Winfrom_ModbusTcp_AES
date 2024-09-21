using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using ZedGraph;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Security.Cryptography;
using System.Windows.Forms.DataVisualization.Charting;
using static System.Net.Mime.MediaTypeNames;

namespace Modbus_client
{
    public partial class Form1 : Form
    {
        private TcpClient tcpClient; // Đối tượng TcpClient để kết nối TCP/IP
        private NetworkStream stream; // Đối tượng NetworkStream để thao tác với dữ liệu TCP/IP
        private TcpListener tcpListener;
        Thread thread_read;

        // Các biến toàn cục
        UInt16 TranID = 0; // Mã nhận dạng gói tin Modbus TCP
        int index = 0;
        byte[] Send_data_tcp = new byte[100]; // Bộ đệm gửi dữ liệu tới Server
        byte[] Received_data_tcp = new byte[100]; // Bộ đệm nhận dữ liệu từ Server   
        double x = 0;
        bool flag_on = false;
        UInt32 Minisech = 0;
        UInt32 Minisecl = 0;

        private byte[] buffer = new byte[150];
        private bool isReceiving = false;

        PointPairList Line1 = new PointPairList(); // Dữ liệu đường cong 1
        PointPairList Line2 = new PointPairList(); // Dữ liệu đường cong 2

        public Form1()
        {
            InitializeComponent();
        }

        //Biến mã hóa AES
        public byte[] key = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
        public byte[] plainText = new byte[32];
        public byte[] cipherText = new byte[32];
        public byte[] output;

        // Hàm mã hóa AES
        private static byte[] AES_encrypt_block(byte[] plainText, byte[] Key)
        {
            byte[] output_buffer = new byte[plainText.Length];
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.BlockSize = 128;
                aesAlg.KeySize = 128;
                aesAlg.Padding = PaddingMode.None;
                aesAlg.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                encryptor.TransformBlock(plainText, 0, plainText.Length, output_buffer, 0);
            }
            return output_buffer;
        }

        //Hàm giải mã AES
        private static byte[] AES_Decrypt_block(byte[] cipherText, byte[] Key)
        {
            // Declare the string used to hold the decrypted text. 
            byte[] output_buffer = new byte[cipherText.Length];
            using (AesManaged aesAlg = new AesManaged())
            {
                aesAlg.Mode = CipherMode.ECB;
                aesAlg.BlockSize = 128;
                aesAlg.KeySize = 128;
                aesAlg.Padding = PaddingMode.None;
                aesAlg.Key = Key;
                // Create a decrytor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                decryptor.TransformBlock(cipherText, 0, cipherText.Length, output_buffer, 0);
            }
            return output_buffer;
        }
        UInt16 CRC;
        UInt16 ModRTU_CRC(byte[] buf, int len)
        {
            UInt16 crc = 0xFFFF;

            for (int pos = 0; pos < len; pos++)
            {
                crc ^= (UInt16)buf[pos];          // XOR byte into least sig. byte of crc

                for (int i = 8; i != 0; i--)
                {    // Loop over each bit
                    if ((crc & 0x0001) != 0)
                    {      // If the LSB is set
                        crc >>= 1;                    // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                    }
                    else                            // Else LSB is not set
                        crc >>= 1;                    // Just shift right
                }
            }
            // Note, this number has low and high bytes swapped, so use it accordingly (or swap bytes)
            //return crc;
            return (UInt16)(((crc >> 8) & 0xff) | ((crc << 8) & 0xff00));
        }
        Int64 TS1;
        //---------------------------------------------------------------------------------

        // Hàm khởi tạo đồ thị
        private void init_graph(ZedGraphControl Zg)
        {
            GraphPane myPane = Zg.GraphPane;

            myPane.Title.Text = "Đặc tính quá độ động cơ";
            myPane.XAxis.Title.Text = "Thời gian";
            myPane.YAxis.Title.Text = "Tốc độ động cơ (vòng/phút)";

            LineItem curve1 = myPane.AddCurve("Giá trị đặt tốc độ động cơ", Line1, Color.Red, SymbolType.None);
            LineItem curve2 = myPane.AddCurve("Tốc độ động cơ đo thực tế", Line2, Color.Blue, SymbolType.None);

            curve1.Line.Width = 1.0F;
            curve2.Line.Width = 2.0F;

            myPane.XAxis.Scale.Min = 0;
            myPane.XAxis.Scale.Max = 500;
            myPane.YAxis.Scale.Min = -30;
            myPane.YAxis.Scale.Max = 280;

            zedGraphControl1.AxisChange();
        }

        // Hàm vẽ đồ thị
        void vedothi(ZedGraphControl Zed_graph_name, PointPairList Line1, PointPairList Line2, double x1, double y1, double x2, double y2)
        {
            try
            {
                Line1.Add(x1, y1);
                Line2.Add(x2, y2);
                Zed_graph_name.AxisChange();
                Zed_graph_name.Invalidate(); // Vẽ lại ZedGraphControl
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hàm gửi yêu cầu đọc thanh ghi (Function Code 03)
        void ModTCP_Req_03(UInt16 transactionID, byte unitID, UInt16 firstRegister, UInt16 numOfRegisters)
        {

            // Mã nhận dạng gói tin (Transaction ID) - 2 byte
            Send_data_tcp[0] = (byte)(transactionID / 256);  // Byte cao
            Send_data_tcp[1] = (byte)(transactionID % 256);  // Byte thấp

            // Mã định danh giao thức (Protocol ID) - 2 byte (luôn là 0 cho Modbus TCP)
            Send_data_tcp[2] = 0x00;
            Send_data_tcp[3] = 0x00;

            // Độ dài gói tin (Length) - 2 byte (bao gồm phần còn lại của gói tin)
            Send_data_tcp[4] = 0x00;
            Send_data_tcp[5] = 0x06;

            // Mã nhận dạng thiết bị (Unit ID) - 1 byte
            Send_data_tcp[6] = unitID;

            // Bổ sung 8 byte Timestamp định danh gói tin     
            Int64 TS = DateTime.Now.Ticks / 10000;//mini seconds
            //lưu dấu thời gian
            Int64 TS1 = TS;
            Send_data_tcp[7] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[8] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[9] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[10] = Convert.ToByte(TS % 256); TS = TS / 256;

            Send_data_tcp[11] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[12] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[13] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[14] = Convert.ToByte(TS % 256);

            // Mã chức năng (Function Code) - 1 byte (0x03 để đọc thanh ghi giữ)
            Send_data_tcp[15] = 0x03;

            // Địa chỉ thanh ghi bắt đầu (Starting Register Address) - 2 byte
            Send_data_tcp[16] = (byte)(firstRegister / 256);  // Byte cao
            Send_data_tcp[17] = (byte)(firstRegister % 256);  // Byte thấp

            // Số lượng thanh ghi cần đọc (Number of Registers) - 2 byte
            Send_data_tcp[18] = (byte)(numOfRegisters / 256);  // Byte cao
            Send_data_tcp[19] = (byte)(numOfRegisters % 256);  // Byte thấp

            // thêm 0 vào cuối đủ 32 byte tính từ Timestamp (index =7) đến cuối
            for (int i = 20; i < 37; i++)
                Send_data_tcp[i] = 0;

            // Bổ sung 2 byte CRC
            UInt16 CRC = ModRTU_CRC(Send_data_tcp, 37);
            Send_data_tcp[37] = (byte)(CRC / 256);
            Send_data_tcp[38] = (byte)(CRC % 256);

            //mã hóa từ FC (index = 7) đến cuối gói tin
            for (int i = 0; i < 32; i++)
                plainText[i] = Send_data_tcp[7 + i];
            // Mã hóa
            cipherText = AES_encrypt_block(plainText, key);
            for (int i = 0; i < 32; i++)
                Send_data_tcp[7 + i] = cipherText[i];

            index = 0;
        }

        // Hàm gửi yêu cầu ghi một thanh ghi (Function Code 06)
        void ModTCP_Req_06(UInt16 transactionID, byte unitID, UInt16 firstRegister, UInt16 ValueOfRegisters)
        {
            // Mã nhận dạng gói tin (Transaction ID) - 2 byte
            Send_data_tcp[0] = (byte)(transactionID / 256);  // Byte cao
            Send_data_tcp[1] = (byte)(transactionID % 256);  // Byte thấp

            // Mã định danh giao thức (Protocol ID) - 2 byte (luôn là 0 cho Modbus TCP)
            Send_data_tcp[2] = 0x00;
            Send_data_tcp[3] = 0x00;

            // Độ dài gói tin (Length) - 2 byte (bao gồm phần còn lại của gói tin)
            Send_data_tcp[4] = 0x00;
            Send_data_tcp[5] = 0x06;

            // Mã nhận dạng thiết bị (Unit ID) - 1 byte
            Send_data_tcp[6] = unitID;

            // Bổ sung 8 byte Timestamp định danh gói tin     
            Int64 TS = DateTime.Now.Ticks / 10000;//mini seconds
            //lưu dấu thời gian
            Int64 TS1 = TS;
            Send_data_tcp[7] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[8] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[9] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[10] = Convert.ToByte(TS % 256); TS = TS / 256;

            Send_data_tcp[11] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[12] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[13] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[14] = Convert.ToByte(TS % 256);

            // Mã chức năng (Function Code) - 1 byte (0x03 để đọc thanh ghi giữ)
            Send_data_tcp[15] = 06;

            // Địa chỉ thanh ghi bắt đầu (Starting Register Address) - 2 byte
            Send_data_tcp[16] = (byte)(firstRegister / 256);  // Byte cao
            Send_data_tcp[17] = (byte)(firstRegister % 256);  // Byte thấp

            // Số lượng thanh ghi cần đọc (Number of Registers) - 2 byte
            Send_data_tcp[18] = (byte)(ValueOfRegisters / 256);  // Byte cao
            Send_data_tcp[19] = (byte)(ValueOfRegisters % 256);  // Byte thấp

            // thêm 0 vào cuối đủ 32 byte tính từ Function Code (index =7) đến cuối
            for (int i = 20; i < 39; i++)
                Send_data_tcp[i] = 0;

            // Bổ sung 2 byte CRC
            UInt16 CRC = ModRTU_CRC(Send_data_tcp, 37);
            Send_data_tcp[37] = (byte)(CRC / 256);
            Send_data_tcp[38] = (byte)(CRC % 256);

            //mã hóa từ FC (index = 7) đến cuối gói tin
            for (int i = 0; i < 32; i++)
                plainText[i] = Send_data_tcp[7 + i];
            // Mã hóa
            cipherText = AES_encrypt_block(plainText, key);
            for (int i = 0; i < 32; i++)
                Send_data_tcp[7 + i] = cipherText[i];

            stream.Write(Send_data_tcp, 0, 39);
        }

        // Hàm gửi yêu cầu ghi nhiều thanh ghi (Function Code 16)
        void ModTCP_Req_16(UInt16 transactionID, byte unitID, UInt16 firstRegister, UInt16 numOfRegisters, byte[] registerValues)
        {
            int byteCount = numOfRegisters * 2; // Số byte cần ghi (mỗi thanh ghi là 2 byte)

            Send_data_tcp[0] = (byte)(transactionID / 256);
            Send_data_tcp[1] = (byte)(transactionID % 256);

            Send_data_tcp[2] = 0x00;
            Send_data_tcp[3] = 0x00;

            Send_data_tcp[4] = (byte)((7 + byteCount) / 256);  // Tính toán độ dài gói tin
            Send_data_tcp[5] = (byte)((7 + byteCount) % 256);

            Send_data_tcp[6] = unitID;

            // Bổ sung 8 byte Timestamp định danh gói tin     
            Int64 TS = DateTime.Now.Ticks / 10000;//mini seconds
            //lưu dấu thời gian
            Int64 TS1 = TS;
            Send_data_tcp[7] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[8] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[9] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[10] = Convert.ToByte(TS % 256); TS = TS / 256;

            Send_data_tcp[11] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[12] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[13] = Convert.ToByte(TS % 256); TS = TS / 256;
            Send_data_tcp[14] = Convert.ToByte(TS % 256);

            Send_data_tcp[15] = 0x10;

            Send_data_tcp[16] = (byte)(firstRegister / 256);
            Send_data_tcp[17] = (byte)(firstRegister % 256);

            Send_data_tcp[18] = (byte)(numOfRegisters / 256);
            Send_data_tcp[19] = (byte)(numOfRegisters % 256);

            //so byte muốn ghi
            Send_data_tcp[20] = (byte)byteCount;

            // Ghi dữ liệu vào gói tin
            for (int i = 0; i < byteCount; i++)
            {
                Send_data_tcp[21 + i] = registerValues[i];
            }

            // thêm 0 vào cuối đủ 32 byte tính từ Function Code (index =7) đến cuối
            for (int i = 21 + byteCount; i < 37; i++)
                Send_data_tcp[i] = 0;
            // Bổ sung 2 byte CRC
            UInt16 CRC = ModRTU_CRC(Send_data_tcp, 37);
            Send_data_tcp[37] = (byte)(CRC / 256);
            Send_data_tcp[38] = (byte)(CRC % 256);
            //mã hóa từ FC (index = 7) đến cuối gói tin
            for (int i = 0; i < 32; i++)
                plainText[i] = Send_data_tcp[7 + i];
            // Mã hóa
            cipherText = AES_encrypt_block(plainText, key);
            for (int i = 0; i < 32; i++)
                Send_data_tcp[7 + i] = cipherText[i];

            stream.Write(Send_data_tcp, 0, 39);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            init_graph(zedGraphControl1);
            // Khóa các nút thao tác.
            btn_bat_dc.Enabled = false;
            btn_tat_dc.Enabled = false;
            btn_set_pid.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (stream != null)
                {
                    stream.Close();
                }
                if (tcpClient != null)
                {
                    tcpClient.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error closing Modbus TCP connection: " + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //-----------------------------------------------------------------------------------------


        // Hàm xử lý Timer (Cập nhật dữ liệu và vẽ đồ thị)
        private void btn_bat_dc_Click(object sender, EventArgs e)
        {
            index = 0;
            ModTCP_Req_06(0x0000, 0x01, 0x0000, 0x0001);

            Line1.Clear();
            Line2.Clear();
            zedGraphControl1.GraphPane.XAxis.Scale.Min = 0;
            zedGraphControl1.GraphPane.XAxis.Scale.Max = 500;
            zedGraphControl1.GraphPane.YAxis.Scale.Min = -30;
            zedGraphControl1.GraphPane.YAxis.Scale.Max = 280;
            this.Refresh();
            x = 0;

            // Tạo luồng thread để đọc dữ liệu
            Thread thread_reciver = new Thread(start_reciver);
            thread_reciver.Start();
            flag_on = true;
        }

        private void btn_tat_dc_Click(object sender, EventArgs e)
        {

            index = 0;
            ModTCP_Req_06(0x0000, 0x01, 0x0000, 0x0000);
            
            Line1.Clear();
            Line2.Clear();
            x = 0;
            flag_on = false;
        }

        private void btn_speed_Click(object sender, EventArgs e)
        {
            // nội dung thanh ghi
            UInt16 setpoint = (UInt16)(Convert.ToDouble(textBox_speed.Text));

            ModTCP_Req_06(0x0000, 0x01, 0x0001, setpoint);
            Thread.Sleep(200);

            if (setpoint > zedGraphControl1.GraphPane.YAxis.Scale.Max)
                zedGraphControl1.GraphPane.YAxis.Scale.Max = setpoint + 20;
        }

        private void btn_set_pid_Click(object sender, EventArgs e)
        {
            // nội dung thanh ghi
            UInt16 kp = (UInt16)(Convert.ToDouble(textBox_kp.Text));
            UInt16 kd = (UInt16)(Convert.ToDouble(textBox_ki.Text));
            UInt16 ki = (UInt16)(Convert.ToDouble(textBox_kd.Text));
            byte[] registerValues = new byte[6];
            registerValues[0] = (byte)(kp / 256);    // Byte cao của kp
            registerValues[1] = (byte)(kp % 256);    // Byte thấp của kp
            registerValues[2] = (byte)(kd / 256);    // Byte cao của kd
            registerValues[3] = (byte)(kd % 256);    // Byte thấp của kd
            registerValues[4] = (byte)(ki / 256);    // Byte cao của ki
            registerValues[5] = (byte)(ki % 256);    // Byte thấp của ki

            // Gọi hàm ModTCP_Req_16 với mảng registerValues
            index = 0;
            ModTCP_Req_16(0x0000, 0x01, 0x0003, 3, registerValues);
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            Line1.Clear(); // xóa đồ thị khi vẽ mới
            Line2.Clear();
            zedGraphControl1.GraphPane.XAxis.Scale.Min = 0;     // Min trục x
            zedGraphControl1.GraphPane.XAxis.Scale.Max = 500;   // Max trục x           
            zedGraphControl1.GraphPane.YAxis.Scale.Min = -30;   // Min trục y
            zedGraphControl1.GraphPane.YAxis.Scale.Max = 280;   // Max trục y             
            this.Refresh();
            x = 0;
        }

        private void btn_ket_noi_Click(object sender, EventArgs e)
        {
            if(btn_ket_noi.Text == "Đóng")
            {
                btn_ket_noi.Text = "Kết nối";
                tcpClient.Close();
            }
            else
            {
                try
                {
                    String ip = textBox_ip.Text;                    // Lấy thông tin ip từ texBox
                    int port = Convert.ToInt32(textBox_port.Text);  // Lấy thông tin port từ texBox
                    tcpClient = new TcpClient(ip, port);            // Kết nối đến server TCP/IP
                    stream = tcpClient.GetStream();                 // Lấy NetworkStream để đọc/ghi dữ liệu
                    tcpClient.ReceiveTimeout = 5000;                // Thời gian chờ nhận dữ liệu (5 giây)
                    tcpClient.SendTimeout = 5000;                   // Thời gian chờ gửi dữ liệu (5 giây)
                    
                    MessageBox.Show("Modbus TCP Connection established.", "Connection Status", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Bật trạng thái cho các nút điều khiển
                    btn_bat_dc.Enabled = true;
                    btn_tat_dc.Enabled = true;
                    btn_set_pid.Enabled = true;
                    btn_set_speed.Enabled = true;
                }
                catch
                {
                    MessageBox.Show("Không thể kết nối TCP/IP", "Thông báo");
                    btn_bat_dc.Enabled = false;
                    btn_tat_dc.Enabled = false;
                    btn_set_speed.Enabled = false;
                }
                btn_ket_noi.Text = "Đóng";
            }
        }
        private void start_reciver()
        {
            while (flag_on)
            {
                try
                {
                    if (tcpClient.Connected && stream.CanRead)
                    {
                        int n = stream.Read(Received_data_tcp, 0, 100);

                        for (int i = 0; i < 32; i++)
                        {
                            cipherText[i] = Received_data_tcp[7 + i];
                        }
                        output = AES_Decrypt_block(cipherText, key);

                        for (int i = 0; i < 32; i++)
                            Received_data_tcp[7 + i] = output[i];
                        byte ok = 0;
                        //Tính lại CRC gói tin nhận được
                        CRC = ModRTU_CRC(Received_data_tcp, 37);
                        //kiểm tra CRC
                        if (CRC == (Received_data_tcp[37] * 256 + Received_data_tcp[38]))
                        {
                            ok = 0;
                            UInt32 minisech, minisecl;

                            minisecl = (UInt32)(((Received_data_tcp[10] * 256) + Received_data_tcp[9]) * 256 + Received_data_tcp[8]) * 256 + Received_data_tcp[7];
                            minisech = (UInt32)(((Received_data_tcp[14] * 256) + Received_data_tcp[13]) * 256 + Received_data_tcp[12]) * 256 + Received_data_tcp[11];
                            if (minisech > Minisech)
                            {
                                ok = 1;
                                Minisech = minisech;
                                Minisecl = minisecl;
                            }
                            else if (minisech == Minisech)
                            {
                                if (minisecl > Minisecl)
                                {
                                    ok = 1;
                                    Minisech = minisech;
                                    Minisecl = minisecl;
                                }
                            }
                        }

                        if (ok == 1 && Received_data_tcp[15] == 0x06)
                        {
                            double Tocdodat = (Received_data_tcp[18] * 256 + Received_data_tcp[19]);
                            double Tocdothuc = Tocdodat + 50;
                            //btn_set_pid.Text = Tocdodat.ToString();
                            // In ra giá trị của Tocdodat và Tocdothuc để kiểm tra
                            Console.WriteLine($"Tocdodat: {Tocdodat}, Tocdothuc: {Tocdothuc} x={x}");

                            // Điều chỉnh trục X, Y nếu cần thiết
                            if (x > (zedGraphControl1.GraphPane.XAxis.Scale.Max - 20))
                                zedGraphControl1.GraphPane.XAxis.Scale.Max = x + 20;

                            if (Tocdothuc > zedGraphControl1.GraphPane.YAxis.Scale.Max)
                                zedGraphControl1.GraphPane.YAxis.Scale.Max = Tocdothuc + 20;
                            else if (Tocdothuc < zedGraphControl1.GraphPane.YAxis.Scale.Min)
                                zedGraphControl1.GraphPane.YAxis.Scale.Min = Tocdothuc - 20;

                            // Vẽ đồ thị
                            zedGraphControl1.Invoke(new Action(() => vedothi(zedGraphControl1, Line1, Line2, x, Tocdodat, x, Tocdothuc)));

                            // Tăng giá trị x cho lần tiếp theo
                            x += 10;
                        }

                        // Gửi yêu cầu mới để cập nhật dữ liệu
                        TranID += 1;
                        ModTCP_Req_03(TranID, 0x01, 0x0001, 0x0002);
                    }
                }
                catch (IOException ex)
                {
                    Console.WriteLine("Lisent...");
                }
            }
        }
    }
}

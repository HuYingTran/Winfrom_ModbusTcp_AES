namespace Modbus_client
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.btn_set_speed = new System.Windows.Forms.Button();
            this.textBox_speed = new System.Windows.Forms.TextBox();
            this.btn_tat_dc = new System.Windows.Forms.Button();
            this.btn_bat_dc = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.textBox_kp = new System.Windows.Forms.TextBox();
            this.textBox_ki = new System.Windows.Forms.TextBox();
            this.textBox_kd = new System.Windows.Forms.TextBox();
            this.btn_set_pid = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_ket_noi = new System.Windows.Forms.Button();
            this.textBox_ip = new System.Windows.Forms.TextBox();
            this.textBox_port = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(321, 5);
            this.zedGraphControl1.Margin = new System.Windows.Forms.Padding(6);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(850, 450);
            this.zedGraphControl1.TabIndex = 8;
            this.zedGraphControl1.UseExtendedPrintDialog = true;
            // 
            // btn_set_speed
            // 
            this.btn_set_speed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_set_speed.Location = new System.Drawing.Point(63, 339);
            this.btn_set_speed.Margin = new System.Windows.Forms.Padding(4);
            this.btn_set_speed.Name = "btn_set_speed";
            this.btn_set_speed.Size = new System.Drawing.Size(120, 40);
            this.btn_set_speed.TabIndex = 13;
            this.btn_set_speed.Text = "SetPoint";
            this.btn_set_speed.UseVisualStyleBackColor = true;
            this.btn_set_speed.Click += new System.EventHandler(this.btn_speed_Click);
            // 
            // textBox_speed
            // 
            this.textBox_speed.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_speed.Location = new System.Drawing.Point(63, 281);
            this.textBox_speed.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_speed.Multiline = true;
            this.textBox_speed.Name = "textBox_speed";
            this.textBox_speed.Size = new System.Drawing.Size(120, 40);
            this.textBox_speed.TabIndex = 12;
            this.textBox_speed.Text = "200";
            this.textBox_speed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btn_tat_dc
            // 
            this.btn_tat_dc.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_tat_dc.Location = new System.Drawing.Point(711, 499);
            this.btn_tat_dc.Margin = new System.Windows.Forms.Padding(4);
            this.btn_tat_dc.Name = "btn_tat_dc";
            this.btn_tat_dc.Size = new System.Drawing.Size(180, 50);
            this.btn_tat_dc.TabIndex = 11;
            this.btn_tat_dc.Text = "Tắt động cơ";
            this.btn_tat_dc.UseVisualStyleBackColor = true;
            this.btn_tat_dc.Click += new System.EventHandler(this.btn_tat_dc_Click);
            // 
            // btn_bat_dc
            // 
            this.btn_bat_dc.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_bat_dc.Location = new System.Drawing.Point(469, 501);
            this.btn_bat_dc.Margin = new System.Windows.Forms.Padding(4);
            this.btn_bat_dc.Name = "btn_bat_dc";
            this.btn_bat_dc.Size = new System.Drawing.Size(180, 50);
            this.btn_bat_dc.TabIndex = 10;
            this.btn_bat_dc.Text = "Bật động cơ";
            this.btn_bat_dc.UseVisualStyleBackColor = true;
            this.btn_bat_dc.Click += new System.EventHandler(this.btn_bat_dc_Click);
            // 
            // btn_clear
            // 
            this.btn_clear.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_clear.Location = new System.Drawing.Point(946, 499);
            this.btn_clear.Margin = new System.Windows.Forms.Padding(4);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(180, 50);
            this.btn_clear.TabIndex = 9;
            this.btn_clear.Text = "Clear";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // textBox_kp
            // 
            this.textBox_kp.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_kp.Location = new System.Drawing.Point(65, 16);
            this.textBox_kp.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_kp.Multiline = true;
            this.textBox_kp.Name = "textBox_kp";
            this.textBox_kp.Size = new System.Drawing.Size(120, 40);
            this.textBox_kp.TabIndex = 12;
            this.textBox_kp.Text = "5";
            this.textBox_kp.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_ki
            // 
            this.textBox_ki.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ki.Location = new System.Drawing.Point(65, 71);
            this.textBox_ki.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_ki.Multiline = true;
            this.textBox_ki.Name = "textBox_ki";
            this.textBox_ki.Size = new System.Drawing.Size(120, 40);
            this.textBox_ki.TabIndex = 12;
            this.textBox_ki.Text = "2";
            this.textBox_ki.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_kd
            // 
            this.textBox_kd.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_kd.Location = new System.Drawing.Point(65, 126);
            this.textBox_kd.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_kd.Multiline = true;
            this.textBox_kd.Name = "textBox_kd";
            this.textBox_kd.Size = new System.Drawing.Size(120, 40);
            this.textBox_kd.TabIndex = 12;
            this.textBox_kd.Text = "5";
            this.textBox_kd.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btn_set_pid
            // 
            this.btn_set_pid.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_set_pid.Location = new System.Drawing.Point(65, 181);
            this.btn_set_pid.Margin = new System.Windows.Forms.Padding(4);
            this.btn_set_pid.Name = "btn_set_pid";
            this.btn_set_pid.Size = new System.Drawing.Size(120, 40);
            this.btn_set_pid.TabIndex = 9;
            this.btn_set_pid.Text = "Thiết lập";
            this.btn_set_pid.UseVisualStyleBackColor = true;
            this.btn_set_pid.Click += new System.EventHandler(this.btn_set_pid_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.No;
            this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(14, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 25);
            this.label1.TabIndex = 20;
            this.label1.Text = "Ki";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Cursor = System.Windows.Forms.Cursors.No;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 25);
            this.label2.TabIndex = 20;
            this.label2.Text = "Kp";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Cursor = System.Windows.Forms.Cursors.No;
            this.label3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(14, 132);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 25);
            this.label3.TabIndex = 20;
            this.label3.Text = "Kd";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Cursor = System.Windows.Forms.Cursors.No;
            this.label4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(190, 287);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 25);
            this.label4.TabIndex = 20;
            this.label4.Text = "Speed(v/p)";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox_kd);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.btn_set_pid);
            this.panel1.Controls.Add(this.btn_set_speed);
            this.panel1.Controls.Add(this.textBox_speed);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.textBox_kp);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.textBox_ki);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 211);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(309, 413);
            this.panel1.TabIndex = 21;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.btn_ket_noi);
            this.panel2.Controls.Add(this.textBox_ip);
            this.panel2.Controls.Add(this.textBox_port);
            this.panel2.Location = new System.Drawing.Point(12, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(300, 200);
            this.panel2.TabIndex = 22;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Cursor = System.Windows.Forms.Cursors.No;
            this.label6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(7, 86);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(51, 25);
            this.label6.TabIndex = 22;
            this.label6.Text = "Port";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Cursor = System.Windows.Forms.Cursors.No;
            this.label5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(7, 38);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 25);
            this.label5.TabIndex = 21;
            this.label5.Text = "IP";
            // 
            // btn_ket_noi
            // 
            this.btn_ket_noi.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_ket_noi.Location = new System.Drawing.Point(63, 128);
            this.btn_ket_noi.Margin = new System.Windows.Forms.Padding(4);
            this.btn_ket_noi.Name = "btn_ket_noi";
            this.btn_ket_noi.Size = new System.Drawing.Size(120, 40);
            this.btn_ket_noi.TabIndex = 21;
            this.btn_ket_noi.Text = "Kết nối";
            this.btn_ket_noi.UseVisualStyleBackColor = true;
            this.btn_ket_noi.Click += new System.EventHandler(this.btn_ket_noi_Click);
            // 
            // textBox_ip
            // 
            this.textBox_ip.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_ip.Location = new System.Drawing.Point(65, 32);
            this.textBox_ip.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_ip.Multiline = true;
            this.textBox_ip.Name = "textBox_ip";
            this.textBox_ip.Size = new System.Drawing.Size(200, 40);
            this.textBox_ip.TabIndex = 14;
            this.textBox_ip.Text = "192.168.88.165";
            this.textBox_ip.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox_port
            // 
            this.textBox_port.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox_port.Location = new System.Drawing.Point(65, 80);
            this.textBox_port.Margin = new System.Windows.Forms.Padding(4);
            this.textBox_port.Multiline = true;
            this.textBox_port.Name = "textBox_port";
            this.textBox_port.Size = new System.Drawing.Size(120, 40);
            this.textBox_port.TabIndex = 13;
            this.textBox_port.Text = "502";
            this.textBox_port.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 603);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btn_tat_dc);
            this.Controls.Add(this.btn_bat_dc);
            this.Controls.Add(this.btn_clear);
            this.Controls.Add(this.zedGraphControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mobus TCP Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.Button btn_set_speed;
        private System.Windows.Forms.TextBox textBox_speed;
        private System.Windows.Forms.Button btn_tat_dc;
        private System.Windows.Forms.Button btn_bat_dc;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.TextBox textBox_kp;
        private System.Windows.Forms.TextBox textBox_ki;
        private System.Windows.Forms.TextBox textBox_kd;
        private System.Windows.Forms.Button btn_set_pid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox_ip;
        private System.Windows.Forms.TextBox textBox_port;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_ket_noi;
    }
}


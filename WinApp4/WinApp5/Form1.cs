using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp5
{
    public partial class frmClient : Form
    {
        public frmClient()
        {
            InitializeComponent();
        }

        // 소켓 선언
        Socket _Sock;

        //-----------------------------------------------------------
        // *Menu
        
        //
        private void mnuStart_Click(object sender, EventArgs e)
        {
            // timer 실행
            timer1.Enabled = true;
            // tb에 숫자 값이  timer interval 값 
            timer1.Interval = int.Parse(tbInterval.Text);


        }

        private void mnuStop_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void mnuClose_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            Close();
        }
        //----------------------------------------------------------

        int pCount;

        // timer
        // timer를 통해 데이터값을 계속 갱신한다
        private void timer1_Tick(object sender, EventArgs e)
        {
            //
            string s1 = tbCode.Text;    // ex) 10001 : 5자리
            string s2 = tbModel.Text;   // ex) 00000 : 5자리
            string s3 = tbTemp.Text;    //  ex) 10.5 : 5자리
            string s4 = tbHum.Text;     // ex) 30.00 : 5자리
            string s5 = tbWind.Text;    // ex) 05.70 : 5자리
            string s6 = dateTimePicker1.Text;   // ex) 2020년 10월 30일

            // 프로토콜 정의 : ex) [STX:02][s1:05][s2:05][s3:05][s4:05][s5:05]...[데이터값]...[ETX:03]
            // STX : 시작, EXT :끝, 데이터값 : 자리수를 넣어준다 , []안넣어줘도 된다(짧을수록 좋다?)
            string sPro = $"[STX]{s1}{s2}{s3}{s4}{s5}[ETX]";
            
            // 소켓 연결
            _Sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _Sock.Connect(tbServerIP.Text, int.Parse(tbSeverPort.Text));

            // 2byte -> 1btye 로변환시켜줘야 하는데 Encording으로 바꿔줘서 Send 메소드로 전송한다.
            // getbyte : string을 byte 형태로 변환
            _Sock.Send(Encoding.Default.GetBytes(sPro));   //string을 byte로 바꿔줘야한다 encoding 활용

            tbProtocol.Text = $"[{pCount}] {sPro}";
            

            
        }
    }
}

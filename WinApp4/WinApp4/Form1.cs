using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinApp4
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        
        // 1. 소켓 변수 필요 (10.30)
        TcpListener _Listen;
        TcpClient _Sock;

        // 2. 데이터를 불러올 수있는공간을 20000으로 설정
        byte[] buf =new byte[20000];

        // 3. delegate : c의 함수 포인터와 비슷
        delegate void AddTextCallBack(string str);

        // tbMemo에 표현하고자 하는데, 직접 엑세스 할 수 없어서 (Write)읽을수 있도록  콜백함수로 연결 시켜준다
        private void AddText(string str)
        {
            // InvokeRequired 
            if (this.tbMemo.InvokeRequired)
            {
                AddTextCallBack d = new AddTextCallBack(AddText);
                this.Invoke(d, new object[] { str });
            }
            else
            {
                tbMemo.Text += str;
            }

        }


        // Button Start버튼 (10.30)
        private void btnStart_Click(object sender, EventArgs e)
        {
            // 1) Listener 열고 동작 수행 - Session Open
            
            // (1) 버튼을 여러번 누르는 것을 방지하는 if문
            if (_Listen == null)
                _Listen = new TcpListener(int.Parse(tbPort.Text));
            
            // (2)
            _Listen.Start();

            //... Thread가 없으면 동작이 없음
            // thread 별도로 돌아가는 프로그램을 만든다는 것과 같다


            // (6) Thread 선언 - 실행
            Thread ServerThread = new Thread(ServerProcess);
            ServerThread.Start();



            ////
            //// (3) 외부로부터의 접속을 받을 수 있다
            //_Sock = _Listen.AcceptTcpClient();  //Session open

            //// (4) 읽어오는 과정
            //NetworkStream ns = _Sock.GetStream();   // 소켓을 열고 
            
            //if (ns.DataAvailable)
            //{
            //    // buf : byte array = buf 라는 바이트 배열에 담겨져 있음 - (string으로 바꿔줘야 화면 출력 가능)
            //    ns.Read(buf, 0, (int)(ns.Length));
            //    tbMemo.Text += buf.ToString() +"\r\n";
            //}

            // (5)
            //_Listen.Stop();

            


        }
        
        
        // (6) 서버 Thread 함수 (10.30)
        private void ServerProcess() 
        {

            while (true) 
            {

                // 외부로부터의 접속을 받을 수 있다 - AcceptTcpClient 
                _Sock = _Listen.AcceptTcpClient();
                
                // (a-1)Cross Thread 오류 발생 : tbMomo 에 접근 불가 ==> invoke 필요 
                // tbMemo.Text += _Sock.Client.RemoteEndPoint.ToString();
                // (a-2) AddText 함수 사용 + GetTokon함수 사용
                string s1 = GetToken(0,_Sock.Client.RemoteEndPoint.ToString(),":");
                AddText($"원격 접속 요청 : {s1}\r\n");
                
                
                
                // 읽어오는 과정 - NetworkStream 
                NetworkStream ns = _Sock.GetStream();    

                if (ns.DataAvailable)
                {
                    // buf : byte array = buf 라는 바이트 배열에 담겨져 있음 - (string으로 바꿔줘야 화면 출력 가능)
                    ns.Read(buf, 0, (int)(buf.Length));
                    // tbMemo.Text += buf.ToString() + "\r\n";
                    AddText(Encoding.Default.GetString(buf) + "\r\n");

                    // string -> byte -> string
                }

            }
            

        }
        

        //
        public string GetToken(int n, string str, string sep = ",") // ","
        {
            int i, j, k;  // local index
            int n1 = 0, n2 = 0, n3 = 0;  // temp int 변수
            string sRet;

            for (i = 0, n1 = 0; i < n; i++)  // 0  1  2  3  4  5 ...
            {   // IndexOf : 문자가 없을 경우 -1
                n1 = str.IndexOf(sep, n1) + 1;  // i 번째 구분자   
                if (n1 == 0) return "";
            }   // n1 : n 번째 필드 시작

            n2 = str.IndexOf(sep, n1);  // n+1 번째 구분자
            if (n2 == -1) n2 = str.Length;
            n3 = n2 - n1;  // 문자열 길이 계산

            sRet = str.Substring(n1, n3);
            return sRet;
        }




        //






    }
}

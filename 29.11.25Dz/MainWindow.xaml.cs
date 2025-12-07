using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace _29._11._25Dz
{
   
       public partial class MainWindow : Window
       {
          private int sharedData = 0;
          private readonly object lockObject = new object();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnRace_Click(object sender, RoutedEventArgs e)
        {
            txtStatus.Text = "Запущена гонка данных (без синхронизации)";
            sharedData = 0;

            
            for (int i = 0; i < 5; i++)
            {
                int threadNum = i;
                Task.Run(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                    
                        sharedData++;
                        Dispatcher.Invoke(() =>
                        {
                            txtStatus.Text = $"Поток {threadNum} увеличил значение: {sharedData}";
                        });
                        Thread.Sleep(1);
                    }
                });
            }
        }

        private void BtnLock_Click(object sender, RoutedEventArgs e)
        {
            txtStatus.Text = "Запущено безопасное добавление с lock";
            sharedData = 0;

            for (int i = 0; i < 5; i++)
            {
                int threadNum = i;
                Task.Run(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        lock (lockObject)
                        {
                            sharedData++;
                        }
                        Dispatcher.Invoke(() =>
                        {
                            txtStatus.Text = $"Поток {threadNum} безопасно увеличил: {sharedData}";
                        });
                        Thread.Sleep(1);
                    }
                });
            }
        }

        private void BtnMonitor_Click(object sender, RoutedEventArgs e)
        {
            txtStatus.Text = "Проверка с Monitor.TryEnter";
            sharedData = 0;
            object monitorObject = new object();

            for (int i = 0; i < 5; i++)
            {
                int threadNum = i;
                Task.Run(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool lockTaken = false;
                        try
                        {
                      
                            lockTaken = Monitor.TryEnter(monitorObject, TimeSpan.FromMilliseconds(100));
                            if (lockTaken)
                            {
                       
                                sharedData++;
                                Dispatcher.Invoke(() =>
                                {
                                    txtStatus.Text = $"Поток {threadNum} увеличил: {sharedData}";
                                });
                            }
                            else
                            {
                              
                                Dispatcher.Invoke(() =>
                                {
                                    txtStatus.Text = $"Поток {threadNum} не смог захватить монитор";
                                });
                            }
                        }
                        finally
                        {
                            if (lockTaken)
                                Monitor.Exit(monitorObject);
                        }
                        Thread.Sleep(1);
                    }
                });
            }
        }
        private void btnTwoTask_Click(object sender, RoutedEventArgs e)
        {

            TwoTask Case = new TwoTask();
            Case.Show();



        }



    }


}

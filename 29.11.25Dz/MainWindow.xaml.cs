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
          private int sharedCounter = 0;
          private readonly object lock_Obj = new object();
          private readonly object monitor_Obj = new object();



        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnRaceCondition_Click(object sender, RoutedEventArgs e)
        {
            txtOutput.Clear();
            sharedCounter = 0;
            AppendOutput("Запуск гонки данных без синхронизации...\n");

          
            Task task1 = Task.Run(() => IncrementWithoutSync("Поток 1"));
            Task task2 = Task.Run(() => IncrementWithoutSync("Поток 2"));

            Task.WaitAll(task1, task2);

            AppendOutput($"Результат без синхронизации: {sharedCounter}\n");
        }

        private void IncrementWithoutSync(string threadName)
        {
            for (int i = 0; i < 1000; i++)
            {
               
                sharedCounter++;
                Thread.Sleep(1);
            }
        }

        private void BtnLock_Click(object sender, RoutedEventArgs e)
        {
            txtOutput.Clear();
            sharedCounter = 0;
            AppendOutput("Запуск безопасного добавления с lock...\n");

            Task task1 = Task.Run(() => SafeIncrement("Поток 1"));
            Task task2 = Task.Run(() => SafeIncrement("Поток 2"));

            Task.WaitAll(task1, task2);

            AppendOutput($"Результат с lock: {sharedCounter}\n");
        }

        private void SafeIncrement(string threadName)
        {
            for (int i = 0; i < 1000; i++)
            {
                lock (lock_Obj)
                {
                    sharedCounter++;
                }
                Thread.Sleep(1);
            }
        }

        private void BtnMonitorTimeout_Click(object sender, RoutedEventArgs e)
        {
            txtOutput.Clear();
            AppendOutput("Проверка монитора с таймаутом...\n");

            if (Monitor.TryEnter(monitor_Obj, TimeSpan.FromSeconds(1)))
            {
                try
                {
                    AppendOutput("Успешно захватили монитор.\n");
                    
                    Thread.Sleep(2000);
                }
                finally
                {
                    Monitor.Exit(monitor_Obj);
                    AppendOutput("Освободили монитор.\n");
                }
            }
            else
            {
                AppendOutput("Не удалось захватить монитор за отведенное время.\n");
            }
        }

        private void AppendOutput(string text)
        {
            Dispatcher.Invoke(() =>
            {
                txtOutput.AppendText(text);
                txtOutput.ScrollToEnd();
            });
        }

        private void btnTwoTask_Click(object sender, RoutedEventArgs e)
        {

            TwoTask Case = new TwoTask();
            Case.Show();

        }




    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace _29._11._25Dz
{
    /// <summary>
    /// Логика взаимодействия для TwoTask.xaml
    /// </summary>
    public partial class TwoTask : Window
    {
        private readonly List<int> sharedList = new List<int>();
        private readonly object lock_Obj = new object();

        private CancellationTokenSource cts;
        private Task producerTask;
        private Task consumerTask;

        private int itemCounter = 0;

        public TwoTask()
        {
            InitializeComponent();
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            btnStart.IsEnabled = false;
            btnStop.IsEnabled = true;

            cts = new CancellationTokenSource();

            producerTask = Task.Run(() => Producer(cts.Token));
            consumerTask = Task.Run(() => Consumer(cts.Token));
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            btnStart.IsEnabled = true;
            btnStop.IsEnabled = false;
        }

        private void Producer(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    lock (lock_Obj)
                    {
                        itemCounter++;
                        sharedList.Add(itemCounter);
                        AppendLog($"Производитель добавил: {itemCounter}");
                        Monitor.Pulse(lock_Obj); 
                    }
                    Thread.Sleep(500);
                }
            }
            catch (OperationCanceledException) { }
        }

        private void Consumer(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    int item = 0;
                    lock (lock_Obj)
                    {
                        while (sharedList.Count == 0)
                        {
                            Monitor.Wait(lock_Obj); 
                        }
                        item = sharedList[0];
                        sharedList.RemoveAt(0);
                    }
                    
                    AppendLog($"Потребитель обработал: {item}");
                    Thread.Sleep(1000);
                }
            }
            catch (OperationCanceledException) { }
        }

        private void AppendLog(string message)
        {
            Dispatcher.Invoke(() =>
            {
                txtLog.AppendText($"{DateTime.Now:HH:mm:ss}: {message}\n");
                txtLog.ScrollToEnd();
            });
        }
    }


}

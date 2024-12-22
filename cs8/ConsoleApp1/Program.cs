using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        //1
        class BankAccount
        {
            private decimal balance;
            private readonly object lockObj = new object();

            public BankAccount(decimal initialBalance)
            {
                balance = initialBalance;
            }

            public void Withdraw(decimal amount)
            {
                Monitor.Enter(lockObj);
                try
                {
                    if (balance >= amount)
                    {
                        Console.WriteLine($"Зняття {amount} виконано.");
                        balance -= amount;
                    }
                    else
                    {
                        Console.WriteLine("Недостатньо коштів.");
                    }
                }
                finally
                {
                    Monitor.Exit(lockObj);
                }
            }

            public decimal GetBalance()
            {
                return balance;
            }
        }

        static void Task1()
        {
            BankAccount account = new BankAccount(100);

            Thread[] threads = new Thread[5];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread(() => account.Withdraw(30));
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine($"Залишок на рахунку: {account.GetBalance()}");
        }

        // 2
        static void Task2()
        {
            int[] array = new int[10000];
            Random random = new Random();
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = random.Next(1, 100);
            }

            int totalSum = 0;
            int numThreads = 4;
            int segmentSize = array.Length / numThreads;
            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                int start = i * segmentSize;
                int end = (i == numThreads - 1) ? array.Length : start + segmentSize;

                threads[i] = new Thread(() =>
                {
                    int localSum = 0;
                    for (int j = start; j < end; j++)
                    {
                        localSum += array[j];
                    }
                    Interlocked.Add(ref totalSum, localSum);
                });
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine($"Загальна сума елементів масиву: {totalSum}");
        }

        //3
        static void Task3()
        {
            int visitorCount = 0;
            int numThreads = 10;
            Thread[] threads = new Thread[numThreads];

            for (int i = 0; i < numThreads; i++)
            {
                threads[i] = new Thread(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        Interlocked.Increment(ref visitorCount);
                    }
                });
                threads[i].Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            Console.WriteLine($"Кінцеве значення лічильника відвідувачів: {visitorCount}");
        }

        static void Main()
        {
            Console.WriteLine("Завдання 1: Моделювання банківського рахунку");
            Task1();

            Console.WriteLine("\nЗавдання 2: Сума елементів масиву");
            Task2();

            Console.WriteLine("\nЗавдання 3: Лічильник відвідувачів сайту");
            Task3();
        }
    }
}

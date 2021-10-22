using ServiceStack.OrmLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace OrmLiteDbConnectionTest
{
    class Program
    {
        static OrmLiteConnectionFactory factory;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            factory = new OrmLiteConnectionFactory("Uid=root;Password=root;Server=localhost;Port=3307;Database=;SslMode=None;convert zero datetime=True", MySqlDialect.Provider);
            int nbr = 10;
            // await DoTestWithoutUsing(nbr);
            // await DoTestWithUsing(nbr);
            // await DoTestWithUsingUsingTasks(nbr);
            DoTestWithUsingInThreadPool(nbr);
            await Task.Delay(-1);
        }

        static async Task DoTestWithoutUsing(int nbr)
        {
            for (int i = 0; i < nbr; i++)
            {
                IDbConnection db = factory.OpenDbConnection();
                await db.SingleAsync<int>("SELECT 1");
            }
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}\tDoTestWithoutUsing: DONE");
        }

        static async Task DoTestWithUsing(int nbr)
        {
            for (int i = 0; i < nbr; i++)
            {
                using (IDbConnection db = factory.OpenDbConnection())
                {
                    await db.SingleAsync<int>("SELECT 1");
                }
            }
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}\tDoTestWithUsing: DONE");
        }

    

        static async Task DoTestWithUsingUsingTasks(int nbr)
        {
            List<Task> tasks = new();
            for (int i = 0; i < nbr; i++)
            {
                tasks.Add(Task.Run(() =>
                {
                    using (IDbConnection db = factory.OpenDbConnection())
                    {
                        db.Single<int>("SELECT 1");
                    }
                }));
            }
            await Task.WhenAll(tasks);
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}\tDoTestWithUsingInParallell: DONE");
        }

        static void DoTestWithUsingInThreadPool(int nbr)
        {
            for (int i = 0; i < nbr; i++)
            {
                ThreadPool.QueueUserWorkItem((o) =>
                {
                    using (IDbConnection db = factory.OpenDbConnection())
                    {
                        db.Single<int>("SELECT 1");
                    }
                });
            }
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}\tDoTestWithUsingInThreadPool: DONE");
        }
    }
}

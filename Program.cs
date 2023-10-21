using System;
using System.IO;
using System.Reflection.Emit;
using System.Security.Cryptography;
using System.Text;

namespace HelloApp
{
    class Program
    {
        public static string CalculateHash(string input)
        {
            using SHA256 hash = SHA256.Create();
            return Convert.ToHexString(hash.ComputeHash(Encoding.ASCII.GetBytes(input))).ToLower();
        }
        public static List<string> ProductPasswords()
        {
            List<string> passwords = new List<string>();
            for (char a = 'a'; a <= 'z'; a++)
            {
                for (char b = 'a'; b <= 'z'; b++)
                {
                    for (char c = 'a'; c <= 'z'; c++)
                    {
                        for (char d = 'a'; d <= 'z'; d++)
                        {
                            for (char e = 'a'; e <= 'z'; e++)
                            {
                                passwords.Add(a.ToString() + b.ToString() + c.ToString() + d.ToString() + e.ToString());
                            }
                        }
                    }
                }
            }
            return passwords;
        }

        static string GetPasswordFromIndex(int index, char[] alphabet, int passLen)
        {
            char[] password = new char[passLen];
            
            for (int i = 0; i < passLen; i++)
            {
                password[i] = alphabet[index % alphabet.Length];
                index /= alphabet.Length;
            }

            return new string(password);
        }

        public static List<string> BruteForceHashInRange(int firstIndex, int lastIndex, string hash, char[] alphabet, int passLen)
        {
            List<string> passwords = new List<string>();

            for (int i = firstIndex; i < lastIndex; i++)
            {
                string password = GetPasswordFromIndex(i, alphabet, passLen);

                if (CalculateHash(password) == hash)
                {
                    passwords.Add(password);
                }
            }

            return passwords;
        }

        static List<string> BruteForceHash(int numThread, string hash)
        {

            char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
            int passwordLen = 5;
            List<string> passwords = new List<string>();
            int numPasswords = (int)Math.Pow(alphabet.Length, passwordLen);
            int passwordInThread = numPasswords / numThread;

            Task<List<string>>[] tasks = new Task<List<string>>[numThread];

            for (int i=0; i<numThread; i++)
            {
                int firstIndex = i * passwordInThread; 
                int lastIndex = (i == numThread - 1) ? numPasswords : (i + 1) * passwordInThread;

                tasks[i] = Task.Run(() => BruteForceHashInRange(firstIndex, lastIndex, hash, alphabet, passwordLen));
            }

            Task.WaitAll(tasks);
            foreach(Task<List<string>> task in tasks)
            {
                passwords.AddRange(task.Result);
            }

            return passwords;
        }

        static void Main(string[] args)
        {
            List<string> hashes = new List<string>()
            { "1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad", //zyzzx
                "3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b", //apple
                "74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f" }; //mmmmm


            string hash = "";

            DateTime start;
            TimeSpan time;


            int threadCount;
            Console.Write("Количество потоков > ");
            threadCount = int.Parse(Console.ReadLine());
            
            for (int i = 0; i< hashes.Count; i++)
            {
                Console.WriteLine("Подбор для хэша: " + hashes[i]);
                start =  DateTime.Now;
                List<string> res = BruteForceHash(threadCount, hashes[i]);
                time = DateTime.Now - start;

                for (int k=0; k < res.Count; k++)
                {
                    Console.WriteLine(res[k]);
                }

                Console.WriteLine(String.Format("Затраченное время: {0}.{1}", time.Seconds, time.Milliseconds.ToString().PadLeft(3, '0')));
            }

        }

    }
}


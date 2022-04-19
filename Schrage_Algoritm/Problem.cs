using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Schrage_Algorithm
{
    internal class Problem
    {
        public int countoftasks;
        public List<Task> Nn_listoftasks, Ng_listoftasks, sigma;
        public int countSorted;
        public int rMin;
        public int seed;
        public ulong Cmax;
        public Problem()
        {
            Cmax = 0;
            countSorted = 0;
            seed = 0;
            countoftasks = 0;
            Nn_listoftasks = new List<Task>();
            Ng_listoftasks = new List<Task>();
            sigma = new List<Task>();
        }
        public Problem(int s, int c)
        {
            Cmax = 0;
            seed = s;
            countSorted = 0;
            countoftasks = c;
            Nn_listoftasks = new List<Task>();
            Ng_listoftasks = new List<Task>();
            sigma = new List<Task>();
        }
        public void RandomElements()
        {
            Random random = new Random(seed);
            int[] rpd = new int[3];

            for (int i = 1; i <= countoftasks; i++)
            {
                rpd[0] = random.Next(countoftasks) + 1;
                rpd[1] = random.Next(countoftasks)+1;
                rpd[2] = random.Next(countoftasks)+1;
                Nn_listoftasks.Add(new Task(i, rpd[0], rpd[1], rpd[2]));
            }
        }
        public void Read_tasks_from_file()
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data/JACK3.DAT");
            //for different set of data change JACK3.DAT to JACK2.DAT or JACK1.DAT or change data in those files
            //Console.WriteLine(filePath); 
            string line;
            if (File.Exists(filePath))
            {
                StreamReader file = null;
                try
                {
                    file = new StreamReader(filePath);
                    countoftasks = int.Parse(file.ReadLine());
                    int i = 0;
                    while (i <= countoftasks && (line = file.ReadLine()) != null)
                    {
                        i++;
                        string[] bits = line.Split(' ');
                        Nn_listoftasks.Add(new Task(i, int.Parse(bits[0]), int.Parse(bits[1]), int.Parse(bits[2])));
                    }
                }
                finally
                {
                    if (file != null)
                        file.Close();
                }
            }
        }
        static void Swap(List<Task> list, int indexA, int indexB)
        {
            Task tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }
        public void SchrageScheduleTasks()
        {
            rMin = 1000000;
            ulong time = 1;
            int count_Nn = countoftasks;
            int countNg = 0;
            int i;
            while (count_Nn != 0 || countNg != 0)
            {
                for (int j = 0; count_Nn != 0 && j< count_Nn; j++)
                {
                    if(rMin> Nn_listoftasks[j].relaese_date) rMin = Nn_listoftasks[j].relaese_date;
                    if(((ulong)Nn_listoftasks[j].relaese_date <= time))
                    {
                        i = 0;
                        while (i < countNg && Nn_listoftasks[j].delivery_time < Ng_listoftasks[i].delivery_time) i++;
                        Ng_listoftasks.Insert(i,Nn_listoftasks[j]);
                        countNg++;
                        Nn_listoftasks.RemoveAt(j);
                        count_Nn--;
                        j--;
                        countSorted++;
                    }
                }
                if (countNg==0)
                {
                    time = (ulong) rMin;
                }
                else 
                {
                    sigma.Add(Ng_listoftasks[0]);
                    time += (ulong)Ng_listoftasks[0].processing_time;
                    Ng_listoftasks.RemoveAt(0);
                    countSorted--;
                    countNg--;
                }
            }
            Cmax = time + (ulong)sigma[countoftasks - 1].delivery_time;
        }
        public void Schrage_pmtn_ScheduleTasks()
        {
            rMin = 1000000;
            ulong time = 1, ct = 1,lp=0;
            int count_Nn = countoftasks;
            int countNg = 0;
            int i;
            Task l = new Task();
            while (count_Nn != 0 || countNg != 0)
            {
                for (int j = 0; count_Nn != 0 && j < count_Nn; j++)
                {
                    if (rMin > Nn_listoftasks[j].relaese_date) rMin = Nn_listoftasks[j].relaese_date;
                    if (((ulong)Nn_listoftasks[j].relaese_date <= time))
                    {
                        i = 0;
                        while (i < countNg && Nn_listoftasks[j].delivery_time < Ng_listoftasks[i].delivery_time) i++;
                        Ng_listoftasks.Insert(i, Nn_listoftasks[j]);
                        countNg++;
                        Nn_listoftasks.RemoveAt(j);
                        count_Nn--;
                        j--;
                        if (countSorted > 0 && Ng_listoftasks[i].delivery_time > l.delivery_time && ((int)ct+ sigma[countSorted - 1].processing_time) > Ng_listoftasks[i].relaese_date)
                        {
                            lp = time - (ulong) Ng_listoftasks[i].relaese_date;
                            time = (ulong)Ng_listoftasks[i].relaese_date;
                            if (lp > 0)
                            {
                                if (sigma[countSorted - 1].done)
                                {
                                    l.processing_time = (int) lp;
                                    Ng_listoftasks.Insert(i + 1, l);
                                }
                                    sigma[countSorted - 1].done = false;
                            }
                        }
                    }
                }
                
                if (countNg == 0)
                {
                    time = (ulong)rMin;
                }
                else
                {
                    ct = time;
                    sigma.Add(Ng_listoftasks[0]);
                    time += (ulong)Ng_listoftasks[0].processing_time;
                    l = new Task();
                    l.id = Ng_listoftasks[0].id;
                    l.processing_time = Ng_listoftasks[0].processing_time;
                    l.delivery_time = Ng_listoftasks[0].delivery_time;
                    l.relaese_date = Ng_listoftasks[0].relaese_date;
                    Ng_listoftasks.RemoveAt(0);
                    countSorted++;
                    countNg--;
                }
            }
            Cmax = time + (ulong)sigma[countoftasks - 1].delivery_time;
        }

    }
}

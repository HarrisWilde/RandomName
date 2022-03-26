using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Threading;

namespace RandomName
{
    public class NameEventArgs: EventArgs
    {
        public string Name { get; set; }
    }

    public delegate void NameEventHandler(object sender, NameEventArgs e);
    public class RandomNameSelector
    {
        private NameEventHandler SelectingNameEventHandler;
        private NameEventHandler SelectedNameEventHandler;

        public event NameEventHandler Selecting
        {
            add { this.SelectingNameEventHandler += value; }
            remove { this.SelectingNameEventHandler -= value; }
        }

        public event NameEventHandler Selected
        {
            add { this.SelectedNameEventHandler += value; }
            remove { this.SelectedNameEventHandler -= value; }
        }

        private List<string> _List;
        public BindingList<string> _Namelist;
        public bool IsSelecting;

        public RandomNameSelector()
        {
            List<string> list = new List<string>();
            if (!System.IO.File.Exists("class.txt"))
            {
                System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("class.txt", true, new System.Text.UTF8Encoding(false));
                streamWriter.Close();
            } else
            {
                System.IO.StreamReader streamReader = new System.IO.StreamReader("class.txt",true);
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    if (line.Length > 0)
                    {
                        list.Add(line);
                    }
                }
            }

            _List = list;
            _Namelist = new BindingList<string>();
            _List.ForEach(x => _Namelist.Add(x));
            IsSelecting = false;
        }

        public void ResetNameList()
        {
            _Namelist.Clear();
            _List.ForEach(x => _Namelist.Add(x));
        }

        public async Task StartSelecting()
        {
            IsSelecting = true;
            await Task.Run(() =>
            {
                Random random = new Random();
                int index = 0;
                while (IsSelecting)
                {
                    index = random.Next(0, _Namelist.Count);
                    if (SelectingNameEventHandler != null)
                    {
                        SelectingNameEventHandler.Invoke(this, new NameEventArgs { Name = _Namelist[index] });
                    }
                    Thread.Sleep(50);
                }

                SelectedNameEventHandler.Invoke(this, new NameEventArgs { Name = _Namelist[index] });
            });
            
        }

        public async Task StartSelectingWithTime(int times)
        {
            await Task.Run(() =>
            {
                Random random = new Random();
                int index = 0;
                for (int i = 0; i < times; i++)
                {
                    index = random.Next(0, _Namelist.Count);
                    if (SelectingNameEventHandler != null)
                    {
                        SelectingNameEventHandler.Invoke(this, new NameEventArgs { Name = _Namelist[index] });
                        SelectedNameEventHandler.Invoke(this, new NameEventArgs { Name = _Namelist[index] });
                    }
                    Thread.Sleep(100);
                }
            });

        }
    }
}

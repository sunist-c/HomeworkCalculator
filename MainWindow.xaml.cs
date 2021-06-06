using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Data;
using HomeworkCalculator.Module;
using HomeworkCalculator.ViewModule;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using HomeworkCalculator.Config;
using HomeworkCalculator.Algorithm;
using System.Runtime.InteropServices;
using System.Windows.Media;

namespace HomeworkCalculator
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<FormsView> viewList = new ObservableCollection<FormsView>();
        char[] banCharac = new[] { '.', '\\', '<', '>', ':', '?', '*', '\"', '/' };
        public MainWindow()
        {
            InitializeComponent();
            viewList.Add(new FormsView() { Name = "数据为空" });
            DataViewer.ItemsSource = viewList;
            Console.WriteLine(JsonConvert.SerializeObject(new configType()));
        }

        private void BroseNameList_Click(object sender, RoutedEventArgs e)
        {
            var x = new OpenFileDialog();
            x.Filter = "配置文件(config.json)|config.json";
            if (x.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //System.Windows.MessageBox.Show(x.FileName);
                FileInfo info = new FileInfo(x.FileName);
                FileStream fs = new FileStream(x.FileName, System.IO.FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                string buffer = sr.ReadToEnd();
                fs.Close();
                sr.Close();
                configType type = JsonConvert.DeserializeObject<configType>(buffer);
                Config.Config.Load(type, info.DirectoryName);
                FileType.ItemsSource = Config.Config.AllFileType;
                viewList.Clear();
                var original = CsvFileLoader.Load(Path.Combine(Config.Config.ConfigPath, "student.csv"));
                foreach (var r in original) viewList.Add(r);

                NameListPath.Text = Path.Combine(Config.Config.ConfigPath, "config.json");
                StatusBarText.Text = "配置文件已更新";
            }
        }

        private void FileNameFormatUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (Config.Config.ConfigPath is null || Config.Config.ConfigPath == "")
            {
                StatusBarText.Text = "未给定配置，无法进行操作";
                return;
            }
            StatusBarText.Text = "正在重新扫描文件，这是一个耗时操作，请稍后";
            viewList.Clear();
            var original = CsvFileLoader.Load(Path.Combine(Config.Config.ConfigPath, "student.csv"));
            foreach (var c in banCharac)
            {
                if (FileNameFormat.Text.Contains(c))
                {
                    StatusBarText.Text = "给定的命名规则不能包含特殊字符，该操作可能会导致文件损坏";
                    return;
                }
            }
            
            string[] format = FileNameFormat.Text.Split('|');
            
            switch (Config.Config.MatchingRules)
            {
                case "Name":
                    foreach (var st in original)
                    {
                        DirectoryInfo info = new DirectoryInfo(Config.Config.ConfigPath);
                        var ii = info.GetFiles();
                        foreach (var finfo in ii)
                        {
                            if (finfo.Name.Contains(st.Name))
                            {
                                foreach (var ex in FileType.SelectedItems)
                                {
                                    if (ex.ToString() == finfo.Extension.ToUpper())
                                        st.Status = "正常提交";
                                }
                                if (st.Status != "正常提交") st.Status = "格式错误";
                                st.FilePath = finfo.Name;
                                break;
                            }
                            else
                                st.Status = "未提交";
                        }
                        viewList.Add(st);
                    }
                    break;
                case "StudentNumber":
                    foreach (var st in original)
                    {
                        DirectoryInfo info = new DirectoryInfo(Config.Config.ConfigPath);
                        var ii = info.GetFiles();
                        foreach (var finfo in ii)
                        {
                            if (finfo.Name.Contains(st.StudentNumber))
                            {
                                foreach (var ex in FileType.SelectedItems)
                                {
                                    if (ex.ToString() == finfo.Extension.ToUpper())
                                        st.Status = "正常提交";
                                }
                                if (st.Status != "正常提交") st.Status = "格式错误";
                                st.FilePath = finfo.Name;
                                break;
                            }
                            else
                                st.Status = "未提交";
                        }
                        viewList.Add(st);
                    }
                    break;
                case "Strict":
                    foreach (var st in original)
                    {
                        string buffer = "";
                        for (int i = 0; i < format.Length; ++i)
                        {
                            if (format[i] == "$Name")
                                buffer += st.Name;
                            else if (format[i] == "$StudentNumber")
                                buffer += st.StudentNumber;
                            else
                                buffer += format[i];
                        }

                        DirectoryInfo info = new DirectoryInfo(Config.Config.ConfigPath);
                        var ii = info.GetFiles();
                        foreach (var finfo in ii)
                        {
                            if (finfo.Name.Split('.')[0] == buffer)
                            {
                                foreach (var ex in FileType.SelectedItems)
                                {
                                    if (ex.ToString() == finfo.Extension.ToUpper())
                                        st.Status = "正常提交";
                                }

                                if (st.Status != "正常提交") st.Status = "格式错误";
                                st.FilePath = finfo.Name;
                                break;
                            }
                            else if (finfo.Name.Contains(st.Name) || finfo.Name.Contains(st.StudentNumber))
                            {
                                st.Status = "文件名错误";
                                st.FilePath = finfo.Name;
                                break;
                            }
                            else
                                st.Status = "未提交";
                        }
                        viewList.Add(st);
                    }
                    break;
                default:
                    break;
            }

            SetBrush();
            StatusBarText.Text = "文件扫描完成，结果已打印";
        }

        private void CheckStatus_Click(object sender, RoutedEventArgs e)
        {
            FileNameFormatUpdate_Click(sender, e);
        }

        private void ChangeFileName_Click(object sender, RoutedEventArgs e)
        {
            if (Config.Config.ConfigPath is null || Config.Config.ConfigPath == "")
            {
                StatusBarText.Text = "未给定配置，无法进行操作";
                return;
            }

            if (FileNameFormat.Text == "")
            {
                StatusBarText.Text = "未给定命名规则，无法进行操作";
                return;
            }
            foreach (var c in banCharac)
            {
                if (FileNameFormat.Text.Contains(c))
                {
                    StatusBarText.Text = "给定的命名规则不能包含特殊字符，该操作可能会导致文件损坏";
                    return;
                }
            }

            StatusBarText.Text = "正在更改名称，这是一个耗时操作，请稍候";
            var original = CsvFileLoader.Load(Path.Combine(Config.Config.ConfigPath, "student.csv"));
            string[] format = FileNameFormat.Text.Split('|');
            foreach (var st in original)
            {
                string buffer = "";
                for (int i = 0; i < format.Length; ++i)
                {
                    if (format[i] == "$Name")
                        buffer += st.Name;
                    else if (format[i] == "$StudentNumber")
                        buffer += st.StudentNumber;
                    else
                        buffer += format[i];
                }
                DirectoryInfo info = new DirectoryInfo(Config.Config.ConfigPath);
                var ii = info.GetFiles();
                foreach (var finfo in ii)
                {
                    if (finfo.Name.Contains(st.Name) || finfo.Name.Contains(st.StudentNumber))
                    {
                        try
                        {
                            File.Move(finfo.FullName, finfo.FullName.Replace(finfo.Name, $"{buffer}{finfo.Extension}"));
                        }
                        catch (Exception ex)
                        {
                            System.Windows.MessageBox.Show(ex.Message, "异常提示", MessageBoxButton.OK, MessageBoxImage.Error);
                            StatusBarText.Text = "更改名称失败，详情请见异常提示";
                        }
                    }
                }
            }
            StatusBarText.Text = "名称更改完成";
        }

        private void UnZip_Click(object sender, RoutedEventArgs e)
        {
            if (Config.Config.ConfigPath is null || Config.Config.ConfigPath == "")
            {
                StatusBarText.Text = "未给定配置，无法进行操作";
                return;
            }

            System.Windows.MessageBox.Show("暂未添加解压依赖库，此功能暂时无法使用\n建议使用系统终端/批处理程序来替代本步骤", "提示", MessageBoxButton.OK, (MessageBoxImage)MessageBoxIcon.Warning);
        }

        private void CopyToPath_Click(object sender, RoutedEventArgs e)
        {
            if (Config.Config.ConfigPath is null || Config.Config.ConfigPath == "")
            {
                StatusBarText.Text = "未给定配置，无法进行操作";
                return;
            }

            var s = new FolderBrowserDialog();
            if (s.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                StatusBarText.Text = "正在复制文件，这是一个耗时操作，请稍候";
                var original = CsvFileLoader.Load(Path.Combine(Config.Config.ConfigPath, "student.csv"));
                foreach (var st in original)
                {
                    DirectoryInfo info = new DirectoryInfo(s.SelectedPath);
                    DirectoryInfo orgInfo = new DirectoryInfo(Config.Config.ConfigPath);
                    var ii = orgInfo.GetFiles();
                    foreach (var finfo in ii)
                    {
                        if (finfo.Name.Contains(st.Name) || finfo.Name.Contains(st.StudentNumber))
                        {
                            try
                            {
                                File.Copy(finfo.FullName, finfo.FullName.Replace(finfo.DirectoryName, info.FullName));
                            }
                            catch (Exception ex)
                            {
                                System.Windows.MessageBox.Show(ex.Message, "异常提示", MessageBoxButton.OK, MessageBoxImage.Error);
                                StatusBarText.Text = "更改名称失败，详情请见异常提示";
                            }
                        }
                    }
                }
                StatusBarText.Text = "文件复制完成";
            }
        }

        private void OutputData_Click(object sender, RoutedEventArgs e)
        {
            if (Config.Config.ConfigPath is null || Config.Config.ConfigPath == "")
            {
                StatusBarText.Text = "未给定配置，无法进行操作";
                return;
            }

            StatusBarText.Text = "正在输出结果，这是一个耗时操作，请稍候";
            var dialog = new SaveFileDialog();
            dialog.Filter = "CSV表格(*.csv)|*.csv";
            dialog.FileName = "Output";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Console.WriteLine(dialog.FileName);

                try
                {
                    FileStream fs = new FileStream(dialog.FileName, System.IO.FileMode.OpenOrCreate);
                    StreamWriter sw = new StreamWriter(fs);
                    foreach (var r in viewList)
                        sw.WriteLine($"{r.Name},{r.StudentNumber},{r.FilePath},{r.Status}");
                    sw.Close();
                    fs.Close(); 
                    StatusBarText.Text = $"已成功输出{new FileInfo(dialog.FileName).Name}，该文件的编码格式为UTF-8";
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, "异常提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    StatusBarText.Text = "输出失败，可能是文件被占用了";
                }
            }
            else
                StatusBarText.Text = "输出失败，可能是用户中止了操作";
        }

        private void GoToGitHub_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GoToDocument_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetBrush()
        {
            for (int i = 0; i < viewList.Count; ++i)
            {
                if (viewList[i].Status == "未提交")
                {
                    DataGridRow dr = (DataGridRow)DataViewer.ItemContainerGenerator.ContainerFromIndex(i);
                    if (dr == null)
                    {
                        DataViewer.UpdateLayout();
                        DataViewer.UpdateLayout();
                        DataViewer.ScrollIntoView(DataViewer.Items[i]);
                        dr = (DataGridRow)DataViewer.ItemContainerGenerator.ContainerFromIndex(i);
                    }
                    dr.Background = new SolidColorBrush(Colors.Red);
                }
                else if (viewList[i].Status == "格式错误")
                {
                    DataGridRow dr = (DataGridRow)DataViewer.ItemContainerGenerator.ContainerFromIndex(i);
                    if (dr == null)
                    {
                        DataViewer.UpdateLayout();
                        DataViewer.UpdateLayout();
                        DataViewer.ScrollIntoView(DataViewer.Items[i]);
                        dr = (DataGridRow)DataViewer.ItemContainerGenerator.ContainerFromIndex(i);
                    }
                    dr.Background = new SolidColorBrush(Colors.Yellow);
                }
                if (viewList[i].Status == "文件名错误")
                {
                    DataGridRow dr = (DataGridRow)DataViewer.ItemContainerGenerator.ContainerFromIndex(i);
                    if (dr == null)
                    {
                        DataViewer.UpdateLayout();
                        DataViewer.UpdateLayout();
                        DataViewer.ScrollIntoView(DataViewer.Items[i]);
                        dr = (DataGridRow)DataViewer.ItemContainerGenerator.ContainerFromIndex(i);
                    }
                    dr.Background = new SolidColorBrush(Colors.Yellow);
                }
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetBrush();
        }
    }
}

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
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Win32;
using System.IO;

using System.Windows.Forms;
using System.Reflection;

namespace FileNameChange
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Assembly version 읽어오기
            Assembly assemObj = Assembly.GetExecutingAssembly();
            Version v = assemObj.GetName().Version; // 현재 실행되는 어셈블리..dll의 버전 가져오기
            int majorV = v.Major; // 주버전
            int minorV = v.Minor; // 부버전
            int buildV = v.Build; // 빌드번호
            int revisionV = v.Revision; // 수정번호

            this.Title = "FileNameConver: " + majorV.ToString() + "." + minorV.ToString() + "." + buildV.ToString() + "." + revisionV.ToString();
        }

        string SearchPath = "";
        string SearchOption = "";
        private Microsoft.Win32.SaveFileDialog SaveFileMade;
        TreeViewItem RootItem = new TreeViewItem();
        TreeViewItem ChangeRootItem = new TreeViewItem();

        string[] fileEntries;
        string[] ChangefileEntries;

        private void BtBrowse_Click(object sender, RoutedEventArgs e)
        {

            System.Windows.Forms.FolderBrowserDialog dlg = new System.Windows.Forms.FolderBrowserDialog();

            //ShowDialog로 다이얼로그를 열어서 선택하면 OK 이고 아무 선택을 하지 않으면 No 인듯
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TBFileDirectory.Text = dlg.SelectedPath;//선택한 경로
                SearchPath = TBFileDirectory.Text;
            }
            else
            {
                System.Windows.MessageBox.Show("Not Selected");
            }       
        }

        private void BtFindFile_Click(object sender, RoutedEventArgs e)
        {
            RootItem.Items.Clear();
            TVFindFileList.Items.Clear();
            try
            {
                SearchPath = TBFileDirectory.Text;
                RootItem.Header = SearchPath;
                // xaml파일에 미리 정의된 TreeView Control
                TVFindFileList.Items.Add(RootItem);
                SearchOption = TBFindFileType.Text;
                // 지정된 폴더로 부터 하위끝까지 검색하여 트리를 만듬
                ProcessDirectory(SearchPath, SearchOption, ref RootItem);

            }
            catch
            {
                System.Windows.MessageBox.Show("Can't File Find");
            }

        }


        // 폴더 검색은 재귀함수를 통해 하위 노드로 계속 들어감
        private void ProcessDirectory(string targetDirectory, string Seatchoption, ref TreeViewItem OwnerNode)
        {
            int count = 0;
            // Process the list of files found in the directory.
            fileEntries = Directory.GetFiles(targetDirectory, Seatchoption);
            foreach (string fileName in fileEntries)
            {
                if (File.Exists(fileName))
                {
                    ProcessFile(fileName, ref OwnerNode);
                    count++;
                }
            }
            LbFindFileCount.Content = count;

  //k          ConverttoData(SearchPath, ref fileEntries);
        }

        private void ProcessFile(string path, ref TreeViewItem OwnerNode)
        {
            TreeViewItem Item = new TreeViewItem();
            Item.Header = path;
            OwnerNode.Items.Add(Item);
        }
/*
        String[][] FileName = new string[31][];
        private void ConverttoData(string targetDirectory, ref string[] fileEntries)
        {
            int shots = 30;

            FileName.Initialize();

            String textparameter = "";
            int count = 10;  //file count

            count = fileEntries.Length;

            int i = 0;
            int j = 0;

            for (i = 0; i < shots + 1; i++)
            {
                FileName[i] = new string[count];//해당 행만큼의 1차원 배열 생성
            }

            for (i = 0; i < count; i++)
            {
                j = 0;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(fileEntries[i].ToString()))
                {
                    //       FileName[j][i] = fileEntries[i].ToString();  //파일이름을 조작하여 날자시간만 추려야 함
                    FileName[j][i] = fileEntries[i];  //파일이름을 조작하여 날자시간만 추려야 함
                    textparameter = sr.ReadLine();
                    for (j = 1; j < shots + 1; j++)
                    {
                        FileName[j][i] = sr.ReadLine();
                    }
                    sr.Close();
                }
            }
        }
*/
        private void BtChangeStart_Click(object sender, RoutedEventArgs e)
        {
            if (fileEntries == null || fileEntries.Length < 1)
            {
                System.Windows.MessageBox.Show("Not find File");
                return;
            }

            ChangefileEntries = new string[1024];

            string savepath = SearchPath + "\\Save";
            string filefullpath = "";
            string filename = "";
            string extentionname = "";

            int count = 0;

            if (!Directory.Exists(savepath)) Directory.CreateDirectory(savepath);

            TVChangeFileList.Items.Clear();

            TreeViewItem treeItem = new TreeViewItem();
            treeItem.Header = savepath;
            TVChangeFileList.Items.Add(treeItem);

            for(int i = 0; i<fileEntries.Length; i++)
            {
                filefullpath = System.IO.Path.GetDirectoryName(fileEntries[i]);
                extentionname = System.IO.Path.GetExtension(fileEntries[i]);
                filename = System.IO.Path.GetFileNameWithoutExtension(fileEntries[i]);

                string[] filenamesplit = filename.Split('_');
                // Date ,  time, ID, ODOS, Treatx, CLRT, count
                
                if(filenamesplit[4].Contains("treat"))
                {
                    string[] treatname_arr = filenamesplit[4].Split(new string[] { "treat" }, StringSplitOptions.None);
                    int treat_count = int.Parse(treatname_arr[1]);
                    treat_count++;
                    filenamesplit[4] = "treat" + treat_count.ToString();
                }
                else if (filenamesplit[4].Contains("test"))
                {
                    string[] testname_arr = filenamesplit[4].Split(new string[] { "test" }, StringSplitOptions.None);
                    int test_count = int.Parse(testname_arr[1]);
                    test_count++;
                    filenamesplit[4] = "test" + test_count.ToString();
                }

                int shotcountsplit = int.Parse(filenamesplit[6]);
                shotcountsplit++;
                filenamesplit[6] = shotcountsplit.ToString();

                if (filenamesplit[5] == "Cl") filenamesplit[5] = "classic";
                else filenamesplit[5] = "ramp";
                string changefilename = savepath + "\\" + filenamesplit[0] + "_" + filenamesplit[1] + "_" + filenamesplit[4] + "_" + filenamesplit[5] + "_" + filenamesplit[2] + "_" + filenamesplit[3] + "_" + filenamesplit[6] + extentionname;

                System.IO.File.Copy(fileEntries[i], changefilename, true);
                ProcessFile(changefilename, ref treeItem);

                ChangefileEntries[i] = string.Format("{0}",changefilename);   //kek150128 여기 runtime error

                count++;
            }
            LbChangeFileCount.Content = count;
        }


    }
}

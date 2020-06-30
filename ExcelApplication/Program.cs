using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;


/// <summary>
/// 不足  ：对于bool类型的字段，没有写入到实体类中，需要完善
/// </summary>


//如果要支持xlsx格式表格，请在本机电脑安装这个
//http://download.microsoft.com/download/7/0/3/703ffbcb-dc0c-4e19-b0da-1463960fdcdb/AccessDatabaseEngine.exe

namespace ExcelTool
{

    class Program
    {
        public static bool isReadPerSheet=true;

        private static string SourceExcelPath; //源excel路径
        private static string OutBytesFilePath; //bytes文件路径
        private static string OutCSharpFilePath; //c#脚本路径
        private static string OutLuaFilePath; //lua脚本路径

        private static string OutBytesFilePath_Server; //服务器端表格文件路径
        private static string OutCSharpFilePath_Server; //服务器端c#脚本路径

        private static CreateFileConfigsEntity configEntity;

        private static string OutXmlFilePath;   //xml路径
        private static string OutEntityFilePath;   //entity路径
        private static string OutDbModelFilePath;   //DbModel路径

        private static string OutConstFilePath;   //DbModel路径

        static string currDir = "";


        static int excelFileNumber = 0;

        static string appRootPath;
        static string CreateFolder(string path,string folderName)
        {
            
            if (string.IsNullOrEmpty(path)) path = currDir + "\\" + folderName;
            if (!Directory.Exists(path))
            {
               
                Directory.CreateDirectory(path);
                return path;
            }
            return path;
        }

        static void LoadXml()
        {
            string xmlFilePath = Environment.CurrentDirectory + "\\CreateFileConfigs.xml";
            if (File.Exists(xmlFilePath))
            {
                string str = "";
                using (FileStream fs = new FileStream(xmlFilePath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        str = sr.ReadToEnd();
                    }
                }
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(xmlFilePath);
                    // doc.LoadXml(str);
                    XmlElement root = doc.DocumentElement;   //获取根节点
                    foreach (XmlElement node in root)
                    {
                        CreateFileConfigsEntity t = new CreateFileConfigsEntity();
                        t.XmlElementToObject(node);
                        if (t.id == "1")
                        {
                            configEntity = t;
                           // Console.WriteLine(configEntity.id + " " + configEntity.isCreateByteFile + " " + configEntity.isCreateDBModelFile + " " + configEntity.isCreateEntityFile + " " + configEntity.isCreateJsonFile +
                             //   " " + configEntity.isCreateReadJsonFun + " " + configEntity.isCreateReadXmlFun + " " + configEntity.entityBaseName + " " + configEntity.dbModelBaseName);
                        }
                    }

                    if (configEntity == null) WriteColorLine("xml配置文件有错误", ConsoleColor.Red);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("读取xml配置文件失败 请检查文件格式是否有误: " + ex.Message);
                }
               
            }
            else
            {
                WriteColorLine("不存在 生成各种配置文件的 xml配置文件", ConsoleColor.Red);
            }
        }
        static void Main(string[] args)
        {
            currDir = Environment.CurrentDirectory;
            WriteColorLine("-------------------作    者：Lixi ----------------------- \r\n",ConsoleColor.Yellow);
            WriteColorLine("-------------------EXCEL表格生成工具--------------------- \r\n",ConsoleColor.Green);
            // Console.ForegroundColor = color;
            // LoadXml();
            //LoadConfig();
            //G:\配置文件\工具\ExcelToConfigs\bin\Debug
            // string appRootDir =DirectoryInfo  Environment.CurrentDirectory

            DirectoryInfo appRootDir = new DirectoryInfo(Environment.CurrentDirectory);
       
            appRootPath = appRootDir.Parent.Parent.Parent.FullName+"\\";
            Console.WriteLine("app 根目录 ：" + appRootPath);
            WriteColorLine(currDir+"------------------- ----------------------- \r\n", ConsoleColor.Yellow);

            OutXmlFilePath =appRootPath + CreateFolder(SettingConfigDBModel.Instance.Get(ConstDefine.createXmlPath), "xml");
            OutEntityFilePath = appRootPath + CreateFolder(SettingConfigDBModel.Instance.Get(ConstDefine.createEntityPath), "entity");
            OutDbModelFilePath = appRootPath + CreateFolder(SettingConfigDBModel.Instance.Get(ConstDefine.createDbModelPath), "dbModel");
            OutConstFilePath = appRootPath + CreateFolder(SettingConfigDBModel.Instance.Get(ConstDefine.createConstPath), "Const");

            // ReadFiles(SourceExcelPath);

            string SourceExcelPath =appRootPath + SettingConfigDBModel.Instance.Get(ConstDefine.excelPath);
            if (!Directory.Exists(SourceExcelPath) && string.IsNullOrEmpty(SourceExcelPath))
            {
                WriteColorLine("-------------------没有Excel配置文件 ----------------------- \r\n", ConsoleColor.Red);
                WriteColorLine("Excel配置文件路径 ："+SourceExcelPath+" \r\n", ConsoleColor.Red);
                Console.ReadLine();
                return;
            }
            ReadFiles(SourceExcelPath);


           // Console.WriteLine("表格全部生成完成");
            WriteColorLine("\r\n"+ "表格全部生成完成 共有 ：" + excelFileNumber + "个Excel文件 \r\n", ConsoleColor.Red);
            // System.Diagnostics.Process.Start("explorer.exe",SettingConfigDBModel.Instance.Get(ConstDefine.createPath));

            string outCreatePath = appRootPath;
            outCreatePath = outCreatePath.Substring(0, outCreatePath.LastIndexOf('\\'));

            if (!Directory.Exists(outCreatePath)&&string.IsNullOrEmpty(outCreatePath))
            {
                outCreatePath = currDir;
            }
            System.Diagnostics.Process.Start("explorer.exe", outCreatePath);
            Console.ReadLine();
        }

       public static void WriteColorLine(string str, ConsoleColor color)
        {
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = color; Console.WriteLine(str);
            Console.ForegroundColor = currentForeColor;
        }

        private static void LoadConfig()
        {
            string configPath = Environment.CurrentDirectory + "\\config.txt";

            if (File.Exists(configPath))
            {
                string str = "";
                using (FileStream fs = new FileStream(configPath, FileMode.Open))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        str = sr.ReadToEnd();
                    }
                }

                if (!string.IsNullOrEmpty(str))
                {
                    string[] arr = str.Split('\n');
                    if (arr.Length >= 7)
                    {
                        SourceExcelPath =appRootPath + arr[0].Trim();
                        OutBytesFilePath = appRootPath + arr[1].Trim();
                        OutCSharpFilePath = appRootPath + arr[2].Trim();
                        OutLuaFilePath = appRootPath + arr[3].Trim();

                        OutBytesFilePath_Server = appRootPath + arr[4].Trim();
                        OutCSharpFilePath_Server = appRootPath + arr[5].Trim();
                        OutXmlFilePath = appRootPath + arr[6].Trim();
                    }
                }
            }
        }

        public static List<string> ReadFiles(string path)
        {
            string[] arr = Directory.GetFiles(path);

            List<string> lst = new List<string>();

            int len = arr.Length;
            for (int i = 0; i < len; i++)
            {
                string filePath = arr[i];
                FileInfo file = new FileInfo(filePath);
                if (file.Name.IndexOf("~$") > -1)
                {
                    continue;
                }
                if (file.Extension.Equals(".xls") || file.Extension.Equals(".xlsx"))
                {
                    excelFileNumber++;
                    ReadData(file.Extension.Equals(".xls"), file.FullName, file.Name.Substring(0, file.Name.LastIndexOf('.')));
                }
            }

          
            //Console.WriteLine(openFolderFullPath);
           // Console.WriteLine(OutXmlFilePath.LastIndexOf(':'));

           
            return lst;
        }
        static void ReadPerSheet(bool isXls, string filePath, string fileName)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            //把表格复制一下
            string newPath = filePath + ".temp";

            File.Copy(filePath, newPath, true);     
            string strConn = "";
            if (isXls)
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + newPath + ";" + "Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            }
            else
            {
                strConn = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source =" + newPath + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1'";
            }

            #region 获取工作表
         
                //定义存放的数据表  
                DataTable dtTest = new DataTable();
                //连接数据源  
                OleDbConnection connstr = new OleDbConnection(strConn);
                connstr.Open();
                //适配到数据源  
                dtTest = connstr.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                if (dtTest == null)//如果为空的话
                {
                    //return null;   
                }
                DataSet dsTest = new DataSet();
                int i = 0;
                String[] excelSheets = new String[dtTest.Rows.Count];
                foreach (DataRow row in dtTest.Rows)
                {
                    excelSheets[i] = row["TABLE_NAME"].ToString();
                    WriteColorLine("\r\n" + "工作表名称" + excelSheets[i], ConsoleColor.Red);

                    string sql = string.Format("select * from [{0}]", excelSheets[i]);
                    //定义存放的数据表  
                    OleDbDataAdapter adapter = new OleDbDataAdapter(sql, strConn);
                    adapter.Fill(dsTest, excelSheets[i].Replace("$", "").Replace("'", ""));//存入表明去掉sheet页名称中多余字符
                    i++;
                }
                for (int j = 0; j < dsTest.Tables.Count; j++)//遍历sheet页数据
                {
                    WriteColorLine("\r\n" + "工作表列数" + dsTest.Tables[j].Columns.Count, ConsoleColor.Red);

                }
                connstr.Close();
            

            #endregion



        }



        private static void ReadData(bool isXls, string filePath, string fileName)
        {

            if (string.IsNullOrEmpty(filePath)) return;

            //把表格复制一下
            string newPath = filePath + ".temp";

            File.Copy(filePath, newPath, true);

            string tableName = "Sheet1";
            string strConn = "";
            if (isXls)
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + newPath + ";" + "Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            }
            else
            {
                strConn = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source =" + newPath + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1'";
            }

           List<string> sheets= ExcelHelper.GetExcelSheetNames(filePath);
            foreach(var str in sheets)
            {
                Console.WriteLine(str);
            }
          

            //定义存放的数据表  
            DataTable dtTest = new DataTable();
            //连接数据源  
            OleDbConnection connstr = new OleDbConnection(strConn);
            connstr.Open();
            //适配到数据源  
            dtTest = connstr.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            if (dtTest == null)//如果为空的话
            {
                //return null;   
            }
            DataSet dsTest = new DataSet();
            int i = 0;
            String[] excelSheets = new String[dtTest.Rows.Count];
            foreach (DataRow row in dtTest.Rows)
            {
                excelSheets[i] = row["TABLE_NAME"].ToString();
                WriteColorLine("工作表名称：" + excelSheets[i]+ "\r\n", ConsoleColor.Red);

                string sql = string.Format("select * from [{0}]", excelSheets[i]);
                //定义存放的数据表  
                OleDbDataAdapter adapter = new OleDbDataAdapter(sql, strConn);
                adapter.Fill(dsTest, excelSheets[i].Replace("$", "").Replace("'", ""));//存入表明去掉sheet页名称中多余字符
                i++;
            }
            for (int j = 0; j < dsTest.Tables.Count; j++)//遍历sheet页数据
            {
               // WriteColorLine("\r\n" + "工作表列数" + dsTest.Tables[j].Columns.Count, ConsoleColor.Red);

            }

            DataTable dt = null;         
            dt = dsTest.Tables[0];

            connstr.Close();
            File.Delete(newPath);

            if (fileName.Equals("Sys_Localization", StringComparison.CurrentCultureIgnoreCase))
            {
                //多语言表 单独处理
                CreateLocalization(fileName, dt);
            }
            else
            {
                CreateData(fileName, dt);
                
                if(SettingConfigDBModel.Instance.Get(ConstDefine.isCreateXmlFile)=="1")
                CreateXml(fileName, dt);

                if(CanCreateConst(fileName, dt))
                {
                    CreateConstFile(fileName, dt);
                }
            }
        }


        private static void OldReadData(bool isXls, string filePath, string fileName)
        {

            if (string.IsNullOrEmpty(filePath)) return;

            //把表格复制一下
            string newPath = filePath + ".temp";

            File.Copy(filePath, newPath, true);

            string tableName = "Sheet1";
            string strConn = "";
            if (isXls)
            {
                strConn = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + newPath + ";" + "Extended Properties='Excel 8.0;HDR=NO;IMEX=1';";
            }
            else
            {
                strConn = "Provider = Microsoft.ACE.OLEDB.12.0; Data Source =" + newPath + ";Extended Properties='Excel 12.0;HDR=NO;IMEX=1'";
            }



            DataTable dt = null;

            string strExcel = "";
            OleDbDataAdapter myCommand = null;
            DataSet ds = null;
            strExcel = string.Format("select * from [{0}$]", tableName);
            myCommand = new OleDbDataAdapter(strExcel, strConn);
            ds = new DataSet();
            // myCommand.Fill(ds, "table1");
            myCommand.Fill(ds, strExcel.Replace("$", "").Replace("'", ""));
            dt = ds.Tables[1];

            WriteColorLine("\r\n" + "数量" + ds.Tables.Count, ConsoleColor.Red);

            myCommand.Dispose();

            File.Delete(newPath);

            if (fileName.Equals("Sys_Localization", StringComparison.CurrentCultureIgnoreCase))
            {
                //多语言表 单独处理
                CreateLocalization(fileName, dt);
            }
            else
            {
                CreateData(fileName, dt);

                if (SettingConfigDBModel.Instance.Get(ConstDefine.isCreateXmlFile) == "1")
                    CreateXml(fileName, dt);

                if (CanCreateConst(fileName, dt))
                {
                    CreateConstFile(fileName, dt);
                }
            }
        }

        #region 创建普通表
        //表头
        static string[,] tableHeadArr = null;


        private static void CreateConstFile(string fileName, DataTable dt)
        {
           

            int keyIndex = -1;
            int descIndex = -1;
            try
            {
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    string value = dt.Rows[0][i].ToString();
                    if (value == "key")
                    {
                        keyIndex = i;
                    }
                    if (value == "desc")
                    {
                        descIndex = i;
                    }
                }
              //  WriteColorLine( "key= "+keyIndex+" desc= "+descIndex, ConsoleColor.Green);

                if (keyIndex == -1 && descIndex == -1) return;


                
                

                StringBuilder sbr = new StringBuilder();
                sbr.Append("using UnityEngine;\r\n");
                
                sbr.Append("using System;\r\n\r\n");
                sbr.AppendFormat("public class {0}Const",fileName);
                sbr.Append("{\r\n");
                int rows = dt.Rows.Count;
                int columns = dt.Columns.Count;

                for (int i = 3; i < rows; i++)
                {
                     for (int j = columns-1; j > -1; j--)
                     {
                        if(j==descIndex)
                        {
                            sbr.Append("\t/// <summary>\r\n");
                            sbr.AppendFormat("\t/// {0}\r\n", dt.Rows[i][j].ToString());
                            sbr.Append("\t/// </summary>\r\n");
                        }
                        if (j == keyIndex)
                        {
                            
                            sbr.AppendFormat("\t public const string {0} = \"{1}\"; \r\n\r\n", dt.Rows[i][j].ToString(), dt.Rows[i][j].ToString());
                            
                        }
                    }  
                }
                sbr.Append("}");


                using (FileStream fs = new FileStream(string.Format("{0}/{1}Const.cs", OutConstFilePath, fileName), FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                          sw.Write(sbr.ToString());
                         WriteColorLine("表格=>" + fileName + " 生成 "+ fileName+"Const 文件完成 " +" \r\n", ConsoleColor.Green);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("表格=>" + fileName + " 处理失败:" + ex.Message);
            }
        }

        private static bool CanCreateConst(string fileName,DataTable dt)
        {
            
            if (dt == null)
                return false;
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                string value = dt.Rows[0][i].ToString();
                if (value == "key")
                {
                   // WriteColorLine(fileName, ConsoleColor.DarkBlue);
                    return true;
                }
            }
            return false;
        }

        private static void CreateXml(string fileName, DataTable dt)
        {
            int numberXmlData = 0;
            try
            {
                //数据格式 行数 列数 二维数组每项的值 这里不做判断 都用string存储
                tableHeadArr = null;

                CanCreateConst(fileName, dt);
                 
                StringBuilder sbr = new StringBuilder();
                sbr.Append("<?xml version=\"1.0\"?>\r\n");
                sbr.Append("<root>\r\n");
                using (MMO_MemoryStream ms = new MMO_MemoryStream())
                {

                    numberXmlData = 0;
                    int rows = dt.Rows.Count;
                    int columns = dt.Columns.Count;
                    tableHeadArr = new string[columns, 3];
                    ms.WriteInt(rows - 3); //减去表头的三行
                    ms.WriteInt(columns);                          
                    for (int i = 0; i < rows; i++)
                    {
                        if(string.IsNullOrEmpty(dt.Rows[i][0].ToString().Trim()))
                        {
                            break;
                        }
                        if(i>=3)
                        sbr.Append("<table ");                
                        for (int j = 0; j < columns; j++)
                        {
                            if (i < 3)
                            {
                                tableHeadArr[j, i] = dt.Rows[i][j].ToString().Trim();
                            }
                            else
                            {
                                string type = tableHeadArr[j, 0];
                                string value = dt.Rows[i][j].ToString().Trim();
                              //  Console.WriteLine("type=" + type + "||" + "value=" + value);
                              if(!string.IsNullOrEmpty(type))
                                {
                                    //todo 空置的时候也要写入
                                    sbr.AppendFormat("{0}=\"{1}\" ", type, value);
                                    if(string.IsNullOrEmpty(value))
                                    {
                                        char lieChar = (char)(65 + j);
                                        int hang = i + 1;

                                        int lie = j + 1;
                                        WriteColorLine(lieChar + hang.ToString() + " :第 " + hang + " 行第 " + lie + " 列数据有问题，没有赋值， 文件：" + fileName, ConsoleColor.Red);
                                    }

                                }else
                                {
                                    if(string.IsNullOrEmpty(type))
                                    {
                                        char lieChar = (char)(65 + j);
                                        int hang = i + 1;

                                        int lie = j + 1;
                                        WriteColorLine(lieChar + hang.ToString()+ " :第 " + hang + " 行第 " + lie + " 列数据有问题，没有赋值， 文件：" + fileName, ConsoleColor.Red);

                                    }

                                 
                                }

                                // sbr.AppendFormat("//创建时间：{0}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                            }
                        }
                        if (i >= 3)
                        {
                            numberXmlData++;
                            sbr.Append("/>\r\n");
                        }
                           
                    }
                    sbr.Append("</root>\r\n");

                }
                using (FileStream fs = new FileStream(string.Format("{0}/{1}.xml", OutXmlFilePath, fileName), FileMode.Create))
                {
                    using (StreamWriter sw = new StreamWriter(fs))
                    {
                        sw.Write(sbr.ToString());
                        WriteColorLine("表格=>" + fileName + " 生成 XML 文件完成,共生成 "+ numberXmlData +" 条数据 路径："+OutXmlFilePath+" \r\n",ConsoleColor.Green);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("表格=>" + fileName + " 处理失败:" + ex.Message);
            }
        }
        private static void CreateData(string fileName, DataTable dt)
        {
            try
            {
                //数据格式 行数 列数 二维数组每项的值 这里不做判断 都用string存储
                tableHeadArr = null;

                byte[] buffer = null;

                using (MMO_MemoryStream ms = new MMO_MemoryStream())
                {
                    int rows = dt.Rows.Count;
                    int columns = dt.Columns.Count;

                    tableHeadArr = new string[columns, 3];

                    ms.WriteInt(rows - 3); //减去表头的三行
                    ms.WriteInt(columns);
                    for (int i = 0; i < rows; i++)
                    {
                        for (int j = 0; j < columns; j++)
                        {
                            if (i < 3)
                            {
                                tableHeadArr[j, i] = dt.Rows[i][j].ToString().Trim();
                            }
                            else
                            {
                                string type = tableHeadArr[j, 1];
                                string value = dt.Rows[i][j].ToString().Trim();

                                //Console.WriteLine("type=" + type + "||" + "value=" + value);

                                switch (type.ToLower())
                                {
                                    case "int":
                                        ms.WriteInt(string.IsNullOrEmpty(value) ? 0 : int.Parse(value));
                                        break;
                                    case "long":
                                        ms.WriteLong(string.IsNullOrEmpty(value) ? 0 : long.Parse(value));
                                        break;
                                    case "short":
                                        ms.WriteShort(string.IsNullOrEmpty(value) ? (short)0 : short.Parse(value));
                                        break;
                                    case "float":
                                        ms.WriteFloat(string.IsNullOrEmpty(value) ? 0 : float.Parse(value));
                                        break;
                                    case "byte":
                                        ms.WriteByte(string.IsNullOrEmpty(value) ? (byte)0 : byte.Parse(value));
                                        break;
                                    case "bool":
                                        ms.WriteBool(string.IsNullOrEmpty(value) ? false : bool.Parse(value));
                                        break;
                                    case "double":
                                        ms.WriteDouble(string.IsNullOrEmpty(value) ? 0 : double.Parse(value));
                                        break;
                                    default:
                                        ms.WriteUTF8String(value);
                                        break;
                                }
                            }
                        }
                    }
                    buffer = ms.ToArray();
                }

                //------------------
                //写入文件
                //------------------

                //{

                //FileStream fs = new FileStream(string.Format("{0}\\{1}", OutBytesFilePath, fileName + ".bytes"), FileMode.Create);
                //fs.Write(buffer, 0, buffer.Length);
                //fs.Close();

                //Console.WriteLine("客户端表格=>" + fileName + " 生成bytes文件完成");

                //}

                //{
                //    FileStream fs = new FileStream(string.Format("{0}\\{1}", OutBytesFilePath_Server, fileName + ".bytes"), FileMode.Create);
                //    fs.Write(buffer, 0, buffer.Length);
                //    fs.Close();

                //    Console.WriteLine("服务器端表格=>" + fileName + " 生成bytes文件完成");
                //}

                // CreateEntity(fileName, tableHeadArr);
                if (SettingConfigDBModel.Instance.Get(ConstDefine.isCreateEntityFile)=="1")
                {
                    NewCreateEntity(fileName, tableHeadArr);
                    Console.WriteLine("表格=>" + fileName + " 生成实体脚本完成  文件路径："+OutEntityFilePath);

                }

                //  CreateServerEntity(fileName, tableHeadArr);
                //  Console.WriteLine("服务器表格=>" + fileName + " 生成实体脚本完成");

                if (SettingConfigDBModel.Instance.Get(ConstDefine.isCreateDBModelFile) == "1")
                {
                    // CreateDBModel(fileName, tableHeadArr);
                     NewCreateDBModel(fileName, tableHeadArr);
                    Console.WriteLine("表格=>" + fileName + " 生成数据访问脚本完成  文件路径：" + OutDbModelFilePath);
                }

               

                // CreateServerDBModel(fileName, tableHeadArr);
                //  Console.WriteLine("服务器表格=>" + fileName + " 生成数据访问脚本完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine("表格=>" + fileName + " 处理失败:" + ex.Message);
            }
        }



        /// <summary>
        /// 创建客户端实体
        /// </summary>
        private static void NewCreateEntity(string fileName, string[,] dataArr)
        {
            if (dataArr == null) return;

            StringBuilder sbr = new StringBuilder();
            sbr.Append("\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("//作    者：Lixi  \r\n");
            sbr.AppendFormat("//创建时间：{0}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //  sbr.Append("//备    注：此代码为工具生成 请勿手工修改\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("using System.Collections;\r\n");
        
            sbr.Append("\r\n");
            sbr.Append("/// <summary>\r\n");
            sbr.AppendFormat("/// {0}实体\r\n", fileName);
            sbr.Append("/// </summary>\r\n");
            sbr.AppendFormat("public partial class {0}Entity : {1}\r\n", fileName, SettingConfigDBModel.Instance.Get(ConstDefine.entityBaseName));
            sbr.Append("{\r\n");

            for (int i = 0; i < dataArr.GetLength(0); i++)
            {
                //去掉第一行（第一行是变量名称第一）
                if (i == 0) continue;

                //如果类型名称为空，则跳过处理
                if (string.IsNullOrEmpty(dataArr[i, 2])) continue;  
                sbr.Append("    /// <summary>\r\n");
                sbr.AppendFormat("    /// {0}\r\n", dataArr[i, 2]);
                sbr.Append("    /// </summary>\r\n");
                sbr.AppendFormat("    public {0} {1};\r\n", dataArr[i, 1], dataArr[i, 0]);

               // Console.WriteLine("type :" + dataArr[i, 1] + " " + dataArr[i, 0]);
                sbr.Append("\r\n");
            }

            if(SettingConfigDBModel.Instance.Get(ConstDefine.isCreateReadXmlFun)=="1")
            {

                sbr.AppendFormat("  public {0}Entity XmlToObject(System.Xml.XmlElement element)\r\n", fileName);
                sbr.Append("  {\r\n");
                // content = element.GetAttribute("content");

                for (int i = 0; i < dataArr.GetLength(0); i++)
                {
                    string type = dataArr[i, 1];

                    //如果类型名称为空，则跳过处理          
                    if (string.IsNullOrEmpty(dataArr[i, 2])) continue;

                    if (type.ToLower() == "string")
                    {
                        sbr.AppendFormat("\t if (element.HasAttribute(\"{0}\") && !string.IsNullOrEmpty(element.GetAttribute(\"{1}\")))\r\n", dataArr[i, 0], dataArr[i, 0]);
                        sbr.Append("\t {\r\n");
                        sbr.AppendFormat("\t\t{0}= element.GetAttribute(\"{1}\");\r\n", dataArr[i, 0], dataArr[i, 0]);
                        sbr.Append("\t }\r\n\r\n");
                    }
                    else
                    {
                        sbr.AppendFormat("\t if (element.HasAttribute(\"{0}\") && !string.IsNullOrEmpty(element.GetAttribute(\"{1}\")))\r\n", dataArr[i, 0], dataArr[i, 0]);
                        sbr.Append("\t {\r\n");
                        sbr.AppendFormat("\t\t{0}= {1}.Parse( element.GetAttribute(\"{2}\"));\r\n", dataArr[i, 0], type, dataArr[i, 0]);
                        sbr.Append("\t }\r\n\r\n");                      
                    }

                }
                sbr.Append("     return this;\r\n");
                sbr.Append("  }\r\n");
               
            }



            sbr.Append("}\r\n");
            using (FileStream fs = new FileStream(string.Format("{0}/{1}Entity.cs", OutEntityFilePath, fileName), FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(sbr.ToString());
                }
            }

            #region  //  =======================创建Lua的实体
            //sbr.Clear();



            //sbr.AppendFormat("{0}Entity = {{ ", fileName);

            //for (int i = 0; i < dataArr.GetLength(0); i++)
            //{

            //    if (i == dataArr.GetLength(0) - 1)
            //    {
            //        if (dataArr[i, 1].Equals("string", StringComparison.CurrentCultureIgnoreCase))
            //        {
            //            sbr.AppendFormat("{0} = \"\"", dataArr[i, 0]);
            //        }
            //        else
            //        {
            //            sbr.AppendFormat("{0} = 0", dataArr[i, 0]);
            //        }
            //    }
            //    else
            //    {
            //        if (dataArr[i, 1].Equals("string", StringComparison.CurrentCultureIgnoreCase))
            //        {
            //            sbr.AppendFormat("{0} = \"\", ", dataArr[i, 0]);
            //        }
            //        else
            //        {
            //            sbr.AppendFormat("{0} = 0, ", dataArr[i, 0]);
            //        }
            //    }
            //}
            //sbr.Append(" }\r\n");

            //sbr.Append("\r\n");
            //sbr.AppendFormat("{0}Entity.__index = {0}Entity;\r\n", fileName);
            //sbr.Append("\r\n");
            //sbr.AppendFormat("function {0}Entity.New(", fileName);
            //for (int i = 0; i < dataArr.GetLength(0); i++)
            //{
            //    if (i == dataArr.GetLength(0) - 1)
            //    {
            //        sbr.AppendFormat("{0}", dataArr[i, 0]);
            //    }
            //    else
            //    {
            //        sbr.AppendFormat("{0}, ", dataArr[i, 0]);
            //    }
            //}
            //sbr.Append(")\r\n");
            //sbr.Append("    local self = { };\r\n");
            //sbr.Append("");
            //sbr.AppendFormat("    setmetatable(self, {0}Entity);\r\n", fileName);
            //sbr.Append("\r\n");
            //for (int i = 0; i < dataArr.GetLength(0); i++)
            //{
            //    sbr.AppendFormat("    self.{0} = {0};\r\n", dataArr[i, 0]);
            //}
            //sbr.Append("\r\n");
            //sbr.Append("    return self;\r\n");
            //sbr.Append("end");

            //using (FileStream fs = new FileStream(string.Format("{0}/{1}Entity.bytes", OutLuaFilePath, fileName), FileMode.Create))
            //{
            //    using (StreamWriter sw = new StreamWriter(fs))
            //    {
            //        sw.Write(sbr.ToString());
            //    }
            //}

            #endregion
        }

        /// <summary>
        /// 创建客户端实体
        /// </summary>
        private static void CreateEntity(string fileName, string[,] dataArr)
        {
            if (dataArr == null) return;

            StringBuilder sbr = new StringBuilder();
            sbr.Append("\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("//作    者：Lixi  \r\n");
            sbr.AppendFormat("//创建时间：{0}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
          //  sbr.Append("//备    注：此代码为工具生成 请勿手工修改\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("using System.Collections;\r\n");           
            sbr.Append("\r\n");
            sbr.Append("/// <summary>\r\n");
            sbr.AppendFormat("/// {0}实体\r\n", fileName);
            sbr.Append("/// </summary>\r\n");
            sbr.AppendFormat("public partial class {0}Entity : {1}\r\n", fileName,configEntity.entityBaseName);
            sbr.Append("{\r\n");

            for (int i = 0; i < dataArr.GetLength(0); i++)
            {
                if (i == 0) continue;
                sbr.Append("    /// <summary>\r\n");
                sbr.AppendFormat("    /// {0}\r\n", dataArr[i, 2]);
                sbr.Append("    /// </summary>\r\n");
                sbr.AppendFormat("    public {0} {1};\r\n", dataArr[i, 1], dataArr[i, 0]);

                Console.WriteLine("type :" + dataArr[i, 1] + " " + dataArr[i, 0]);
                sbr.Append("\r\n");
            }

            sbr.Append("}\r\n");


            using (FileStream fs = new FileStream(string.Format("{0}/{1}Entity.cs", OutCSharpFilePath, fileName), FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(sbr.ToString());
                }
            }

            //=======================创建Lua的实体
            sbr.Clear();

            sbr.AppendFormat("{0}Entity = {{ ", fileName);

            for (int i = 0; i < dataArr.GetLength(0); i++)
            {

                if (i == dataArr.GetLength(0) - 1)
                {
                    if (dataArr[i, 1].Equals("string", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sbr.AppendFormat("{0} = \"\"", dataArr[i, 0]);
                    }
                    else
                    {
                        sbr.AppendFormat("{0} = 0", dataArr[i, 0]);
                    }
                }
                else
                {
                    if (dataArr[i, 1].Equals("string", StringComparison.CurrentCultureIgnoreCase))
                    {
                        sbr.AppendFormat("{0} = \"\", ", dataArr[i, 0]);
                    }
                    else
                    {
                        sbr.AppendFormat("{0} = 0, ", dataArr[i, 0]);
                    }
                }
            }
            sbr.Append(" }\r\n");

            sbr.Append("\r\n");
            sbr.AppendFormat("{0}Entity.__index = {0}Entity;\r\n", fileName);
            sbr.Append("\r\n");
            sbr.AppendFormat("function {0}Entity.New(", fileName);
            for (int i = 0; i < dataArr.GetLength(0); i++)
            {
                if (i == dataArr.GetLength(0) - 1)
                {
                    sbr.AppendFormat("{0}", dataArr[i, 0]);
                }
                else
                {
                    sbr.AppendFormat("{0}, ", dataArr[i, 0]);
                }
            }
            sbr.Append(")\r\n");
            sbr.Append("    local self = { };\r\n");
            sbr.Append("");
            sbr.AppendFormat("    setmetatable(self, {0}Entity);\r\n", fileName);
            sbr.Append("\r\n");
            for (int i = 0; i < dataArr.GetLength(0); i++)
            {
                sbr.AppendFormat("    self.{0} = {0};\r\n", dataArr[i, 0]);
            }
            sbr.Append("\r\n");
            sbr.Append("    return self;\r\n");
            sbr.Append("end");

            using (FileStream fs = new FileStream(string.Format("{0}/{1}Entity.bytes", OutLuaFilePath, fileName), FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(sbr.ToString());
                }
            }
        }

        /// <summary>
        /// 创建服务器端实体
        /// </summary>
        private static void CreateServerEntity(string fileName, string[,] dataArr)
        {
            if (dataArr == null) return;

            StringBuilder sbr = new StringBuilder();
            sbr.Append("\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("//作    者：Lixi\r\n");
            sbr.AppendFormat("//创建时间：{0}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
           // sbr.Append("//备    注：此代码为工具生成 请勿手工修改\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("using System.Collections;\r\n");
            sbr.Append("\r\n");
            sbr.Append("/// <summary>\r\n");
            sbr.AppendFormat("/// {0}实体\r\n", fileName);
            sbr.Append("/// </summary>\r\n");
            sbr.AppendFormat("public partial class {0}Entity : DataTableEntityBase\r\n", fileName);
            sbr.Append("{\r\n");

            for (int i = 0; i < dataArr.GetLength(0); i++)
            {
                if (i == 0) continue;
                sbr.Append("    /// <summary>\r\n");
                sbr.AppendFormat("    /// {0}\r\n", dataArr[i, 2]);
                sbr.Append("    /// </summary>\r\n");
                sbr.AppendFormat("    public {0} {1};\r\n", dataArr[i, 1], dataArr[i, 0]);
                sbr.Append("\r\n");
            }

            sbr.Append("}\r\n");


            using (FileStream fs = new FileStream(string.Format("{0}/{1}Entity.cs", OutCSharpFilePath_Server, fileName), FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(sbr.ToString());
                }
            }
        }

        /// <summary>
        /// 创建客户端数据管理类
        /// </summary>
        private static void NewCreateDBModel(string fileName, string[,] dataArr)
        {
            if (dataArr == null) return;

            StringBuilder sbr = new StringBuilder();
            sbr.Append("\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("//作    者：Lixi  \r\n");
            sbr.AppendFormat("//创建时间：{0}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //   sbr.Append("//备    注：此代码为工具生成 请勿手工修改\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("using System.Collections;\r\n");
            sbr.Append("using System.Collections.Generic;\r\n");
            sbr.Append("using System;\r\n");
            sbr.Append("using System.Xml;\r\n");

            sbr.Append("\r\n");
            sbr.Append("/// <summary>\r\n");
            sbr.AppendFormat("/// {0}数据管理\r\n", fileName);
            sbr.Append("/// </summary>\r\n");
            sbr.AppendFormat("public partial class {0}DBModel : {1}<{0}DBModel, {0}Entity>\r\n", fileName, SettingConfigDBModel.Instance.Get(ConstDefine.dbModelBaseName));
            sbr.Append("{\r\n");

            sbr.Append("    /// <summary>\r\n");
            sbr.Append("    /// 文件名称\r\n");
            sbr.Append("    /// </summary>\r\n");
            sbr.AppendFormat("    public override string DataTableName {{ get {{ return \"{0}\"; }} }}\r\n", fileName);
            sbr.Append("\r\n");


            sbr.Append("    /// <summary>\r\n");
            sbr.Append("    /// 加载列表\r\n");
            sbr.Append("    /// </summary>\r\n");
            sbr.Append("    protected override void LoadList(string dataTableData)\r\n");
            sbr.Append("    {\r\n");
            sbr.Append("        XmlDocument doc = new XmlDocument();\r\n");
            sbr.Append("        doc.LoadXml(dataTableData);\r\n");
            sbr.Append("        XmlElement root = doc.DocumentElement;\r\n");
            sbr.Append("\r\n");
            sbr.Append("        foreach (XmlElement node in root)\r\n");
            sbr.Append("        {\r\n");
            sbr.AppendFormat("            {0}Entity entity = new {0}Entity();\r\n", fileName);

            sbr.AppendFormat("            entity = entity.XmlToObject(node);\r\n");

            sbr.Append("\r\n");
            sbr.Append("            m_List.Add(entity);\r\n");
            sbr.Append("            m_Dic[entity.id] = entity;\r\n");
            sbr.Append("        }\r\n");
            sbr.Append("    }\r\n");

            sbr.Append("}");
            using (FileStream fs = new FileStream(string.Format("{0}/{1}DBModel.cs", OutDbModelFilePath, fileName), FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(sbr.ToString());
                }
            }

            //===============生成lua的DBModel
            //sbr.Clear();
            //sbr.Append("--数据访问\r\n");
            //sbr.AppendFormat("{0}DBModel = {{ }}\r\n", fileName);
            //sbr.Append("\r\n");
            //sbr.AppendFormat("local this = {0}DBModel;\r\n", fileName);
            //sbr.Append("\r\n");
            //sbr.AppendFormat("local {0}Table = {{ }}; --定义表格\r\n", fileName.ToLower());
            //sbr.Append("\r\n");
            //sbr.AppendFormat("function {0}DBModel.LoadList()\r\n", fileName);
            //sbr.AppendFormat("    local ms = CS.YouYou.GameEntry.Lua:LoadDataTable(\"{0}\");\r\n", fileName);
            //sbr.Append("    local rows = ms:ReadInt();\r\n");
            //sbr.Append("    ms:ReadInt();\r\n");
            //sbr.Append("\r\n");
            //sbr.Append("    for i = 0, rows, 1 do\r\n");
            //sbr.AppendFormat("        {0}Table[#{0}Table + 1] = {1}Entity.New(\r\n", fileName.ToLower(), fileName);

            //string str = "";
            //for (int i = 0; i < dataArr.GetLength(0); i++)
            //{
            //    if (dataArr[i, 1].Equals("byte", StringComparison.CurrentCultureIgnoreCase))
            //    {
            //        str += string.Format("                ms:Read{1}(),\r\n", dataArr[i, 0], ChangeTypeName(dataArr[i, 1]));
            //    }
            //    else
            //    {
            //        str += string.Format("                ms:Read{1}(),\r\n", dataArr[i, 0], ChangeTypeName(dataArr[i, 1]));
            //    }
            //}
            //str = str.TrimEnd(',', '\r', '\n');
            //sbr.AppendFormat("{0}\r\n", str);
            //sbr.Append("        );\r\n");
            //sbr.Append("    end\r\n");
            //sbr.Append("\r\n");
            //sbr.Append("end\r\n");
            //sbr.Append("\r\n");
            //sbr.AppendFormat("function {0}DBModel.GetList()\r\n", fileName);
            //sbr.AppendFormat("    return {0}Table;\r\n", fileName.ToLower());
            //sbr.Append("end");
            //sbr.Append("\r\n");
            //sbr.Append("\r\n");
            //sbr.AppendFormat("function {0}DBModel.GetEntity(id)\r\n", fileName);
            //sbr.AppendFormat("    local ret = nil;\r\n");
            //sbr.AppendFormat("    for i = 1, #{0}Table, 1 do\r\n", fileName.ToLower());
            //sbr.AppendFormat("        if ({0}Table[i].Id == id) then\r\n", fileName.ToLower());
            //sbr.AppendFormat("            ret = {0}Table[i];\r\n", fileName.ToLower());
            //sbr.AppendFormat("            break;\r\n");
            //sbr.AppendFormat("        end\r\n");
            //sbr.AppendFormat("    end\r\n");
            //sbr.AppendFormat("    return ret;\r\n");
            //sbr.AppendFormat("end");

            //using (FileStream fs = new FileStream(string.Format("{0}/{1}DBModel.bytes", OutLuaFilePath, fileName), FileMode.Create))
            //{
            //    using (StreamWriter sw = new StreamWriter(fs))
            //    {
            //        sw.Write(sbr.ToString());
            //    }
            //}
        }

        /// <summary>
        /// 创建客户端数据管理类
        /// </summary>
        private static void CreateDBModel(string fileName, string[,] dataArr)
        {
            if (dataArr == null) return;

            StringBuilder sbr = new StringBuilder();
            sbr.Append("\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("//作    者：Lixi  \r\n");
            sbr.AppendFormat("//创建时间：{0}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
         //   sbr.Append("//备    注：此代码为工具生成 请勿手工修改\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("using System.Collections;\r\n");
            sbr.Append("using System.Collections.Generic;\r\n");
            sbr.Append("using System;\r\n");
         
            sbr.Append("\r\n");
            sbr.Append("/// <summary>\r\n");
            sbr.AppendFormat("/// {0}数据管理\r\n", fileName);
            sbr.Append("/// </summary>\r\n");
            sbr.AppendFormat("public partial class {0}DBModel : {1}<{0}DBModel, {0}Entity>\r\n", fileName,SettingConfigDBModel.Instance.Get(ConstDefine.dbModelBaseName));
            sbr.Append("{\r\n");

            sbr.Append("    /// <summary>\r\n");
            sbr.Append("    /// 文件名称\r\n");
            sbr.Append("    /// </summary>\r\n");
            sbr.AppendFormat("    public override string DataTableName {{ get {{ return \"{0}\"; }} }}\r\n", fileName);
            sbr.Append("\r\n");


            sbr.Append("    /// <summary>\r\n");
            sbr.Append("    /// 加载列表\r\n");
            sbr.Append("    /// </summary>\r\n");
            sbr.Append("    protected override void LoadList(MMO_MemoryStream ms)\r\n");
            sbr.Append("    {\r\n");
            sbr.Append("        int rows = ms.ReadInt();\r\n");
            sbr.Append("        int columns = ms.ReadInt();\r\n");
            sbr.Append("\r\n");
            sbr.Append("        for (int i = 0; i < rows; i++)\r\n");
            sbr.Append("        {\r\n");
            sbr.AppendFormat("            {0}Entity entity = new {0}Entity();\r\n", fileName);

            for (int i = 0; i < dataArr.GetLength(0); i++)
            {
                if (dataArr[i, 1].Equals("byte", StringComparison.CurrentCultureIgnoreCase))
                {
                    sbr.AppendFormat("            entity.{0} = (byte)ms.Read{1}();\r\n", dataArr[i, 0], ChangeTypeName(dataArr[i, 1]));
                }
                else
                {
                    sbr.AppendFormat("            entity.{0} = ms.Read{1}();\r\n", dataArr[i, 0], ChangeTypeName(dataArr[i, 1]));
                }
            }

            sbr.Append("\r\n");
            sbr.Append("            m_List.Add(entity);\r\n");
            sbr.Append("            m_Dic[entity.Id] = entity;\r\n");
            sbr.Append("        }\r\n");
            sbr.Append("    }\r\n");

            sbr.Append("}");
            using (FileStream fs = new FileStream(string.Format("{0}/{1}DBModel.cs", OutDbModelFilePath, fileName), FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(sbr.ToString());
                }
            }

            //===============生成lua的DBModel
            sbr.Clear();
            sbr.Append("--数据访问\r\n");
            sbr.AppendFormat("{0}DBModel = {{ }}\r\n", fileName);
            sbr.Append("\r\n");
            sbr.AppendFormat("local this = {0}DBModel;\r\n", fileName);
            sbr.Append("\r\n");
            sbr.AppendFormat("local {0}Table = {{ }}; --定义表格\r\n", fileName.ToLower());
            sbr.Append("\r\n");
            sbr.AppendFormat("function {0}DBModel.LoadList()\r\n", fileName);
            sbr.AppendFormat("    local ms = CS.YouYou.GameEntry.Lua:LoadDataTable(\"{0}\");\r\n", fileName);
            sbr.Append("    local rows = ms:ReadInt();\r\n");
            sbr.Append("    ms:ReadInt();\r\n");
            sbr.Append("\r\n");
            sbr.Append("    for i = 0, rows, 1 do\r\n");
            sbr.AppendFormat("        {0}Table[#{0}Table + 1] = {1}Entity.New(\r\n", fileName.ToLower(), fileName);

            string str = "";
            for (int i = 0; i < dataArr.GetLength(0); i++)
            {
                if (dataArr[i, 1].Equals("byte", StringComparison.CurrentCultureIgnoreCase))
                {
                    str += string.Format("                ms:Read{1}(),\r\n", dataArr[i, 0], ChangeTypeName(dataArr[i, 1]));
                }
                else
                {
                    str += string.Format("                ms:Read{1}(),\r\n", dataArr[i, 0], ChangeTypeName(dataArr[i, 1]));
                }
            }
            str = str.TrimEnd(',', '\r', '\n');
            sbr.AppendFormat("{0}\r\n", str);
            sbr.Append("        );\r\n");
            sbr.Append("    end\r\n");
            sbr.Append("\r\n");
            sbr.Append("end\r\n");
            sbr.Append("\r\n");
            sbr.AppendFormat("function {0}DBModel.GetList()\r\n", fileName);
            sbr.AppendFormat("    return {0}Table;\r\n", fileName.ToLower());
            sbr.Append("end");
            sbr.Append("\r\n");
            sbr.Append("\r\n");
            sbr.AppendFormat("function {0}DBModel.GetEntity(id)\r\n", fileName);
            sbr.AppendFormat("    local ret = nil;\r\n");
            sbr.AppendFormat("    for i = 1, #{0}Table, 1 do\r\n", fileName.ToLower());
            sbr.AppendFormat("        if ({0}Table[i].Id == id) then\r\n", fileName.ToLower());
            sbr.AppendFormat("            ret = {0}Table[i];\r\n", fileName.ToLower());
            sbr.AppendFormat("            break;\r\n");
            sbr.AppendFormat("        end\r\n");
            sbr.AppendFormat("    end\r\n");
            sbr.AppendFormat("    return ret;\r\n");
            sbr.AppendFormat("end");

            using (FileStream fs = new FileStream(string.Format("{0}/{1}DBModel.bytes", OutLuaFilePath, fileName), FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(sbr.ToString());
                }
            }
        }

        /// <summary>
        /// 创建服务器端数据管理类
        /// </summary>
        private static void CreateServerDBModel(string fileName, string[,] dataArr)
        {
            if (dataArr == null) return;

            StringBuilder sbr = new StringBuilder();
            sbr.Append("\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("//作    者：Lixi \r\n");
            sbr.AppendFormat("//创建时间：{0}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sbr.Append("//备    注：此代码为工具生成 请勿手工修改\r\n");
            sbr.Append("//===================================================\r\n");
            sbr.Append("using System.Collections;\r\n");
            sbr.Append("using System.Collections.Generic;\r\n");
            sbr.Append("using System;\r\n");
            sbr.Append("\r\n");
            sbr.Append("/// <summary>\r\n");
            sbr.AppendFormat("/// {0}数据管理\r\n", fileName);
            sbr.Append("/// </summary>\r\n");
            sbr.AppendFormat("public partial class {0}DBModel : DataTableDBModelBase<{0}DBModel, {0}Entity>\r\n", fileName);
            sbr.Append("{\r\n");

            sbr.Append("    /// <summary>\r\n");
            sbr.Append("    /// 文件名称\r\n");
            sbr.Append("    /// </summary>\r\n");
            sbr.AppendFormat("    public override string DataTableName {{ get {{ return \"{0}\"; }} }}\r\n", fileName);
            sbr.Append("\r\n");


            sbr.Append("    /// <summary>\r\n");
            sbr.Append("    /// 加载列表\r\n");
            sbr.Append("    /// </summary>\r\n");
            sbr.Append("    protected override void LoadList(MMO_MemoryStream ms)\r\n");
            sbr.Append("    {\r\n");
            sbr.Append("        int rows = ms.ReadInt();\r\n");
            sbr.Append("        int columns = ms.ReadInt();\r\n");
            sbr.Append("\r\n");
            sbr.Append("        for (int i = 0; i < rows; i++)\r\n");
            sbr.Append("        {\r\n");
            sbr.AppendFormat("            {0}Entity entity = new {0}Entity();\r\n", fileName);

            for (int i = 0; i < dataArr.GetLength(0); i++)
            {
                if (dataArr[i, 1].Equals("byte", StringComparison.CurrentCultureIgnoreCase))
                {
                    sbr.AppendFormat("            entity.{0} = (byte)ms.Read{1}();\r\n", dataArr[i, 0], ChangeTypeName(dataArr[i, 1]));
                }
                else
                {
                    sbr.AppendFormat("            entity.{0} = ms.Read{1}();\r\n", dataArr[i, 0], ChangeTypeName(dataArr[i, 1]));
                }
            }

            sbr.Append("\r\n");
            sbr.Append("            m_List.Add(entity);\r\n");
            sbr.Append("            m_Dic[entity.Id] = entity;\r\n");
            sbr.Append("        }\r\n");
            sbr.Append("    }\r\n");

            sbr.Append("}");
            using (FileStream fs = new FileStream(string.Format("{0}/{1}DBModel.cs", OutCSharpFilePath_Server, fileName), FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(sbr.ToString());
                }
            }
        }

        private static string ChangeTypeName(string type)
        {
            string str = string.Empty;

            switch (type)
            {
                case "byte":
                    str = "Byte";
                    break;
                case "int":
                    str = "Int";
                    break;
                case "short":
                    str = "Short";
                    break;
                case "long":
                    str = "Long";
                    break;
                case "float":
                    str = "Float";
                    break;
                case "string":
                    str = "UTF8String";
                    break;
            }

            return str;
        }
        #endregion

        #region 创建多语言表
        private static void CreateLocalization(string fileName, DataTable dt)
        {
            try
            {
                int rows = dt.Rows.Count;
                int columns = dt.Columns.Count;

                int newcolumns = columns - 3; //减去前三列 后面表示有多少种语言

                int currKeyColumn = 2; //当前的Key列
                int currValueColumn = 3; //当前的值列

                tableHeadArr = new string[columns, 3];

                while (newcolumns > 0)
                {
                    newcolumns--;

                    #region 写入文件
                    byte[] buffer = null;

                    using (MMO_MemoryStream ms = new MMO_MemoryStream())
                    {
                        ms.WriteInt(rows - 3); //减去表头的三行
                        ms.WriteInt(2); //多语言表 只有2列 Key Value

                        for (int i = 0; i < rows; i++)
                        {
                            for (int j = 0; j < columns; j++)
                            {
                                if (i < 3)
                                {
                                    tableHeadArr[j, i] = dt.Rows[i][j].ToString().Trim();
                                }
                                else
                                {
                                    if (j == currKeyColumn)
                                    {
                                        //写入key
                                        string value = dt.Rows[i][j].ToString().Trim();
                                        ms.WriteUTF8String(value);
                                    }
                                    else if (j == currValueColumn)
                                    {
                                        //写入value
                                        string value = dt.Rows[i][j].ToString().Trim();
                                        ms.WriteUTF8String(value);
                                    }
                                }
                            }
                        }
                        buffer = ms.ToArray();
                    }

                    //------------------
                    //写入文件
                    //------------------
                    FileStream fs = new FileStream(string.Format("{0}/Localization/{1}", OutBytesFilePath, tableHeadArr[currValueColumn, 0] + ".bytes"), FileMode.Create);
                    fs.Write(buffer, 0, buffer.Length);
                    fs.Close();

                    currValueColumn++;
                    #endregion
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("表格=>" + fileName + " 处理失败:" + ex.Message);
            }
        }
        #endregion
    }
}
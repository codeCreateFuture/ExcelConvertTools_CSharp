
//===================================================
//作    者：Lixi  
//创建时间：2019-06-11 18:24:09
//===================================================
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;

/// <summary>
/// SettingConfig数据管理
/// </summary>
public partial class SettingConfigDBModel
{
    protected List<SettingConfigEntity> m_List;
    protected Dictionary<string, SettingConfigEntity> m_Dic;

    public SettingConfigDBModel()
    {
        m_List = new List<SettingConfigEntity>();
        m_Dic = new Dictionary<string, SettingConfigEntity>();

        LoadList();
    }

    
    private static SettingConfigDBModel instance;

    public static SettingConfigDBModel Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SettingConfigDBModel();
            }
            return instance;
        }
    }
    /// <summary>
    /// 文件名称
    /// </summary>
    public string DataTableName { get { return "SettingConfig"; } }

    /// <summary>
    /// 加载列表
    /// </summary>
    protected void LoadList()
    {
        string xmlFilePath = Environment.CurrentDirectory + "\\SettingConfig.xml";
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
                    SettingConfigEntity entity = new SettingConfigEntity();
                    entity.XmlToObject(node);
                    m_List.Add(entity);
                    m_Dic[entity.key] = entity;

                }

                if (m_List.Count == 0) ExcelTool.Program.WriteColorLine("xml配置文件有错误", ConsoleColor.Red);
            }
            catch (Exception ex)
            {
                Console.WriteLine("读取xml配置文件失败 请检查文件格式是否有误: " + ex.Message);
            }

        }
        else
        {
            ExcelTool.Program.WriteColorLine("不存在 生成各种配置文件的 xml配置文件", ConsoleColor.Red);
        }
    }


    public List<SettingConfigEntity> GetList()
    {
        return m_List;
    }

    /// <summary>
    /// 根据编号获取实体
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string Get(string key)
    {
        if (m_Dic.ContainsKey(key))
        {
            return m_Dic[key].value;
        }
        return null;
    }
}